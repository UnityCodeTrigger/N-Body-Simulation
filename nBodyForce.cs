using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nBodyForce : MonoBehaviour
{

    List<Bodies> AllBodies;
    public bool merge = false;
    public bool randomColors = true;
    public bool showLines = false;

    private void Awake()
    {
        Time.fixedDeltaTime = 0.01f;            
        Application.targetFrameRate = 60;       //Limita la frequencia de fotogramas

        AllBodies = new List<Bodies>();
    }

    private void Update()
    {
        //Dynamic body
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CreateParticle(mousePos.x, mousePos.y,false);
        }
        //Static body
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CreateParticle(mousePos.x, mousePos.y,true);
        }

        //Remove last body
        if (Input.GetKeyDown(KeyCode.R))
            RemoveLastBody();
        //Color body
        if (Input.GetKeyDown(KeyCode.C))
            randomColors = !randomColors;
        //Lines
        if (Input.GetKeyDown(KeyCode.L))
            showLines = !showLines;
    }

    private void FixedUpdate()
    {
        //Calculate acceleration
        CalculateForceToAllBodies();

        //Merge Bodies (Not good idea)
        if(merge) MergeBodies(1f);
    }

    public void CreateParticle(float x,float y,bool isStatic)
    {   
        GameObject _particle = new GameObject();                            //Create Gameobject from code.
        _particle.AddComponent<Bodies>();
        _particle.transform.position = new Vector3(x, y, 0);                //Set random position.
        Bodies particle = _particle.GetComponent<Bodies>();                 //Add the body component.
        AddBody(particle);                                                 //Add the body to all bodies list.
        if(randomColors)
            particle.RandomParticleColor();
        particle.gameObject.name = "Body " + AllBodies.Count.ToString();
        particle.SetMass(Random.RandomRange(3f,25f));                       //Set random mass beetween 3-25.
        particle.staticBody = isStatic;
    }

    //Most important part from script (Gravity Logic)
    void CalculateForceToAllBodies()
    {
        if (AllBodies.Count >= 2)                                            //Gravity only afects n>=2 bodies
        {
            for (int j = AllBodies.Count - 1; j >= 0; j--)
            {
                for (int i = AllBodies.Count - 1; i >= 0; i--)
                {
                    Bodies body1 = AllBodies[j];
                    Bodies body2 = AllBodies[i];
                    ApplyCentralForce(body1, body2, 1f, 3f);                //Calculate Acceleration

                    body1.SetVelocity();                                    //set velocity to body
                    body1.SetPosition();                                    //update position to body (current position + velocity)
                    body2.SetVelocity();
                    body2.SetPosition();

                    //Feedback
                    if (showLines)
                    {
                        if (randomColors)
                            body1.DrawLineNearestBody(FindBiggerParticle(body1, 10f), body1._particleColor);
                        else
                            body1.DrawLineNearestBody(FindBiggerParticle(body1, 10f), Color.black);
                    }
                    else
                        body1.ResetLine();
                    //End feedback
                }
            }
        }
    }

    Bodies FindBiggerParticle(Bodies body, float limit)
    {
        Bodies biggerBody = body;

        foreach (Bodies bodyInCol in AllBodies)
        {
            if(Vector2.Distance(body._Position,bodyInCol._Position) < limit)
            {
                if (body._mass < bodyInCol._mass && bodyInCol._mass > biggerBody._mass)
                {
                    biggerBody = bodyInCol;
                }
            }
        }
        return biggerBody;
    }

    //The bad idea :)
    void MergeBodies(float minDistance)
    {
        if (AllBodies.Count > 1)
        {
            for (int i = AllBodies.Count - 1; i >= 0; i--)
            {
                for (int j = AllBodies.Count - 1; j >= 0; j--)
                {
                    Bodies body1 = AllBodies[j];
                    Bodies body2 = AllBodies[i];

                    if (Vector2.Distance(body1._Position, body2._Position) < minDistance)
                    {
                        if (body1._mass > body2._mass)
                        {
                            body1.SetMass(body1._mass + body2._mass);
                            body2.gameObject.SetActive(false);
                            RemoveBody(body2);
                            return;
                        }
                        else if (body2._mass > body1._mass)
                        {
                            body2.SetMass(body2._mass + body1._mass);
                            body1.gameObject.SetActive(false);
                            RemoveBody(body1);
                            return;
                        }
                    }
                }
            }
        }
    }

    void ApplyCentralForce(Bodies a, Bodies b, float strenght, float minDis)
    {
        //Newton's law of universal gravitation: (G*m1*m2/r^2)u

        Vector2 dir = b._Position - a._Position;                    //u = direction normalized (here is not normalized)
        float d = dir.magnitude;                                    //r = direction magnitude
        if (d < minDis) d = minDis;                                 //this fixes a bug that happens when you divide by 0 (when two bodies are very close together it digs the possibility of dividing by 0)
        dir.Normalize();                                     

        float force = (strenght * a._mass * b._mass) / (d * d);     //Law of universal gravitation apllied to code (Only gives the acceleration)
        d = d * force;                                              //formula * u (gives the vector force)
        a.SetAcceleration(dir);
        b.SetAcceleration(-dir);
    }

    public void AddBody(Bodies body)
    {
        AllBodies.Add(body);
        FindObjectOfType<UI_Manager>().UpdateNumberOfBodies(AllBodies.Count);
    }

    public void RemoveBody(Bodies body)
    {
        AllBodies.Remove(body);
        FindObjectOfType<UI_Manager>().UpdateNumberOfBodies(AllBodies.Count);
    }

    public void RemoveLastBody()
    {
        Bodies body = AllBodies[AllBodies.Count - 1];
        RemoveBody(body);
        Destroy(body.gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodies : MonoBehaviour
{
    float mass;
    [SerializeField] Vector3 velocity;
    Vector3 acceleration;
    Color particleColor;
    public bool staticBody = false;
    //
    float topVel = 5;

    private void OnEnable()
    {
        Instantiate(Resources.Load("ParticleRenderer"),transform);
        LineRenderer_NearestBody_Setup();
    }

    void LineRenderer_NearestBody_Setup()
    {
        GameObject lrGo = new GameObject();
        lrGo.transform.parent = transform;
        lrGo.gameObject.name = "LR-NearestObj";
        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetColors(Color.black, Color.black);
        lr.material = Resources.Load("ParticleMaterial") as Material;
    }

    public void DrawLineNearestBody(Bodies body)
    {
        _lineRendererNearestBody.SetPosition(0, transform.position);
        _lineRendererNearestBody.SetPosition(1, body._Position);

        float d = Vector2.Distance(body._Position, _Position);
        if (d < _radius) d = _radius;

        _lineRendererNearestBody.SetWidth((this._radius/ 2) / d, (body._radius / 2) / d);
    }
    public void DrawLineNearestBody(Bodies body, Color c)
    {
        _lineRendererNearestBody.SetPosition(0, transform.position);
        _lineRendererNearestBody.SetPosition(1, body._Position);
        _lineRendererNearestBody.SetColors(particleColor, c);

        float d = Vector2.Distance(body._Position, _Position);
        if (d < _radius) d = _radius;

        _lineRendererNearestBody.SetWidth((this._radius / 2) / d, (body._radius / 2) / d);
    }
    public void ResetLine()
    {
        _lineRendererNearestBody.SetPosition(0, transform.position);
        _lineRendererNearestBody.SetPosition(1, transform.position);
    }
    public void RandomParticleColor()
    {
        Color c = new Color(Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f),1);
        particleColor = c;
        GetComponentInChildren<SpriteRenderer>().color = c;
    }

    public void SetMass(float _mass)
    {
        mass = _mass;
        ChangeSize();
    }
    void ChangeSize()
    {
        transform.GetChild(0).localScale = Vector3.one * (mass);
    }

    //The most important
    public void SetPosition()
    {
        if (!staticBody)
            transform.position = transform.position + velocity;
    }
    //The most important
    public void SetVelocity()
    {
        if (!staticBody)
        {
            velocity = velocity + acceleration;
            velocity = Vector2.ClampMagnitude(velocity, topVel);
        }
    }
    //The most important
    public void SetAcceleration(Vector3 force)
    {
        if (!staticBody)
        {
            float G = 0.0001f;
            Vector2 vectorG = new Vector2(G, G);
            acceleration *= 0;

            acceleration += force / mass;
            acceleration *= vectorG;
            Vector3.ClampMagnitude(velocity, topVel);
        }
    }

    //Only get variables
    public Vector2 _Position => transform.position;
    public Vector2 _velocity => this.velocity;
    public float _mass => this.mass;
    public float _radius => mass/2;
    public Color _particleColor => this.particleColor;
    public LineRenderer _lineRendererNearestBody => transform.GetChild(1).GetComponent<LineRenderer>();
}

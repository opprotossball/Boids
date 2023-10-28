using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidStats _stats;
    public BoidStats Stats {
        get
        {
            if (_stats.isDefault)
            {
                return HiveManager.GetHiveStats(_stats.hiveId);
            }
            else
            {
                return _stats;
            }
        }
        set { _stats = value; }
    }
    public HiveManager HiveManager { get; set; }
    private Rigidbody rb;
    private static readonly float DegreesPerRay = 3f;

    public void SimulateStep(IEnumerable<Boid> boids)
    {
        AddForce(CalculateDirection(boids));
    }

    private Vector3 CalculateDirection(IEnumerable<Boid> boids)
    {
        float sightRange = Stats.sightRange;
        Vector3 spearationDirection = new Vector3 (0, 0, 0);
        Vector3 aligmentDirection = new Vector3(0, 0, 0);
        Vector3 centroid = new Vector3(0, 0, 0);
        int nOthers = 0;

        foreach (Boid boid in boids)
        {
            nOthers++;
            Vector3 diff = GetPosition() - boid.GetPosition();
            spearationDirection += diff.normalized * (sightRange - diff.magnitude);
            aligmentDirection += boid.GetVelocity();
            centroid += boid.GetPosition();
        }

        Vector3 cohesionDirection = centroid / nOthers - GetPosition();

        return spearationDirection.normalized * Stats.separation +
               aligmentDirection.normalized * Stats.aligment +
               cohesionDirection.normalized * Stats.cohesion +
               AvoidObstaclesDirection().normalized * Stats.avoidForce +
               Random.insideUnitSphere.normalized * Stats.randomForce;
    }

    private Vector3 AvoidObstaclesDirection()
    {
        Vector3 d = new Vector3(0, 0, 0);
        Vector3 rayDirection;
        int nSteps = Mathf.CeilToInt(Stats.viewAngle / DegreesPerRay);
        RaycastHit hit;
        for (int i = 0; i < nSteps; i++)
        {
            rayDirection = Quaternion.AngleAxis(RotationAngle(i, DegreesPerRay), transform.up) * transform.forward;
            for (int j = 0; j < nSteps; j++)
            {
                Vector3 dir = Quaternion.AngleAxis(RotationAngle(j, DegreesPerRay), transform.right) * rayDirection;
                Ray ray = new Ray(transform.position, dir);
                if (!Physics.Raycast(ray, out hit, Stats.sightRange, LayerMask.GetMask("Obstacles")))
                {
                    return dir - transform.forward;
                }
            }
        }
        return -transform.forward;
    }

    private float RotationAngle(int i, float degreesPerStep)
    {
        return i % 2 == 0 ? degreesPerStep * i / 2 : -degreesPerStep * i / 2; 
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.AddForce(transform.forward * 1.5f, mode: ForceMode.Impulse);
    }

    private void Start()
    {
    }

    public bool IsInSightAngle(Vector3 dir)
    {
        return Vector3.Angle(GetDirection(), dir) < Stats.viewAngle;
    }

    public Vector3 GetPosition()
    {
        return rb.position;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public Vector3 GetDirection()
    {
        return GetVelocity().normalized;
    }

    private void AddForce(Vector3 dir)
    {
        rb.AddForce(dir * Stats.force, mode: ForceMode.Impulse);
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("HIT");
    }
}

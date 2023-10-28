using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class HiveManager : MonoBehaviour
{
    private List<Boid> boids;
    private int nHives;
    private List<BoidStats> hiveStats;
    private BoidStats defaultStats;
    [SerializeField] private GameObject boidPrefab;

    public void SetViewAngle(float value, int hiveId)
    {
        VerifyHiveExists(hiveId);
        BoidStats stats = hiveStats[hiveId];
        stats.viewAngle = value;
        hiveStats[hiveId] = stats;
        //UpdateStats(int hiveId);
    }

    //private void UpdateStats(int hiveId)
    //{

    //}

    public int GetNumOfHives()
    {
        return nHives;
    }

    public void SpawnBoid(int hiveId)
    {
        float box = 1.5f;
        Vector3 position = new Vector3(Random.Range(-box, box), Random.Range(-box, box), Random.Range(-box, box));
        //Vector3 position = new Vector3(0,0,0);
        SpawnBoid(hiveId, position);
    }

    public void SpawnBoid(int hiveId, Vector3 position)
    {
        GameObject boid = GameObject.Instantiate(boidPrefab, position, Quaternion.identity);
        Boid boidScript = boid.GetComponent<Boid>();
        BoidStats stats = defaultStats;
        stats.hiveId = hiveId;
        boidScript.Stats = stats;
        boidScript.HiveManager = this;
        boids.Add(boidScript);
    }

    public BoidStats GetHiveStats(int hiveId)
    {
        VerifyHiveExists(hiveId);
        return hiveStats[hiveId];
    }

    private void VerifyHiveExists(int hiveId)
    {
        if (hiveId >= nHives)
        {
            Debug.LogException(new Exception("Hive do not exist!"));
        }
    }

    public void CreateHive(BoidStats stats)
    {
        stats.hiveId = nHives++;
        hiveStats.Add(stats);
    }

    private HiveManager() 
    { 
        boids = new List<Boid>();
        hiveStats = new List<BoidStats>();
        defaultStats = new BoidStats();
        defaultStats.isDefault = true;
    }

    public IEnumerable<Boid> GetBoidsInRange(Boid boid)
    {
        foreach (Boid other in boids) 
        {
            if (boid == other)
            {
                continue;
            }
            Vector3 diff = boid.GetPosition() - other.GetPosition();
            if (diff.magnitude <= boid.Stats.sightRange && boid.IsInSightAngle(diff))
            {
                yield return other;
            }
        }
    }

    public static HiveManager Instance { get { return NestedSingleton.Instance; } }
    private class NestedSingleton
    {
        static NestedSingleton() { }
        internal static readonly HiveManager Instance = new HiveManager();
    }

    private void FixedUpdate()
    {
        foreach (Boid boid in boids)
        {
            boid.SimulateStep(GetBoidsInRange(boid));
        }
    }
}

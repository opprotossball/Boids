using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float separation;
    [SerializeField] private float aligment;
    [SerializeField] private float cohesion;
    [SerializeField] private float sightRange;
    [SerializeField] private float randomForce;
    [SerializeField] private int numberOfBoids;
    [SerializeField] private float avoidForce;
    [SerializeField] private int viewAngle;
    private HiveManager hm;
    private BoidStats stats;
    void Start()
    {
        hm = GameObject.Find("Hive").GetComponent<HiveManager>();
        stats.force = force;
        stats.separation = separation;
        stats.aligment = aligment;
        stats.cohesion = cohesion;
        stats.sightRange = sightRange;  
        stats.randomForce = randomForce;
        stats.avoidForce = avoidForce;
        stats.viewAngle = viewAngle;
        hm.CreateHive(stats);
        for (int i = 0; i < numberOfBoids; i++)
        {
            hm.SpawnBoid(0);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            hm.SpawnBoid(0);
        }
    }
}

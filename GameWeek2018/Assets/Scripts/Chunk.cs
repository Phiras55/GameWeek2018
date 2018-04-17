using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Entry and Exit Points")]
    [SerializeField] private Transform entryPoint;
    [SerializeField] public Transform exitPoint;

    [Header("Obstacles")]
    [SerializeField] private bool           generateObstacles;
    [SerializeField] private int            minObstaclesCount;
    [SerializeField] private int            maxObstaclesCount;
    [SerializeField] private List<string>   obstaclesID;
    [Range(0, 100)]
    [SerializeField] private List<float>    obstacleSpawnChance;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void SpawnObstacles()
    {

    }

    public void InitAndPlace(Transform lastChunkExitPoint)
    {
        Vector3 dirToExit = (lastChunkExitPoint.position - lastChunkExitPoint.root.position).normalized;

        transform.position = lastChunkExitPoint.position + dirToExit * (entryPoint.position + transform.position).magnitude;

        transform.rotation = Quaternion.LookRotation(dirToExit);
    }
}

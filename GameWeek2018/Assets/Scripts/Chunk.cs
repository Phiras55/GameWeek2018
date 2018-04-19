using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Entry and Exit Points")]
    [SerializeField] private Transform  entryPoint;
    [SerializeField] public Transform   exitPoint;
    [SerializeField] public Transform  spawnPoint;

    public string KeyName { get; set; }

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
        Vector3 dirToExit   = (lastChunkExitPoint.position - lastChunkExitPoint.parent.position).normalized;
        Vector3 length      = transform.position - entryPoint.position;

        transform.position = lastChunkExitPoint.position + dirToExit * length.magnitude;

        transform.rotation = Quaternion.LookRotation(dirToExit);
    }
}

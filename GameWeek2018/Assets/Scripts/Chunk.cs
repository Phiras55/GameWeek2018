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

    public void InitAndPlace(Transform lastChunkExitPoint)
    {
        Vector3 dirToExit   = (lastChunkExitPoint.position - lastChunkExitPoint.parent.position);
        dirToExit.y = 0;
        dirToExit.x = 0;
        dirToExit   = dirToExit.normalized;

        Vector3 length      = transform.position - entryPoint.position;
        length.y    = 0;
        length.x    = 0;

        Vector3 lastChunkExitPointNoY = lastChunkExitPoint.position;
        lastChunkExitPointNoY.y = 0;

        transform.position = lastChunkExitPointNoY + dirToExit * length.magnitude;

        //transform.rotation = Quaternion.LookRotation(dirToExit);
    }
}

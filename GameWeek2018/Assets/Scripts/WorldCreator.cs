﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [Header("World Parameters")]
    [SerializeField] private int        chunkViewDistance;
    [SerializeField] private int        chunkRandomStartIndex;
    [SerializeField] private int        chunkRandomEndIndex;
    [SerializeField] private float      unloadDistance;
    [SerializeField] private float      loadDistance;
    [SerializeField] private float      moveWorldMaxDistance;

    [Header("Start Zone")]
    [SerializeField] private string     nameOfStartChunk;
    [SerializeField] private GameObject playerPrefab;

    private List<Chunk>                 loadedChunks;
    private Chunk                       startZone;
    private ObjectPooler                chunkManager;
    private ObjectPooler                obstacleManager;
    private GameObject                  player;
    private int                         currentChunkIndex;
    private Vector3                     direction;

	// Use this for initialization
	void Start ()
    {
        chunkManager        = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ObjectPooler>();
        obstacleManager     = GameObject.FindGameObjectWithTag("ObstacleManager").GetComponent<ObjectPooler>();
        loadedChunks        = new List<Chunk>();
        currentChunkIndex   = 0;

        startZone                       = chunkManager.GetPoolInstance(nameOfStartChunk).GetComponent<Chunk>();
        startZone.KeyName               = nameOfStartChunk;
        startZone.transform.position    = Vector3.zero;

        if (!startZone.gameObject.activeSelf)
            startZone.gameObject.SetActive(true);

        loadedChunks.Add(startZone);

        player = Instantiate(playerPrefab);
        player.transform.position = startZone.spawnPoint.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        direction = player.transform.position - loadedChunks[0].transform.position;
		if(direction.sqrMagnitude >= unloadDistance * unloadDistance)
        {
            UnloadChunk();
        }

        direction = player.transform.position - loadedChunks[loadedChunks.Count - 1].transform.position;
        if (direction.sqrMagnitude <= loadDistance * loadDistance)
        {
            GenerateChunk();
        }

        direction = Vector3.zero - player.transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude >= moveWorldMaxDistance * moveWorldMaxDistance)
        {
            MoveWorld();
        }
	}

    private void GenerateChunk()
    {
        Chunk generatedChunk;
        int randInt     = Random.Range(chunkRandomStartIndex, chunkRandomEndIndex + 1);
        string tempName = chunkManager.pool.poolList[randInt].name;

        generatedChunk = chunkManager.GetPoolInstance(tempName).GetComponent<Chunk>();

        generatedChunk.transform.position = Vector3.zero;
        generatedChunk.KeyName = tempName;
        generatedChunk.InitAndPlace(loadedChunks[loadedChunks.Count - 1].exitPoint.transform);

        if (!generatedChunk.gameObject.activeSelf)
            generatedChunk.gameObject.SetActive(true);

        loadedChunks.Add(generatedChunk);
    }

    private void UnloadChunk()
    {
        if(startZone.gameObject.activeSelf)
        {
            startZone.gameObject.SetActive(false);
            loadedChunks.RemoveAt(0);
        }
        else
        {
            Chunk currentChunk = loadedChunks[0];
            chunkManager.BackToPoolQueue(currentChunk.KeyName, currentChunk.gameObject);
            loadedChunks.RemoveAt(0);
        }
    }

    void MoveWorld()
    {
        Vector3 normalizedDirection = direction.normalized;
        Vector3 movementToTeleport = normalizedDirection * moveWorldMaxDistance;

        player.transform.position += movementToTeleport;

        foreach(Chunk c in loadedChunks)
        {
            c.transform.position += movementToTeleport;
        }
    }

    private void UnloadAll()
    {

    }

    private void RestartRun()
    {

    }
}

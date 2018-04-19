using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [Header("World Parameters")]
    [SerializeField] private int        chunkViewDistance;
    [SerializeField] private float      unloadDistance;
    [SerializeField] private float      loadDistance;
    [SerializeField] private float      moveWorldMaxDistance;

    [Header("Chunk Difficulty")]
    [SerializeField] private int        easyChunkStart;
    [SerializeField] private int        easyChunkEnd;
    [SerializeField] private int        mediumChunkStart;
    [SerializeField] private int        mediumChunkEnd;
    [SerializeField] private int        hardChunkStart;
    [SerializeField] private int        hardChunkEnd;

    [Header("Start Zone")]
    [SerializeField] private string     nameOfStartChunk;
    [SerializeField] private GameObject playerPrefab;

    [Header("Chunk Spawn Chance")]
    [SerializeField] private int[]      chunkTreshold;
    [SerializeField] private Vector3[]  chunkSpawnChance;

    private List<Chunk>                 loadedChunks;
    private Chunk                       startZone;
    private ObjectPooler                chunkManager;
    private ObjectPooler                obstacleManager;
    private GameObject                  player;
    private int                         currentChunkIndex;
    private Vector3                     direction;

    private float                       timeTest = 0;

	// Use this for initialization
	void Start ()
    {
        chunkManager        = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ObjectPooler>();
        obstacleManager     = GameObject.FindGameObjectWithTag("ObstacleManager").GetComponent<ObjectPooler>();
        loadedChunks        = new List<Chunk>();

        startZone                       = chunkManager.GetPoolInstance(nameOfStartChunk).GetComponent<Chunk>();
        startZone.KeyName               = nameOfStartChunk;
        startZone.transform.position    = Vector3.zero;

        player = Instantiate(playerPrefab);
        //player.dieEvent.AddListener(RestartWorld);


        InitializeWorld();
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
        int randInt     = GetRandomChunkIndexFromCurrentChunk();
        string tempName = chunkManager.pool.poolList[randInt].name;

        generatedChunk = chunkManager.GetPoolInstance(tempName).GetComponent<Chunk>();

        generatedChunk.transform.position   = Vector3.zero;
        generatedChunk.KeyName              = tempName;
        generatedChunk.InitAndPlace(loadedChunks[loadedChunks.Count - 1].exitPoint.transform);

        if (!generatedChunk.gameObject.activeSelf)
            generatedChunk.gameObject.SetActive(true);

        ++currentChunkIndex;
        loadedChunks.Add(generatedChunk);
    }

    private int GetRandomChunkIndexFromCurrentChunk()
    {
        int minIndex = 0, maxIndex = 0, chunkTresholdIndex = -1;

        for(int index = 0; index < chunkTreshold.Length; ++index)
        {
            if (currentChunkIndex >= chunkTreshold[index])
                ++chunkTresholdIndex;

            else
                break;
        }
        
        float   rolledChance                    = 0;
        float[] currentSortedChunkSpawnChance   = GetSortedVector3(chunkSpawnChance[chunkTresholdIndex]);

        foreach(float v in currentSortedChunkSpawnChance)
        {
            rolledChance = UnityEngine.Random.Range(0f, 1f);
            if (v == 0)
                continue;
            else if(rolledChance <= v)
            {
                if(chunkSpawnChance[chunkTresholdIndex].y == v)
                {
                    minIndex = mediumChunkStart;
                    maxIndex = mediumChunkEnd;
                    break;
                }
                else if(chunkSpawnChance[chunkTresholdIndex].z == v)
                {
                    minIndex = hardChunkStart;
                    maxIndex = hardChunkEnd;
                    break;
                }
                else
                {
                    minIndex = easyChunkStart;
                    maxIndex = easyChunkEnd;
                    break;
                }
            }
        }

        return UnityEngine.Random.Range(minIndex, maxIndex + 1);
    }

    private float[] GetSortedVector3(Vector3 v)
    {
        float[] result = new float[] { v.x, v.y, v.z };
        Array.Sort(result);
        return result;
    }

    private void UnloadChunk(int id = 0)
    {
        if(startZone.gameObject.activeSelf)
        {
            startZone.gameObject.SetActive(false);
            loadedChunks.RemoveAt(0);
        }
        else
        {
            Chunk currentChunk = loadedChunks[id];
            chunkManager.BackToPoolQueue(currentChunk.KeyName, currentChunk.gameObject);
            loadedChunks.RemoveAt(id);
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

    private void UnloadAllChunks()
    {
        int chunkCount = loadedChunks.Count;
        for (int i = 0; i < chunkCount; ++i)
        {
            UnloadChunk();
        }
    }

    private void UnloadPlayer()
    {
        player.gameObject.SetActive(false);
        player.transform.position = Vector3.zero;
    }

    private void InitializeWorld()
    {
        currentChunkIndex = 0;

        startZone.transform.position = Vector3.zero;

        if (!startZone.gameObject.activeSelf)
            startZone.gameObject.SetActive(true);

        loadedChunks.Add(startZone);

        player.transform.position = startZone.spawnPoint.position;

        if (!player.activeSelf)
            player.SetActive(true);
    }

    private void RestartWorld()
    {
        UnloadAllChunks();
        UnloadPlayer();

        InitializeWorld();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [Header("World Parameters")]
    [SerializeField] private int    chunkViewDistance;
    [SerializeField] private int    chunkRandomStartIndex;
    [SerializeField] private int    chunkRandomEndIndex;

    [Header("Start Zone")]
    [SerializeField] private string nameOfStartChunk;

    private GameObject          player;
    private List<Chunk>         loadedChunks;
    private ObjectPooler        chunkManager;
    private ObjectPooler        obstacleManager;
    private int                 currentChunkIndex;

	// Use this for initialization
	void Start ()
    {
        chunkManager        = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ObjectPooler>();
        obstacleManager     = GameObject.FindGameObjectWithTag("ObstacleManager").GetComponent<ObjectPooler>();
        loadedChunks        = new List<Chunk>();
        currentChunkIndex   = 0;
        player              = GameObject.FindGameObjectWithTag("Player");

        Chunk startChunk                = chunkManager.GetPoolInstance(nameOfStartChunk).GetComponent<Chunk>();
        startChunk.KeyName              = nameOfStartChunk;
        startChunk.transform.position   = Vector3.zero;

        if (!startChunk.gameObject.activeSelf)
            startChunk.gameObject.SetActive(true);

        loadedChunks.Add(startChunk);

        //StartCoroutine(SpawnThings());
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //IEnumerator SpawnThings()
    //{
    //    while (true)
    //    {
    //        GenerateChunk();
    //        if(loadedChunks.Count > 3)
    //            UnloadChunk();
    //        yield return new WaitForSeconds(1);
    //    }
    //}

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
        Chunk currentChunk = loadedChunks[0];
        chunkManager.BackToPoolQueue(currentChunk.KeyName, currentChunk.gameObject);
        loadedChunks.RemoveAt(0);
    }
}

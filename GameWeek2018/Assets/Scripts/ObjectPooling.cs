using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private int            instantiationCount;
    [SerializeField] private List<Chunk>    chunkPrefabs;

    private Dictionary<int, Queue<Chunk>> chunkTypePooling;

	// Use this for initialization
	void Start ()
    {
        chunkTypePooling = new Dictionary<int, Queue<Chunk>>();
        FillPools();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void FillPools()
    {
        int index = 0;
        Chunk currentChunk;
        foreach(Chunk prefab in chunkPrefabs)
        {
            chunkTypePooling[index] = new Queue<Chunk>();
            for (int i = 0; i < instantiationCount; ++i)
            {
                currentChunk = Instantiate(prefab);
                currentChunk.gameObject.SetActive(false);
                currentChunk.transform.SetParent(gameObject.transform);
                chunkTypePooling[index].Enqueue(currentChunk);
            }
            ++index;
        }
    }
}

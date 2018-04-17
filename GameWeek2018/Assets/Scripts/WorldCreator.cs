using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [SerializeField] private int chunkDistance;
    private ObjectPooler ChunkManager;

	// Use this for initialization
	void Start ()
    {
        ChunkManager = GameObject.FindGameObjectWithTag("ChunkManager").GetComponent<ObjectPooler>();

        GameObject chunk1 = ChunkManager.GetPoolInstance("Roof_01");
        GameObject chunk2 = ChunkManager.GetPoolInstance("Roof_02");

        chunk1.transform.position = Vector3.zero;

        if (!chunk1.activeSelf)
            chunk1.SetActive(true);

        chunk2.gameObject.GetComponent<Chunk>().InitAndPlace(chunk1.GetComponent<Chunk>().exitPoint);

        if (!chunk2.activeSelf)
            chunk2.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

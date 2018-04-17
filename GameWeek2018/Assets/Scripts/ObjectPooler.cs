using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public PoolItem pool;

    private Dictionary<string, Pool>                    poolQueue = new Dictionary<string, Pool>();
    private Dictionary<string, LinkedList<GameObject>>  useQueue  = new Dictionary<string, LinkedList<GameObject>>();

    // Use this for initialization
    public void ResetPools() 
    {
        foreach(KeyValuePair<string, LinkedList<GameObject>> pair in useQueue)
        {
            var node = pair.Value.First;
            while (node != null)
            {
                var next = node.Next;
                BackToPoolQueue(pair.Key, node.Value);
                node = next;
            }
        }
    }

    void Awake ()
    {
        if (!pool)
        {
            Debug.LogError("pool Is Empty you MUST define the pool variable to in order to benefit from the object pooling feature");
            return;
        }

        if (poolQueue.Count <= 0)
        {
            foreach (Pool p in pool.poolList)
            {
                p.GetParent().transform.SetParent(transform);
                p.GetParent().transform.localPosition = Vector3.zero;

                poolQueue.Add(p.name, p);
                useQueue.Add(p.name, new LinkedList<GameObject>());
            }

            GeneratePool();
        }

        if(useQueue.Count >= 0)
        {

        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    /// <summary>
    /// Initializes the pools queue
    /// </summary>
    private void GeneratePool()
    {
        foreach(Pool p in pool.poolList)
        {
            if (!poolQueue.ContainsKey(p.name))
                continue;

            for(int i = 0; i < p.poolAmount; ++i)
            {
                GameObject go = Instantiate(p.prefab, transform.position, Quaternion.identity, p.GetParent().transform) as GameObject;
                poolQueue[p.name].GetQueue().Enqueue(go);

                if(go.activeSelf)
                    go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// returns a GameObject from the poolList with the "instanceName" that MUST be the exact same name as in the "Object Pooling" object
    /// </summary>
    /// <param name="instanceName"></param>
    /// <returns>something</returns>
    public GameObject GetPoolInstance(string instanceName)
    {
        if (poolQueue.Count <= 0)
            return null;

        GameObject go;

        if (poolQueue.ContainsKey(instanceName))
        {
            if (poolQueue[instanceName].GetQueue().Count > 0)
            {
                go = poolQueue[instanceName].GetQueue().Dequeue();

                useQueue[instanceName].AddLast(go);
                //go.SetActive(true);
            }
            else if (poolQueue[instanceName].canGrow)
            {
                Debug.Log("Not enough objects in queue.. creating new one");
                GameObject o = (GameObject)Instantiate(poolQueue[instanceName].prefab, transform.position, Quaternion.identity, poolQueue[instanceName].GetParent().transform);

                go = o;
                //poolQueue[instanceName].GetQueue().Enqueue(o);
                //o.SetActive(false);

                //go = poolQueue[instanceName].GetQueue().Dequeue();

                useQueue[instanceName].AddLast(go);
                //go.SetActive(true);
            }
            else go = null;
        }
        else go = null;

        if(go)
            go.SetActive(true);

        return go;
    }

    public void BackToPoolQueue(string key, GameObject o)
    {
        o.SetActive(false);
        useQueue[key].Remove(o);

        poolQueue[key].GetQueue().Enqueue(o);
        o.transform.position = transform.position;
    }
}

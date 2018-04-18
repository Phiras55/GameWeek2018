using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{

    public      string              name;
    public      GameObject          prefab;
    public      int                 poolAmount;
    public      bool                canGrow     =   false;
    private     Queue<GameObject>   queue       =   new Queue<GameObject>();

    private     GameObject          parent;

    public Queue<GameObject> GetQueue()
    {
        return queue;
    }

    public GameObject GetParent()
    {
        if(!parent)
            parent = new GameObject(name);

        return parent;
    }
}

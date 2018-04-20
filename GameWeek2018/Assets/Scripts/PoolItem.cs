using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object Pooling", menuName = "Object Pooling")]
public class PoolItem : ScriptableObject{
    public List<Pool> poolList;
}

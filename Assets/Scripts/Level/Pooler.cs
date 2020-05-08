using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Pooler : MonoBehaviour
{

    public static Pooler Instance;

    public List<Pool> pools = new List<Pool>();

    Dictionary<string, Pool> poolsByName = new Dictionary<string, Pool>();
    private void Awake()
    {

        if(Instance==null)
        Instance = this;

        foreach (var p in pools)
        {
            poolsByName.Add(p.PoolName, p);
            p.CreatePool();

        }
    }


    public GameObject GetPooledObject(string poolName)
    {

        if (poolsByName.ContainsKey(poolName))
        {
            GameObject go = poolsByName[poolName].GetPooledObject();   
            return go;
        }
        else
            return null;
    }
}

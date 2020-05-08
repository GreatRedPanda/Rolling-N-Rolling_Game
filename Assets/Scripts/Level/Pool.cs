using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class Pool 
{
    public string PoolName;

    public int PoolCount;
    public GameObject PooledPrefab;
    Queue<GameObject> pooledObjects = new Queue<GameObject>();
    public Transform PoolingObjectsParent;
    public void CreatePool()
    {
        for (int i = 0; i < PoolCount; i++)
        {
            GameObject go = GameObject.Instantiate(PooledPrefab);

            go.transform.SetParent(PoolingObjectsParent);
            go.SetActive(false);
            pooledObjects.Enqueue(go);
        }

    
    }
    public GameObject GetPooledObject()
    {
        GameObject go = pooledObjects.Dequeue();


        pooledObjects.Enqueue(go);
        return go;
    }
}
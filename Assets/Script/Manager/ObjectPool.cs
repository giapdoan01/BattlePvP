using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool(string tag, GameObject prefab, int size)
    {
        if (pools.ContainsKey(tag)) return;

        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        pools.Add(tag, pool);
    }

    public GameObject Get(string tag, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(tag)) return null;

        GameObject obj = pools[tag].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        pools[tag].Enqueue(obj);
        return obj;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // reset singleton khi bị Destroy để tránh giữ reference cũ
        if (Instance == this)
        {
            Instance = null;
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
        if (!pools.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject obj = pools[tag].Dequeue();
        if (obj == null)
        {
            Debug.LogWarning($"Object with tag {tag} was destroyed!");
            return null;
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        pools[tag].Enqueue(obj);
        return obj;
    }
}

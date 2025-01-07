using System;
using UnityEngine;

public abstract class CustomSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object Lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            var ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
                            _instance = ownerObject.AddComponent<T>();
                            DontDestroyOnLoad(ownerObject);
                        }
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}

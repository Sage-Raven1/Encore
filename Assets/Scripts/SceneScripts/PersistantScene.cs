using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantScene : MonoBehaviour
{
    public static PersistantScene Instance;
    //instance means only one of the thing

    [Header("Persistant Objects")]
    public GameObject[] persistantObjects;

    private void Awake()
    {
        if (Instance != null)
        {
        CleanUpAndDestroy();
        return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistantObjects();
        }
    }

    public void MarkPersistantObjects()
    {
        foreach (GameObject obj in persistantObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj)
            }
        }
    }
    
    public void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistantObjects)
        {
            CleanUpAndDestroy(obj);
        }
        Destroy(gameObject);
    }
}

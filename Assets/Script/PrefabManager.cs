using UnityEngine;
using System.Collections.Generic;
using System;

public class PrefabManager : MonoBehaviour
{
    
    void Start()
    {
        var asdf = Resources.LoadAll<GameObject>("Prefab/ToRead");
        Debug.Log(asdf.Length);

    }

    void Update()
    {
        
    }
}

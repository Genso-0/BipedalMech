﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public bool reloadScene;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(reloadScene)
        {
            reloadScene = false;
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }
}

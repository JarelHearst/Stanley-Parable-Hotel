using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScenes : MonoBehaviour
{
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            BGmusic.instance.GetComponent<AudioSource>().Pause(); // Pause the music
            DontDestroyOnLoad(BGmusic.instance); // Keep the music playing between scenes
        }
        else
        {
            BGmusic.instance.GetComponent<AudioSource>().Play();
        }
    }
}
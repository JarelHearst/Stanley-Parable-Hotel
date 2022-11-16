using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicControl : MonoBehaviour
{
    public static MusicControl instance;
    public AudioSource audioSource;
    bool play;
    void Start() {
        audioSource = GetComponent<AudioSource>();
        play = false;
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
}

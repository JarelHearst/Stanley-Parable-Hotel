using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject options;
    // Start is called before the first frame update
    void Start()
    {
        options.SetActive(false); // Hides the options menu
    }
}

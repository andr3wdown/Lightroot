using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    AudioSource ac;
    private void Start()
    {
        ac = GetComponent<AudioSource>();
    }
    void Update ()
    {
        if (!ac.isPlaying)
        {
            ac.gameObject.SetActive(false);
        }
	}
}

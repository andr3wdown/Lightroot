using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    bool ended = false;
    public Transform last;
    public GameObject endScreen;
    void Update()
    {
        if(last == null && !ended)
        {

            ended = true;
            StartCoroutine(End());
        }
    }
    IEnumerator End()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0;
        endScreen.SetActive(true);

    }
    void OnDisable()
    {
        Time.timeScale = 1;
    }
}

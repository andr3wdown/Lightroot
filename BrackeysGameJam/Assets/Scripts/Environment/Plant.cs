using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    Animator anim;
    static int counter = 6;
    bool visible = true;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10 || other.gameObject.layer == 11 || other.gameObject.layer == 14)
        {
            print(gameObject.name);
            anim.SetTrigger("Bob");
            if(counter % 6 == 0 && visible)
            {
                AudioController.PlayAudio(transform.position, "nature", 0.2f, Random.Range(1f, 1.1f));
            }
            counter++;
        }
    }
    private void OnBecameVisible()
    {
        visible = true;
    }
    private void OnBecameInvisible()
    {
        visible = false;
    }


}

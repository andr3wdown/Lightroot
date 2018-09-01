using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTroughPlatform : MonoBehaviour
{
    Collider col;

    private void Start()
    {
        col = transform.parent.GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            if(other.GetComponent<Rigidbody>().velocity.y > 0)
            {
                col.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            col.enabled = true;
        }
    }

}

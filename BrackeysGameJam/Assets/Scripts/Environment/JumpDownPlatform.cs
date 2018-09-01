using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownPlatform : MonoBehaviour
{
    Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.gameObject.layer == 10)
        {
            if (Input.GetAxis("Vertical") < -0.5f)
            {
                col.enabled = false;
            }
        }
    }
}

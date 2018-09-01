using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            other.GetComponent<CharacterController>().inLight = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            other.GetComponent<CharacterController>().inLight = false;
        }
    }

}

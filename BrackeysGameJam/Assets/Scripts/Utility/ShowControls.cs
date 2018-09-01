using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowControls : MonoBehaviour
{
    public GameObject controls;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            controls.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            controls.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    public Image image;
    public Transform indicatorSpot;
    protected bool inArea = false;
    public virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            image.rectTransform.position = Camera.main.WorldToScreenPoint(indicatorSpot.position);
            image.gameObject.SetActive(true);
            inArea = true;
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            image.gameObject.SetActive(false);
            inArea = false;
        }
    }
}

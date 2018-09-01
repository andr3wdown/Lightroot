using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject drop;
    public int dropAmount;
    bool activated = false;
    Animator anim;
    public float upwardsForce = 3f;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if(inArea && Input.GetKeyDown(KeyCode.F) && !activated)
        {
            AudioController.PlayAudio(Camera.main.transform.position, "ui");
            AudioController.PlayAudio(transform.position, "chest");
            anim.SetBool("Open", true);
            activated = true;
            Launch();
        }
    }
    void Launch()
    {
        for(int i = 0; i < dropAmount; i++)
        {
            GameObject go = Instantiate(drop, indicatorSpot.position, Quaternion.Euler(90, 0, 0));
            go.GetComponent<Rigidbody>().AddForce((Vector3.up * Random.Range(upwardsForce/1.5f, upwardsForce)) + (Vector3.right * Random.Range(-1.2f, 1.2f)), ForceMode.Impulse);
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            image.rectTransform.position = Camera.main.WorldToScreenPoint(indicatorSpot.position);
            image.gameObject.SetActive(!activated);
            inArea = true;
        }
    }
}

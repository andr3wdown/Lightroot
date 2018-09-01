using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    bool active = false;
    Cooldown dropActivationCooldown = new Cooldown(1f, true);
    Collider col;
    Rigidbody rb;
    Transform target;
    public float speed = 8f;
    TrailRenderer trail;
    private void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        target = FindObjectOfType<CharacterController>().transform;
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            active = true;
        }
    }
    private void Update()
    {
        if (active)
        {
            dropActivationCooldown.CountDown();
            if (dropActivationCooldown.TriggerReady(false))
            {
                col.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                gameObject.layer = 12;
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            trail.transform.parent = null;
            trail.autodestruct = true;
            Stats.AddOrbs();
            Stats.LightLevels(5f);
            AudioController.PlayAudio(transform.position, "pickup");
            Destroy(gameObject);
            //TODO: ADD MONEY!!
        }
           
    }
    

}

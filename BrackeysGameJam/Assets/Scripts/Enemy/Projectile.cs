using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Hitbox box;
    public float force;
    Rigidbody rb;
    bool right = false;
    bool init = false;
    public void Init(bool r)
    {
        right = r;
        init = true;
    }
    private void Start()
    {
        box = GetComponent<Hitbox>();
        box.enabled = false;
        rb = GetComponent<Rigidbody>();
    }
    private void Update ()
    {
        
        if (init)
        {
            box.enabled = true;
            transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            rb.velocity = (!right ? Vector3.right : Vector3.left) * force;
        }
        
	}
}

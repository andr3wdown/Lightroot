using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class Enemy : MonoBehaviour
{
    public float hp;
    protected Rigidbody rb;
    protected Animator anim;
    protected bool dead = false;
    public int dropAmount;
    public GameObject drop;
    public float upwardsForce;
    public Transform indicatorSpot;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void Launch()
    {
        for (int i = 0; i < dropAmount; i++)
        {
            GameObject go = Instantiate(drop, indicatorSpot.position, Quaternion.Euler(90, 0, 0));
            go.GetComponent<Rigidbody>().AddForce((Vector3.up * Random.Range(upwardsForce / 1.5f, upwardsForce)) + (Vector3.right * Random.Range(-1.2f, 1.2f)), ForceMode.Impulse);
        }
    }
    public virtual void Death()
    {
        Launch();
        Destroy(gameObject);
    }
    public virtual void Damage(float dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            hp = 0;
            Death();
        }
    }
}

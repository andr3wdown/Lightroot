using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage;
    public bool player;
    public int hittable;
    public bool isProjectile = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == hittable)
        {
            AudioController.PlayAudio(transform.position, "hit", 2f, 0.6f);
            if (player)
            {
                if(other.GetComponent<Enemy>() == null)
                {
                    other.transform.root.GetComponent<Enemy>().Damage(damage);
                }
                else
                {
                    other.GetComponent<Enemy>().Damage(damage);
                    other.GetComponent<Patroller>().target = FindObjectOfType<CharacterController>().transform;
                    other.GetComponent<Patroller>().idling = false;
                    other.GetComponent<Patroller>().idleToggleCooldown.Set(10f);
                }
               
            }
            else
            {
                Stats.HpManipulation(damage);
                if (isProjectile)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}

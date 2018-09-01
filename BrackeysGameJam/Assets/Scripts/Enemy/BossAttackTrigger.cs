using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackTrigger : MonoBehaviour
{
    Cooldown bossAttackCooldown = new Cooldown(3f, true);
    Patroller p;
	void Start()
    {
        p = transform.root.GetComponent<Patroller>();
    }
	void Update ()
    {
        bossAttackCooldown.CountDown();  
	}
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            p.Attack("Attack2", false);
            if (bossAttackCooldown.TriggerReady(transformingTrigger: Random.Range(1f, 3f)))
            {
            
            }
        }
    }

}

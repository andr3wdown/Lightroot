using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller : Enemy
{
    public float speed;
    public float patrolSpeed;
    public float chaseSpeed;
    public float wallCheckDistance;
    public Vector2[] groundChecks;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public float visionDistance;
    public float attackDistance = 3f;
    public float attackDelay;
    public float oppositeTreshold = 2f;
    public float attackPitch = 1f;
    public float attackVolume = 1f;
    
    bool right = false;
    bool running = false;
    bool attacking = false;
    [HideInInspector]
    public bool idling = false;
    bool runOpposite = false;
    bool seesPlayer = false;

    Cooldown stopWaitCooldown = new Cooldown(1f, true);
    [HideInInspector]
    public Cooldown idleToggleCooldown = new Cooldown(8f, true);
    Cooldown idleCooldown = new Cooldown(8f, true);
    Cooldown runningCooldown = new Cooldown();
    
    [HideInInspector]
    public Transform target;
    Vector3 targetPosition;
    public override void Start()
    {
        base.Start();
        right = (Random.Range(0, 2) == 0 ? true : false);
        idleCooldown.Set(Random.Range(6f, 12f));
        idleToggleCooldown.Set(Random.Range(6f, 12f));
    }
    void Patrol()
    {
        rb.velocity = Vector3.right * (right ? 1f : -1f) * speed;
        anim.SetBool("Walking", true);
        bool seesWall = Physics.Raycast(transform.position, (right ? Vector3.right : Vector3.left), wallCheckDistance, groundLayer);
        Collider[] ground = Physics.OverlapSphere(transform.position + (Vector3)(right ? groundChecks[0] : groundChecks[1]), 0.1f, groundLayer);
        bool groundLost = ground.Length < 1;
        if (seesWall || groundLost)
        {
            right = !right;
        }
        transform.localScale = new Vector3(right ? -1 : 1, 1, 1);
    }
    private void FixedUpdate()
    {
        Vision();
        if (target == null)
        {           
            if (!idling)
            {
                Patrol();
            }         
        }
        else
        {
            Chase();
        }
        if (attacking || idling || dead)
        {
            rb.velocity = Vector3.zero;
        }
    }
    private void Update()
    {
        speed = target != null ? chaseSpeed : patrolSpeed;
        anim.SetFloat("SpeedRatio", Mathf.Abs(rb.velocity.x) / chaseSpeed);
        IdleCountDown();
        
    }
    void IdleCountDown()
    {
        if (target == null || idling)
        {
            if (idling)
            {
                if(target != null)
                {
                    Vector3 dir = target.position - transform.position;
                    transform.localScale= new Vector3(dir.x > 0 ? -1 : 1, 1, 1);
                }
                anim.SetBool("Walking", false);
                idleCooldown.CountDown();
                if (idleCooldown.TriggerReady(transformingTrigger: target == null ? Random.Range(6f, 12f) : Random.Range(1f, 3f)))
                {
                    idling = false;
                }
            }
            else
            {
                idleToggleCooldown.CountDown();
                if (idleToggleCooldown.TriggerReady(transformingTrigger: Random.Range(6f, 12f)))
                {
                    idling = true;
                }
            }
        }
    }
    void Vision()
    {
        RaycastHit hit;
        seesPlayer = Physics.Raycast(transform.position, transform.localScale.x < 0 ? Vector3.right : Vector3.left, out hit, visionDistance, playerLayer);
        if (seesPlayer)
        {
            if (target == null)
            {
                idling = false;
                idleToggleCooldown.Set(10f);
            }
            target = hit.transform;        
        }
    }
    void Chase()
    {
        /*if(runOpposite)
        {

        }*/
        if (idling || attacking)
        {
            return;
        }
        if (!running)
        {      

            if(Vector3.Distance(transform.position, target.position) < attackDistance)
            {
                stopWaitCooldown.CountDown();
                if (stopWaitCooldown.TriggerReady())
                {
                    Attack();
                }
                return;
            }
           
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            anim.SetBool("Walking", true);
            rb.velocity = dir * speed;
            transform.localScale = new Vector3(rb.velocity.x < 0 ? 1 : -1, 1, 1);
            bool seesWall = Physics.Raycast(transform.position, dir, wallCheckDistance, groundLayer);
            Collider[] ground = Physics.OverlapSphere((Vector2)transform.position + ( dir.x > 0 ? groundChecks[0] : groundChecks[1]), 0.1f, groundLayer);
            bool groundLost = ground.Length < 1;

            if (seesWall  || groundLost)
            {
                rb.velocity = Vector3.zero;
                target = null;
                idling = true;
                idleCooldown.Set(Random.Range(1f, 2f));
                return;
            }

        }
        else
        {
            Vector3 dir = transform.position - target.position;
            dir.y = 0;
            dir.Normalize();
            anim.SetBool("Walking", true);
            rb.velocity = dir * speed;
            transform.localScale = new Vector3(rb.velocity.x < 0 ? 1 : -1, 1, 1);

            RaycastHit hit;
            bool seesWall = Physics.Raycast(transform.position, dir, out hit, wallCheckDistance, groundLayer);
            /*if(Vector3.Distance(transform.position, hit.point) < oppositeTreshold && Vector3.Distance(transform.position, target.position) < oppositeTreshold)
            {
                runOpposite = true;
                return;
            }*/
            Collider[] ground = Physics.OverlapSphere((Vector2)transform.position + (dir.x > 0 ? groundChecks[0] : groundChecks[1]), 0.1f, groundLayer);
            bool groundLost = ground.Length < 1;
            if (seesWall || groundLost)
            {

                rb.velocity = Vector3.zero;
                idling = true;
                idleCooldown.Set(1.5f);
                running = false;
                return;
            }
        }

    }
    public void Attack(string trigger="Attack", bool turn = true)
    {
        if (!attacking)
        {
            anim.SetTrigger(trigger);
            if (turn)
            {
                transform.localScale = new Vector3((target.position - transform.position).x < 0 ? 1 : -1, 1, 1);
            }
            
            attacking = true;
            StartCoroutine(AttackSequence(attackDelay));
        }      
    }
    public void PlayAttackSound(string sound)
    {
        AudioController.PlayAudio(transform.position, sound, pitch: attackPitch, volume: attackVolume);
    }
    IEnumerator AttackSequence(float delay)
    { 
        yield return new WaitForSeconds(delay);
        attacking = false;
        running = true;
    }
    Vector3 GetNextPoint()
    {
        return Vector3.zero;
    }
}

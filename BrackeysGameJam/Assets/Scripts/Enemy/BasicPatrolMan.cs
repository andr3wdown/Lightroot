using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPatrolMan : Enemy
{
    [Header("BasicProperties")]
    public float speed;

    [Header("Patrol properties")]
    public float wallCheckDistance;
    public Vector2[] wallCheckPoints;
    public Vector2[] groundCheckPoints;
    public LayerMask groundLayer;

    public Vector2 patrolLenght;

    [Header("Vision Properties")]
    public float visionRange;
    public LayerMask playerLayer;

    [Header("CombatProperties")]
    Cooldown combatChaseCooldown = new Cooldown(3f, true);
    Cooldown idleCooldown = new Cooldown(4f, true);
    Cooldown idleToggleCooldown = new Cooldown(8f, true);
    public float attackingRange;
    public float chaseFluctuationMultiplier = 1f;

    bool right = false;
    EnemyState state = EnemyState.idle;
    EnemyState previousState = EnemyState.patrolling;
    Transform target;
    Vector3 origPos = Vector3.zero;
    Vector3 nextChasePos = Vector3.zero;
    public override void Start()
    {
        base.Start();
        origPos = transform.position;
    }
    private void Update()
    {
        if (!dead)
        {
            switch (state)
            {
                case EnemyState.idle:
                    anim.SetBool("Walking", false);
                    rb.velocity = Vector3.zero;
                    Vision();

                    idleCooldown.CountDown();
                    if (idleCooldown.TriggerReady())
                    {
                        state = previousState;
                    }
                    break;

                case EnemyState.patrolling:
                    anim.SetBool("Walking", true);
                    IdleCount();
                    Vision();
                    Turn();
                    Patrol();
                    Move();
                    break;

                case EnemyState.chasing:
                    anim.SetBool("Walking", true);
                    Vision();
                    IdleCount();
                    Chase();
                    Move();
                    break;

                case EnemyState.attacking:
                    anim.SetBool("Walking", false);
                    rb.velocity = Vector3.zero;
                    break;

                case EnemyState.knocked:
                    anim.SetBool("Walking", false);
                    break;
            }
        }
      
    }
    void IdleCount()
    {
        idleToggleCooldown.CountDown();
        if (idleToggleCooldown.TriggerReady(transformingTrigger: Random.Range(6f, 10f)))
        {
            previousState = state;
            state = EnemyState.idle;
        }
    }
    void Move()
    {
        rb.velocity = (transform.localScale.x > 0 ? -Vector3.right : Vector3.right) * speed;
    }
    void Vision()
    {
        RaycastHit hit;
        bool r = Physics.Raycast(transform.position, right ? Vector3.right : -Vector3.right, out hit, visionRange, playerLayer);
        if (r)
        {
            if(state != EnemyState.attacking && state != EnemyState.knocked)
            {
                state = EnemyState.chasing;
                target = hit.transform;
            }
            if(Vector3.Distance(transform.position, hit.point) < attackingRange && combatChaseCooldown.TriggerReady())
            {
                Attack();
            }
        }  
    }
    void Turn()
    {
        RaycastHit hit;
        bool r = Physics.Raycast(transform.position, right ? wallCheckPoints[1] : wallCheckPoints[0], out hit, wallCheckDistance, groundLayer);
        bool r2 = Physics.OverlapSphere(right ? groundCheckPoints[1] : groundCheckPoints[0], 0.1f, groundLayer).Length > 0;
        if (r || !r2)
        {
            right = !right;
            if(hit.point.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    void Patrol()
    {
        if (transform.position.x < origPos.x + patrolLenght.x)
        {
            right = true;
        }
        if(transform.position.x > origPos.x + patrolLenght.y)
        {
            right = false;
        }
    }
    void Chase()
    {
        combatChaseCooldown.CountDown();
        if (!combatChaseCooldown.TriggerReady(false))
        {
            if(Vector3.Distance(transform.position, nextChasePos) < 0.5f || nextChasePos == Vector3.zero)
            {
                Vector3 dir = transform.position - target.position;
                dir.y = 0;
                dir.z = 0;
                dir.Normalize();

                Collider[] ground = Physics.OverlapSphere(transform.position + (dir * chaseFluctuationMultiplier), 0.1f, groundLayer);

                nextChasePos = (ground.Length > 0 ? transform.position : target.position) + ((ground.Length > 0 ? -dir : -dir) * chaseFluctuationMultiplier);
                nextChasePos.y = transform.position.y;
                combatChaseCooldown.Zero();
            }
            FaceTowards(nextChasePos);
            //TODO: add movement!
        }
        else
        {
            if (Vector3.Distance(transform.position, nextChasePos) < 0.5f || nextChasePos == Vector3.zero)
            {
                Vector3 dir = transform.position - target.position;
                dir.y = 0;
                dir.z = 0;
                dir.Normalize();

                Collider[] ground = Physics.OverlapSphere(new Vector3(target.position.x + (dir.x * chaseFluctuationMultiplier), transform.position.y, transform.position.z), 0.1f, groundLayer);

                if(ground.Length > 0)
                {
                    state = EnemyState.idle;
                    nextChasePos = Vector3.zero;
                    return;
                }

                nextChasePos = new Vector3(target.position.x + (dir.x * chaseFluctuationMultiplier), transform.position.y, transform.position.z);
                nextChasePos.y = transform.position.y;
            }
            FaceTowards(nextChasePos);
            //TODO: add movement!

        }
    }
    void FaceTowards(Vector3 point)
    {
        if(point.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void Attack()
    {
        if(state != EnemyState.attacking)
        {
            state = EnemyState.attacking;
            FaceTowards(target.position);
            anim.SetTrigger("Attack");
            StartCoroutine(AttackEnd());
        }
    }
    IEnumerator AttackEnd(float delay = 1.467f)
    {
        yield return new WaitForSeconds(delay);
        state = EnemyState.chasing;
    }
    Vector3 GivePointInRelation(Vector3 vector)
    {
        return (origPos == Vector3.zero ? transform.position : origPos) + vector;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach(Vector2 v in groundCheckPoints)
        {
            Gizmos.DrawSphere(GivePointInRelation(v), 0.2f);
        }
        foreach(Vector2 v in wallCheckPoints)
        {
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + v);
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GivePointInRelation(new Vector2(patrolLenght.x, 0)), 0.2f);
        Gizmos.DrawWireSphere(GivePointInRelation(new Vector2(patrolLenght.y, 0)), 0.2f);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, GivePointInRelation(Vector3.right * (right ? 1f : -1f) * visionRange));

        Color c = Color.cyan;
        c.a = 0.1f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, attackingRange);
    }

}

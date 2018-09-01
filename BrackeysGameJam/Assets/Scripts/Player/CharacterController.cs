using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour
{
    public float acceleration;
    public float speed;
    public float jumpForce;
    public float groundCheckDistance;
    public float groundCheckWidth;
    public ParticleSystem ps;
    public LayerMask groundLayer;
    public GameObject slashObject;
    public Transform slashPoint;

    public float[] attackCooldowns;
    public float[] attackSequenceBreakCooldowns;
    public Cooldown attackCooldown;

    public float[] speeds = { 1.5f, 2 };
    [HideInInspector]
    public float[] accelerations = { 300, 300 };

    public GameObject trailObject;
    public Transform trailGuide;
    public Cooldown jumpCooldown;
    public AudioClips audioClips;
    [HideInInspector]
    public bool inLight = false;
    private Cooldown trailCooldown = new Cooldown(0.05f);
    bool jumped = false;
    bool triggered = false;
    bool attacking = false;
    bool pressed = false;
    bool dead = false;
    bool spent = false;
    
    int attackIndex = 0;
    
    Animator anim;
    Rigidbody rb;
    private static CharacterController playerInstance;
    private void Start()
    {
        playerInstance = this;
        attackCooldown = new Cooldown(0.1f);
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
      
    }
    public static Vector3 playerPos
    {
        get
        {
            return playerInstance.transform.position;
        }
    }
    private void FixedUpdate()
    {
        if(!dead && !DialogueController.inDialogue)
        {
            Move();
        }
        else
        {
            Vector2 vel = rb.velocity;
            vel.x *= 0.90f;
            if (Mathf.Abs(vel.x) < 2f)
            {
                vel.x = 0;
            }
            rb.velocity = vel;
        }
    }
    private void Update()
    {
        if(!dead && !DialogueController.inDialogue)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = speeds[1];
                acceleration = accelerations[1];
            }
            else
            {
                speed = speeds[0];
                acceleration = accelerations[0];
            }

            Trail();
            Jump();

            if (!attacking)
            {
                if (InputVector.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (InputVector.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }


            anim.SetBool("IsGrounded", IsGrounded);
            anim.SetBool("Attacking", attacking);

            Attack();
            if (!attacking)
            {
                ResetAttack(ref attackIndex);
            }
            if (inLight)
            {
                Stats.LightLevels(Time.deltaTime * 15f);
            }
            else
            {
                //Stats.LightLevels(-Time.deltaTime * 1.5f);
            }
        }
        else
        {
            anim.SetBool("Walking", false);
            anim.SetBool("IsGrounded", true);
            anim.SetBool("Attacking", false);
          
        }
      
    }
    int counter = 0;
    void Move()
    {
        bool moving = Mathf.Abs(InputVector.x) > 0.1f;
        if (attacking)
        {
            if (IsGrounded)
            {
                rb.velocity *= 0.5f;
            }
            else
            {
                rb.velocity *= 0.5f;
            }
            
            return;
        }
        anim.SetBool("Walking", moving);
        if (moving && IsGrounded)
        {
            counter++;
            if(counter % 7 == 0)
            {
                ps.Emit(1);
            }
            if(counter >= 600)
            {
                counter = 0;
            }                
        }
        else
        {
            ps.Stop(true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        }
        if (moving)
        {
            rb.AddForce(Vector3.right * InputVector.x * acceleration);
            if (Mathf.Abs(rb.velocity.x) > speed)
            {
                Vector2 vel = rb.velocity;
                vel.x = speed * Mathf.Sign(rb.velocity.x);
                rb.velocity = vel;

            }
            anim.SetFloat("SpeedRatio", Mathf.Abs(rb.velocity.x) / speeds[1] * InputVector.x);
        }
        else if (!moving /*&& IsGrounded*/)
        {
            Vector2 vel = rb.velocity;
            vel.x *= 0.90f;
            if (Mathf.Abs(vel.x) < 2f)
            {
                vel.x = 0;
            }
            rb.velocity = vel;
            //"Controller (XBOX 360 For Windows)"
        }
    }
    void Jump()
    {
        if (/*IsGrounded ||*/ !JumpInput)
        {

            jumped = false;

            jumpCooldown.Reset();
        }
        if (jumped)
        {
            jumpCooldown.CountDown();
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            pressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            pressed = false;
        }

        if (IsGrounded && !triggered)
        {
            triggered = true;
            spent = false;
            StopCoroutine(T());
            StartCoroutine(T(0.4f));
            CameraFollow.CameraShake();           
            AudioController.PlayAudio(transform.position, audioClips.landingSound, 0.5f, 0.9f);
        }
        else if (!IsGrounded)
        {
            triggered = false;
        }


        
        if(!IsGrounded && pressed)
        {
            if (jumpCooldown.LowerThan(jumpCooldown.delay / 2f) && Stats.lightLevel > 0 && JumpInput)
            {
                Stats.LightLevels((-30f - (inLight ? 15f : 0f)) * Time.deltaTime);
                spent = true;
            }
            else
            {
                spent = false;
            }
        }

        if (!IsGrounded && !pressed && rb.velocity.y > 0 || !IsGrounded && jumpCooldown.TriggerReady(false) && rb.velocity.y > 0 || !IsGrounded && !spent && jumpCooldown.LowerThan(jumpCooldown.delay / 2f) && rb.velocity.y > 0)
        {
            Vector3 v = rb.velocity;
            v.y = Mathf.MoveTowards(v.y, 0, 35f * Time.deltaTime);
            rb.velocity = v;
        }

        if (rb.velocity.y < 0 && !IsGrounded)
        {
            rb.AddForce(Vector3.down * 19.81f, ForceMode.Acceleration);
        }
        if (IsGrounded)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
        rb.useGravity = !jumped || jumpCooldown.TriggerReady(false);

        if (IsGrounded && JumpInput && !jumpCooldown.TriggerReady(false) && pressed)
        {
            if (!jumped)
            {
                AudioController.PlayAudio(transform.position, audioClips.jumpSound, 0.7f, 1.5f);
                jumped = true;
                anim.SetTrigger("Jump");
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
        else
        {
            if (jumped)
            {
                anim.ResetTrigger("Jump");
            }
        }


        /*if (attacking)
        {
            return;
        }*/


    }
    public void PlayFootstep()
    {
        if(IsGrounded)
            AudioController.PlayAudio(transform.position, "ground");
    }
    Vector2 InputVector
    {
        get
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
    bool JumpInput
    {
        get
        {
            return Input.GetKey(KeyCode.Space);
        }
    }
    bool AttackInput
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Mouse0);
        }
    }
    bool IsGrounded
    {
        get
        {
            RaycastHit hit;
            RaycastHit hit2;
            bool first = Physics.SphereCast(transform.position + (transform.right * groundCheckWidth), 0.1f, Vector3.down, out hit, groundCheckDistance, groundLayer);
            bool second = Physics.SphereCast(transform.position + (transform.right * -groundCheckWidth), 0.1f, Vector3.down, out hit2, groundCheckDistance, groundLayer);

            return first || second;
        }
    }

    IEnumerator AttackSequence()
    {
        float timer = 0;
        float target = attackSequenceBreakCooldowns[0];
        int lastindex = 0;
        while(timer < target)
        {
            timer += Time.deltaTime;
            if(lastindex != attackIndex)
            {
                target += attackSequenceBreakCooldowns[attackIndex];
                lastindex = attackIndex;
            }
            yield return null;
        }
        attacking = false;
    }
    void Attack()
    {
        attackCooldown.CountDown();
        if(attackCooldown.TriggerReady(transformingTrigger: attackCooldowns[attackIndex]) && AttackInput && Stats.lightLevel > 15f)
        {
            if (!attacking)
            {
                attacking = true;
                StartCoroutine(AttackSequence());
            }
            Stats.LightLevels(-15f);
            AudioController.PlayAudio(transform.position, audioClips.jumpSound, 1f, 0.9f);
            anim.SetTrigger("Attack");
            StopCoroutine(T());
            StartCoroutine(T());
            GameObject go = Instantiate(slashObject, slashPoint.position, Quaternion.identity);
            go.transform.localScale = transform.localScale;
            attackIndex++;
            if(attackIndex >= 2)
            {
                attackIndex = 0;
            }
       
        }
    }
    void ResetAttack(ref int index)
    {
        index = 0;
        attackCooldown.Zero();
    }
    void Trail()
    {
        if (Xor(!IsGrounded, trailActivation))
        {
            trailCooldown.CountDown();
            if (trailCooldown.TriggerReady())
            {
                GameObject go = Instantiate(trailObject, transform.position, trailGuide.rotation);
                go.transform.localScale = new Vector3(transform.localScale.x * go.transform.localScale.x, transform.localScale.y * go.transform.localScale.y, transform.localScale.z * go.transform.localScale.z);
            }           
        }       
        else
        {
            trailCooldown.Zero();
        }
    }
    IEnumerator T(float t=0.7f)
    {
        trailActivation = true;
        yield return new WaitForSeconds(0.7f);
        trailActivation = false;
    }
    bool trailActivation = false;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + (transform.right * groundCheckWidth), transform.position + (transform.right * groundCheckWidth) + (Vector3.down * groundCheckDistance));
        Gizmos.DrawLine(transform.position + (transform.right * -groundCheckWidth), transform.position + (transform.right * -groundCheckWidth) + (Vector3.down * groundCheckDistance));
    }
    [System.Serializable]
    public struct AudioClips
    {
        public AudioClip jumpSound;
        public AudioClip landingSound;
        public AudioClips(AudioClip j, AudioClip l)
        {
            jumpSound = j;
            landingSound = l;
        }
    }
    bool Xor(bool b1, bool b2)
    {
        if(b1 && b2)
        {
            return false;
        }
        if(!b1 && !b2)
        {
            return false;
        }
        return true;
    }
    private void OnDisable()
    {
        playerInstance = null;
    }
    public void Death()
    {
        dead = true;
        anim.SetBool("Dead", true);
        gameObject.layer = 0;
        Patroller[] enemys = FindObjectsOfType<Patroller>();
        StartCoroutine(EndScreen());
        foreach(Patroller e in enemys)
        {
            e.target = null;
        }
    }
    public GameObject endScreen;
    IEnumerator EndScreen()
    {
        yield return new WaitForSeconds(4f);
        endScreen.SetActive(true);
        Time.timeScale = 0;
    
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
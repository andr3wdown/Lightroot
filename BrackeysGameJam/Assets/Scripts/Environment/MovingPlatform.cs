using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Range(0f, 10f)]
    public float platformSpeed;
    public Vector2[] positions;
    int pointIndex = 0;
    public Cooldown stoppingTime;
    Vector3 startingPoint;
    private void Start()
    {
        startingPoint = transform.position;
        stoppingTime = new Cooldown(stoppingTime.delay, true);
    }
    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)startingPoint + positions[pointIndex], platformSpeed * Time.fixedDeltaTime);
        if((Vector2)transform.position == (Vector2)startingPoint + positions[pointIndex])
        {
            stoppingTime.CountDown();
            if (stoppingTime.TriggerReady())
            {
                pointIndex++;
                if(pointIndex >= positions.Length)
                {
                    pointIndex = 0;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Transform colTrans = collision.collider.transform;
        if (colTrans.GetComponent<CharacterController>() != null)
        {
            colTrans.parent = this.transform;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        Transform colTrans = collision.collider.transform;
        if (colTrans.GetComponent<CharacterController>() != null)
        {
            colTrans.parent = null;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(startingPoint != transform.position)
        {
            startingPoint = transform.position;
        }
        Color c = Color.magenta;
        c.a = 0.5f;
        Gizmos.color = c;
        for(int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawCube((Vector2)startingPoint + positions[i], Vector3.one * 0.2f);
        }
    }
}

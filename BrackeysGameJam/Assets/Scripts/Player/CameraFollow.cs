using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    public float cameraSpeed;
    public float cameraShakeResist;
    private Vector3 currentVelocity;
    static CameraFollow instance;
    Camera cam;
    private void Start()
    {    
        cam = Camera.main;
        instance = this;
        target = FindObjectOfType<CharacterController>().transform;
    }
    private void FixedUpdate()
    {
        if(target != null)
        {          
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), ref currentVelocity, cameraSpeed);
        }
    }
    public void Update()
    {
        if(cam.orthographicSize != 5f && !shaking)
        {
            ReturnRotation();
        }
    }
    void ReturnRotation()
    {
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, 5f, cameraShakeResist * Time.deltaTime);
    }
    public static void CameraShake(float min= 0.2f, float max = 0.3f, int amount = 1)
    {      
        if(instance.shaking == false)
        {
            instance.shaking = true;
            instance.StartCoroutine(instance.CameraShakeSequence(min, max, amount));
        }    
    }
    bool shaking = false;
    IEnumerator CameraShakeSequence(float min, float max, int amount)
    {       
        for(int i = 0; i < amount; i++)
        {
            float targ = Random.Range(4.8f, 4.85f);
            while (cam.orthographicSize != targ)
            {
                cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targ, cameraShakeResist * 2f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }         
            yield return new WaitForSeconds(Random.Range(min, max));
        }
        shaking = false;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

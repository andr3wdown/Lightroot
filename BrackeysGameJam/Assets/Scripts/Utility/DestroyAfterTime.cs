using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{

    public float time = 0.17f;
    public float speed;
	void Start ()
    {
        Destroy(gameObject, time);
	}
    private void Update()
    {
        transform.position += Vector3.right * transform.localScale.x * Time.deltaTime * speed;
    }

}

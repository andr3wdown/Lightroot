using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    new SpriteRenderer renderer;
    [Range(0.001f, 5f)]
    public float speed = 1f;
    private void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if(renderer != null)
        {

            Color c = renderer.color;
            if(c.a <= 0)
            {
                Destroy(gameObject);
            }
            c.a -= Time.deltaTime * speed;
            
            renderer.color = c;
        }
    }

}

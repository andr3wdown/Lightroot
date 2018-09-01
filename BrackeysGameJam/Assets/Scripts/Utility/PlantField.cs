using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantField : MonoBehaviour
{
    public GameObject[] plants;
    public float fieldWidth;
    public int plantAmount;
	void Start ()
    {
		for(int i = 0; i < plantAmount; i++)
        {
            Vector3 spot = new Vector3(Random.Range(-fieldWidth, fieldWidth), 0, 0);
            GameObject go = Instantiate(plants[Random.Range(0, plants.Length)], transform.position + spot, Quaternion.identity);
            float scale = Random.Range(0.5f, 1.2f);
            go.transform.localScale = new Vector3(scale, scale, 1);
        }
	}
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * fieldWidth));
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * -fieldWidth));
    }
}

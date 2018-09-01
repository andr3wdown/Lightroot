using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject projectile;
    public Transform shotParent;
    GameObject currentProjectile;
    public void InitiateShoot()
    {
        currentProjectile = Instantiate(projectile, shotParent.position, Quaternion.identity);
        currentProjectile.transform.parent = shotParent;
        currentProjectile.GetComponent<Rigidbody>().isKinematic = true;
        currentProjectile.GetComponent<Rigidbody>().useGravity = false;


    }
    public void LaunchShot()
    {
        bool right = transform.localScale.x > 0;
        currentProjectile.GetComponent<Rigidbody>().isKinematic = false;
        currentProjectile.transform.parent = null;
        currentProjectile.GetComponent<Projectile>().Init(right);

    }

}

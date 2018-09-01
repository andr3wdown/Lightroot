using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    bool isOpen = false;
    public Animator anim;
    private void Start()
    {
        anim.SetBool("Open", isOpen); 
    }
    private void Update()
    {
        anim.SetBool("Open", isOpen);
    }
    public void OpenDoor()
    {
        isOpen = true;
    }
    public void CloseDoor()
    {
        isOpen = false;
    }

}

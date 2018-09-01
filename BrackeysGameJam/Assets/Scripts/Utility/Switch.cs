using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable
{

    public DoorController door;
    Animator selfAnim;
    bool open = false;
    private void Start()
    {
        selfAnim = GetComponent<Animator>();
    }
    void Update ()
    {
        if (!open && inArea && Input.GetKeyDown(KeyCode.F))
        {
            AudioController.PlayAudio(transform.position, "lever");
            door.OpenDoor();
            selfAnim.SetBool("Open", true);
        }
	}

}

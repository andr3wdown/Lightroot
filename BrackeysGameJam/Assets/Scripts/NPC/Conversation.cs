using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversation : Interactable
{
    public Dialogue[] conversations;
    Animator anim;
    int conversationIndex = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (inArea && !DialogueController.inDialogue && Input.GetKeyDown(KeyCode.F))
        {
            if(CharacterController.playerPos.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            DialogueController.StartDialogue(conversations[conversationIndex]);
            conversationIndex++;
            if(conversationIndex > conversations.Length - 1)
            {
                conversationIndex = conversations.Length - 1;
            }
    
        }
        if(inArea && DialogueController.isDisplaying && DialogueController.IsTalking(conversations[conversationIndex].name))
        {
            anim.SetBool("Talking", true);
        }
        else
        {
            anim.SetBool("Talking", false);
        }
    }

}

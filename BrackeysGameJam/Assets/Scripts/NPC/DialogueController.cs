using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public Text dialogueBox;
    public Text nameTag;
    public GameObject next;
    private Queue<string> loadedDialogue = new Queue<string>();
    private Animator anim;
    private static DialogueController instance;
    bool skip = false;
    public static bool inDialogue = false;
    public static bool isDisplaying = false;
    private void Start()
    {       
        instance = this;
        anim = GetComponent<Animator>();
    }
    public static void StartDialogue(Dialogue dialogue)
    {
        inDialogue = true;
        AudioController.PlayAudio(Camera.main.transform.position, "ui");
        instance.anim.SetBool("Dialogue", true);
        instance.loadedDialogue.Clear();
        instance.nameTag.text = dialogue.name;
        for(int i = 0; i < dialogue.sentences.Length; i++)
        {
            instance.loadedDialogue.Enqueue(dialogue.sentences[i]);
        }
        instance.StartCoroutine(instance.DisplaySentence(instance.loadedDialogue.Dequeue()));
    }
    public static bool IsTalking(string name)
    {
        return name == instance.nameTag.text;
    }
    public void NextSentence()
    {
        
        if (isDisplaying)
        {         
            skip = true;
            return;
        }
        AudioController.PlayAudio(Camera.main.transform.position, "ui");
        if (loadedDialogue.Count < 1)
        {
            
            EndConversation();
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DisplaySentence(loadedDialogue.Dequeue()));
    }
    void EndConversation()
    {
        
        instance.anim.SetBool("Dialogue", false);
        StartCoroutine(Delay());
        print("conversation ended!");
    }
    IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        inDialogue = false;
    }
    IEnumerator DisplaySentence(string sentence)
    {
        next.SetActive(false);
        isDisplaying = true;
        dialogueBox.text = "";
        yield return new WaitForSeconds(0.45f);
        for(int i = 0; i < sentence.Length; i++)
        {
            if (skip)
            {
                dialogueBox.text = sentence;
                skip = false;
                break;
            }
            dialogueBox.text += sentence[i];
           
            yield return new WaitForFixedUpdate();
        }
        isDisplaying = false;
        yield return new WaitForSeconds(0.1f);
        next.SetActive(true);
    }
    
    private void Update()
    {
        if (inDialogue && Input.GetKeyDown(KeyCode.Space) || inDialogue && Input.GetKeyDown(KeyCode.F) || inDialogue && Input.GetKeyDown(KeyCode.Mouse0))
        {
            NextSentence();
        }
    }
    private void OnDisable()
    {
        isDisplaying = false;
        inDialogue = false;
    }
   
}

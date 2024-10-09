using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Dialogue : MonoBehaviour
{
    [SerializeField] GameObject dBox;
    [SerializeField] Text dText;
    [SerializeField] int textSpeed;

    public static Dialogue Instance {get; private set; }

    private DialogueAction dialogue;
    private int currentLine = 0; 
    private bool typeBool;

    public event Action StartDialogue;
    public event Action EndDialogue;

    bool finished;

    private void Awake(){
        Instance= this;
        dialogue = new DialogueAction();
    }
    public IEnumerator displayText(DialogueAction dialogue){
        finished = false;
        yield return new WaitForEndOfFrame();
        StartDialogue?.Invoke();
        this.dialogue = dialogue;
        dBox.SetActive(true);
        yield return StartCoroutine(typeTheText(dialogue.getLines()[0]));
    }

    public IEnumerator typeTheText(string lines){
        typeBool = true;
        dText.text = "";
        char[] arr = lines.ToCharArray();

        foreach (char letter in arr)
        {
            dText.text += letter;
            yield return new WaitForSeconds(1f / textSpeed);
        }
        typeBool = false;
        HandleUpdate();
    }
    public void HandleUpdate(){
         if (Input.GetKeyDown(KeyCode.E) && !typeBool){
             currentLine++;
             if (currentLine < this.dialogue.getLines().Count){
                StartCoroutine(typeTheText(this.dialogue.getLines()[currentLine]));
             }
             else{
                currentLine = 0;
                dBox.SetActive(false);
                finished = true;
                EndDialogue?.Invoke();
                
             }
        }
    }
}

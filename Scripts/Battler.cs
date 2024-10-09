using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Battler : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject symbol; 
    [SerializeField] GameObject vision;
    [SerializeField] DialogueAction dialogue; 
    [SerializeField] DialogueAction defaultDialogue;
    [SerializeField] Sprite down;
    [SerializeField] Sprite up;
    [SerializeField] Sprite right;
    [SerializeField] Sprite left;
    [SerializeField] string name;
    SpriteRenderer renderer;
    private Vector3 target;
   [SerializeField] BoolValue saveWin;
    bool battleNotStarted = false;
    [SerializeField] Sprite sprite;


    private void Awake(){
        vision.SetActive(!saveWin.RuntimeValue);
        Debug.Log(this.saveWin.RuntimeValue);
        renderer = GetComponent<SpriteRenderer>();
    }
    public IEnumerator trainerChallenge(PlayerControl player){
        symbol.SetActive(true);
        yield return new WaitForSeconds(1f);
        symbol.SetActive(false);
        

        StartCoroutine(Dialogue.Instance.displayText(dialogue));
        Dialogue.Instance.EndDialogue += startBattle; 
    }
    public void startBattle(){
        Brain.Instance.StartTrainerBattle(this);}

    public void trainerBeaten(){
        this.saveWin.RuntimeValue = true;
        vision.SetActive(false);
    }

    //If there is time, find a better solution
    public void Interact(){
        if (this.saveWin.RuntimeValue == true){
            StartCoroutine(Dialogue.Instance.displayText(dialogue));
            Dialogue.Instance.EndDialogue += startBattle; 
        }
        else{
            StartCoroutine(Dialogue.Instance.displayText(defaultDialogue));
        }
    }
    public void face(Vector3 playerFacingDirection){
        renderer.sprite = (playerFacingDirection.x == 1)? left :
                          (playerFacingDirection.x == -1)? right : 
                          (playerFacingDirection.y == 1)? down : up;
    }

    public string Name {
        get => name;
    }
    public Sprite Spr1te{
        get => sprite;
    }

}

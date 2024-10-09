using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents group of unchangable variables, same category as classes and interfaces 
/*public enum CurrentAction {WalkingAround, Battle, Speaking, Challenge}

public class Brain : MonoBehaviour
{
    [SerializeField] PlayerControl player; 
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCam;
    CurrentAction currentAction; //declaring enum
    keep commented out
    public void Awake(){
        dialogue = new Dialogue();
    } until here
    private void Start(){
        //playerC.wildAppeared += StartBattle;

        Dialogue.Instance.StartDialogue += dialogueStart;
        Dialogue.Instance.EndDialogue += dialogueEnd;

        player.trainerBattle += (Collider2D trainerCollider) => {
            Battler trainer = trainerCollider.GetComponentInParent<Battler>();
            StartCoroutine(trainer.trainerChallenge(player));
            currentAction = CurrentAction.Challenge;
        };
    }

    

    void dialogueStart(){
        currentAction = CurrentAction.Speaking;
    }
    void dialogueEnd(){
        if (currentAction == CurrentAction.Speaking){
            currentAction = CurrentAction.WalkingAround;
        }
    }

    void StartBattle(){
        //currentAction = CurrentAction.Battle;
       // battleSystem.gameObject.SetActive(true);
       //mainCam.gameObject.SetActive(false);
    }

    
    private void Update(){
        if (currentAction == CurrentAction.WalkingAround){
            player.HandleUpdate();
        }
        keep commented out
        else if (state == CurrentAction.Battle){
            playerC.HandleUpdate();
        } until here
        else if (currentAction == CurrentAction.Speaking){
            Dialogue.Instance.HandleUpdate();
        }
    }
}*/
//Notes for azan:
//Change void Update() to public void HandleUpdate(), makes it run only when we want it to

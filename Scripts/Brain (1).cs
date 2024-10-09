using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents group of unchangable variables, same category as classes and interfaces 
public enum CurrentAction {WalkingAround, Battle, Speaking, Challenge, Paused}

public class Brain : MonoBehaviour
{
    [SerializeField] PlayerControl player; 
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject pause;

    bool buttonClick;

  //  public Camera worldCamera { get; set; }
    CurrentAction currentAction; //declaring enum
    public static Brain Instance { get; private set; }
    Battler trainer;

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }

    private void Start(){
        player.wildAppeared += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        player.paused += PauseMenu;

        Dialogue.Instance.StartDialogue += dialogueStart;
        Dialogue.Instance.EndDialogue += dialogueEnd;

        player.trainerBattle += (Collider2D trainerCollider) => {
            Battler trainer = trainerCollider.GetComponentInParent<Battler>();
            StartCoroutine(trainer.trainerChallenge(player));
            currentAction = CurrentAction.Challenge;
        };
    }

    public void ButtonClicked(){
        this.buttonClick = false; 
    }
    void PauseMenu(){
        buttonClick =true;
        currentAction = CurrentAction.Paused;
    }
    void dialogueStart(){
        currentAction = CurrentAction.Speaking;
    }
    void dialogueEnd(){
        if (currentAction == CurrentAction.Speaking){
            currentAction = CurrentAction.WalkingAround;
        }
    }

    void StartBattle()
    {
        Debug.Log("In Start battle method");
        currentAction = CurrentAction.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = player.GetComponent<PokemonParty>();
        //var playerParty = this.party.runtimeValue;
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        //battleSystem.StartBattle();
        battleSystem.StartBattle(playerParty, wildPokemon);
    }
    public void StartTrainerBattle(Battler trainer)
    {
        //Debug.Log("In Start battle method");
        currentAction = CurrentAction.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        this.trainer = trainer;
        var playerParty = player.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();
        
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }
    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.trainerBeaten();
            trainer = null;
        }
        currentAction = CurrentAction.WalkingAround;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    
    private void Update(){
        if (currentAction == CurrentAction.WalkingAround){
            player.HandleUpdate();
        }
        else if (currentAction == CurrentAction.Paused){
            pause.SetActive(true);
            if (Input.GetKeyDown(KeyCode.P) || buttonClick == false){
                pause.SetActive(false);
                currentAction = CurrentAction.WalkingAround;
            }
        }
        else if (currentAction == CurrentAction.Battle || currentAction == CurrentAction.Battle){
            battleSystem.HandleUpdate();
        }
        else if (currentAction == CurrentAction.Speaking){
            Dialogue.Instance.HandleUpdate();
        }
    }
}
//Notes for azan:
//Change void Update() to public void HandleUpdate(), makes it run only when we want it to
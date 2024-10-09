using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour//, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    private Vector2 movement;
    private bool encounter; 
    private bool movingBool;

    public event Action wildAppeared;
    public event Action<Collider2D> trainerBattle;
    public event Action paused;

    public LayerMask wildPoke;
    public LayerMask trainer;
    public LayerMask collision;
    public LayerMask npcLayer;
    public LayerMask doorway;
    public float moveSpeed;
    public Animator anime;
    [SerializeField] CurrentVector initalPos;


    [SerializeField] StringValue workPls;  
    //Estentially a constructor
    //Unity doesn't like public PlayerControl()
    private void Awake(){
        anime = GetComponent<Animator>();
        transform.position = this.initalPos.initalValue;

    }

    private void Start(){
        //transform.position = this.initalPos.initalValue;
    }


    public void HandleUpdate(){
        //Movement
        //Inputs
        if(!movingBool){
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement.x != 0) { movement.y=0;} //No diagonal movement

            //Animation
            if (movement != Vector2.zero){
                anime.SetFloat("Horizontal", movement.x);
                anime.SetFloat("Vertical", movement.y);

                var target = transform.position;
                target.x += movement.x;
                target.y += movement.y; 

                if (walkable(target)){ StartCoroutine(move(target));}
            }
        }
        anime.SetBool("movingBool", movingBool);
        if (Input.GetKeyDown(KeyCode.E)){
            Interact();
        }
       if (Input.GetKeyDown(KeyCode.P)){
            this.initalPos.initalValue = transform.position;
            this.workPls.RuntimeValue = SceneManager.GetActiveScene().name;
            Paused();
        }
    }

    private void Paused(){
        anime.SetBool("movingBool", false);
        paused();
    }
    //Coroutines start the function, stop at yield statement, then resume from yield
    IEnumerator move(Vector3 target){
        movingBool = true;
        //Checks if there is a difference between current and target position
        while ((target - transform.position).sqrMagnitude > 0){
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        //sets current position to target position
        transform.position = target;

        movingBool = false;

        check();
        trainerCheck();
        sceneSwitch();
    }

    private bool walkable(Vector3 target){
        if (Physics2D.OverlapCircle(target, 0.2f, collision | npcLayer) !=null){
            return false;
        }
        return true;
    }
    
    private void check(){ //For wild pokemon, too lazy to rename
        Collider2D wildCollider = Physics2D.OverlapCircle(transform.position, 0.2f, wildPoke);
        if (wildCollider != null){
            int num = UnityEngine.Random.Range(1, 101);
            if (num <= 10){
                anime.SetBool("movingBool", false);
                wildAppeared(); //called again in GameController.cs
                //object reference error????
            }
        }
    }

    void Interact(){
        var facingDirection = new Vector3(anime.GetFloat("Horizontal"), anime.GetFloat("Vertical"));
        var interactPosition = transform.position + facingDirection;

        //Checks if the next tile that you are facing has a collider
        var isNPC = Physics2D.OverlapCircle(interactPosition, 0.3f, npcLayer);
        
        if (isNPC != null){
            isNPC.GetComponent<IInteractable>()?.face(facingDirection);
            isNPC.GetComponent<IInteractable>()?.Interact(); //? is null conditional operator
        }
    }

    private void trainerCheck(){
        Collider2D trainerCollider = Physics2D.OverlapCircle(transform.position, 0.2f, trainer);
        if (trainerCollider != null){
            Debug.Log("Trainer has left you on seen");
            anime.SetBool("movingBool", false);
            trainerBattle?.Invoke(trainerCollider);
        }
    }

    private void sceneSwitch(){
        Collider2D transitioner = Physics2D.OverlapCircle(transform.position, 0.2f, doorway);
        if (transitioner != null){
            transitioner.GetComponent<IScene>()?.LoadScene();
        }
    }

    public string Name {
        get => name;
    }
    public Sprite Sprite {
        get => sprite;
    }
}
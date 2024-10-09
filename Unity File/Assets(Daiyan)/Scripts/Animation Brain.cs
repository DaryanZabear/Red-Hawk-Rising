using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBrain : MonoBehaviour
{
    [SerializeField] List<Sprite> down;
    [SerializeField] List<Sprite> up;
    [SerializeField] List<Sprite> right;
    [SerializeField] List<Sprite> left;

    public float horizontal { get; set;}
    public float vertical {get; set;}
    public bool moveBool {get; set;}
    private bool moveBool2; 

    Animation walkDown; 
    Animation walkUp; 
    Animation walkRight; 
    Animation walkLeft; 

    Animation currentAnime;

    SpriteRenderer renderer;
    private void Start(){
        renderer = GetComponent<SpriteRenderer>();
        walkDown = new Animation (down, renderer, 15);
        walkUp = new Animation (up, renderer, 15);
        walkRight = new Animation (right, renderer, 15);
        walkLeft = new Animation (left, renderer, 15);

        currentAnime = walkDown; 

    }
    private void Update(){
        Animation beforeAnime = currentAnime; 

        currentAnime = (this.horizontal == 1) ? walkRight : 
                       (this.horizontal == -1) ? walkLeft : 
                       (this.vertical == 1) ? walkUp : walkDown; 
        
        if (currentAnime != beforeAnime || moveBool2 != moveBool){
            currentAnime.Start();
        }
        if (moveBool){
            currentAnime.HandleUpdate();
        }
        else{
            renderer.sprite = currentAnime.Frames[0];
        }
        moveBool2 = moveBool;

    }
}

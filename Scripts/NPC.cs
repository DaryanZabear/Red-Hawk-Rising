using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] DialogueAction dialogue; 
    SpriteRenderer renderer;
    [SerializeField] Sprite down;
    [SerializeField] Sprite up;
    [SerializeField] Sprite right;
    [SerializeField] Sprite left;

    public void Awake(){
        renderer = GetComponent<SpriteRenderer>();
    }
    public void Interact(){
        StartCoroutine(Dialogue.Instance.displayText(dialogue));
    }

    public void face(Vector3 playerFacingDirection){
        renderer.sprite = (playerFacingDirection.x == 1)? left :
                          (playerFacingDirection.x == -1)? right : 
                          (playerFacingDirection.y == 1)? down : up;
    }

    
}

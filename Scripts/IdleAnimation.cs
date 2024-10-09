using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimation
{
    private List<Sprite> allSprites; 
    SpriteRenderer renderer;
    private Sprite down;
    private Sprite up;
    private Sprite right;
    private Sprite left;

    public IdleAnimation(List<Sprite> allSprites){
       // renderer = GetComponent<SpriteRenderer>();
        this.allSprites = allSprites;

        this.down = allSprites[3];
        this.up = allSprites[1];
        this.right = allSprites[0];
        this.left = allSprites[2];
    }
    public void face(Vector3 playerFacingDirection){
        renderer.sprite = (playerFacingDirection.x == 1)? left :
                          (playerFacingDirection.x == -1)? right : 
                          (playerFacingDirection.y == 1)? down : up;
    }
}

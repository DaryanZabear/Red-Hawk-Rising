using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation
{
    SpriteRenderer renderer;
    List<Sprite> frames; 
    float frameRate;
    int currentFrame;
    float timer; 

    public Animation(List<Sprite> frames, SpriteRenderer renderer, float frameRate){
        this.frames = frames;
        this.renderer = renderer;
        this.frameRate = frameRate;
    }

    public void Start(){
        currentFrame = 0; 
        timer = 0f; 
        renderer.sprite = frames[0];
    }

    public void HandleUpdate(){
        timer += Time.deltaTime;
        if (timer > frameRate){
            currentFrame = (currentFrame + 1) % frames.Count;
            renderer.sprite = frames[currentFrame];
            timer -= frameRate; 
        }
    }
    public List<Sprite> Frames{
        get { return frames;}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Makes it appear on inspector

public class DialogueAction 
{
    [SerializeField] private List<string> lines;

    public List<string> getLines(){
        return this.lines;
    }
}

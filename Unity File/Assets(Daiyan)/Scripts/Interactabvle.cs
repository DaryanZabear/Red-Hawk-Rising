using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sort of connects classes together, creates an interface for them
//Personal note: https://learn.unity.com/tutorial/interfaces# 
public interface IInteractable
{
    void Interact();
    void face(Vector3 foo);
}

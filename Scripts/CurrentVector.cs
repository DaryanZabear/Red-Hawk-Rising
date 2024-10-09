using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CurrentVector : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2 initalValue;
    public Vector2 defaultValue; 

    public void OnAfterDeserialize(){
     //   initalValue = defaultValue;
    }
    public void OnBeforeSerialize(){}
    
}

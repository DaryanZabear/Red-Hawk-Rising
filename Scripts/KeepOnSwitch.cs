using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOnSwitch : MonoBehaviour
{
    void Start(){
        DontDestroyOnLoad(gameObject);        
    }
}

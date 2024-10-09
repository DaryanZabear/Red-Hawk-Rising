using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveBrain : MonoBehaviour
{
    public static GameSaveBrain gameSave;
    public List<ScriptableObject> objects = new List<ScriptableObject>();

    private void Awake(){
        if(gameSave == null){
            gameSave = this;
        }
        else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }


}

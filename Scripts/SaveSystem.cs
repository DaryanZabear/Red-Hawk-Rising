using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
//4:13
{
    public List<ScriptableObject> objects = new List <ScriptableObject>();
    public static SaveSystem gameSave;


    private void Awake(){
        if(gameSave == null){
            gameSave = this;
        }
        else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }
    
    public void ResetScriptables(){
        for (int i = 0; i < objects.Count; i++){
            string path = Application.persistentDataPath + string.Format("/{0}.obj", i);
            if (File.Exists(path)){
                File.Delete(path);
            }
        }
    }

    private void OnEnable(){
        LoadScriptables();
    }
    private void OnDisable(){
        SaveScriptables();
    }
    public void SaveScriptables(){
        for (int i = 0; i < objects.Count; i++){
            string path = Application.persistentDataPath + string.Format("/{0}.obj", i);
            FileStream file = File.Create(path);

            BinaryFormatter formatter = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);
            formatter.Serialize(file, json);
            file.Close();
        }
    }

    public void LoadScriptables(){
        for(int i =0; i<objects.Count; i++){
            string path = Application.persistentDataPath + string.Format("/{0}.obj", i);
            if (File.Exists(path)){
                FileStream file = File.Open(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                JsonUtility.FromJsonOverwrite((string) formatter.Deserialize(file), objects[i]);
                file.Close();
            }
        }
    }







    /*public static void save(string fileName, object saveData){
        if(!Directory.Exists(Apllication.persistentDataPath + "/saves")){
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);

        file.Close();
        return true;
    }

    public static object Load(string path){
        if (!File.Exists(path)){
            return null;
        }
        
        BinaryFormatter formatter = GetBinary();

        FileStream file = File.Open(path, FileMode.Open);

        try{
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch{
            Debug.Log("Didn't work AAAAAAAAA");
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter getBinary(){
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }*/
}
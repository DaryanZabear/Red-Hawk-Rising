using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScenes : MonoBehaviour, IScene
{
    [SerializeField] string nextScene;
    [SerializeField] Vector2 playerPosition; 
    [SerializeField] CurrentVector playerPrevPos;

    [SerializeField] GameObject fadeIn; 
    [SerializeField] GameObject fadeOut;

    [SerializeField] StringValue workPls;  
    [SerializeField] bool button;


    /*[SerializeField] Animator transition;
    [SerializeField] Image black;*/
    
    

    public static SwitchScenes Instance {get; private set; }

    private void Awake(){
        Instance = this;
        if (fadeOut != null && button == false){
            GameObject image = Instantiate(fadeOut, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(image, 1);
        }
    }
    public void LoadScene(){
        //this.party.runtimeValue = player.GetComponent<PokemonParty>();
        playerPrevPos.initalValue = playerPosition;
        //SceneManager.LoadScene(this.nextScene);
        StartCoroutine(animateLoad(this.nextScene));
    }

    public void ButtonLoad(){
        if (this.workPls.RuntimeValue != ""){
            StartCoroutine(animateLoad(this.workPls.RuntimeValue));
        }
        else{
            StartCoroutine(animateLoad("Main Hall"));
        }
    }

public void StraightLoad(){
    //this.party.runtimeValue = player.GetComponent<PokemonParty>();
        StartCoroutine(animateLoad("Main Menu"));
    }

    IEnumerator animateLoad(string sceneName){
        if (fadeIn != null){
            Instantiate(fadeIn, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSeconds(1f);
        AsyncOperation foo = SceneManager.LoadSceneAsync(sceneName);
        while(!foo.isDone){
            yield return null;
        }
    }
}

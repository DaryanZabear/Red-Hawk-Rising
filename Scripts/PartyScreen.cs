using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;

    /*private void Start(){
        int begin = pokemons[0].Level;
        int end = pokemons[pokemons.Count-1].Level;
        sortPartyByLevel(this.pokemons, begin, end);
    }

    static void sortPartyByLevel(List<Pokemon> pokemons, int begin, int end){
        if (end <= begin) return ;
        int pivot = partition(pokemons, begin, end);
        sortPartyByLevel(pokemons, begin, pivot-1);
        sortPartyByLevel(pokemons, pivot + 1, end);
    }

    static int partition(List<Pokemon> pokemons, int begin, int end){
        int pivot = end;

        int counter = begin;
        for (int i = begin; i < end; i++) {
            if (pokemons[i].Level < pokemons[pivot].Level) {
                int tempor = pokemons[counter].Level;
                pokemons[counter] = pokemons[i];
                pokemons[i].Level = tempor;
                counter++;
            }
        }
        int temp = pokemons[pivot].Level;
        pokemons[pivot] = pokemons[counter];
        pokemons[counter].Level = temp;

        return counter;
    }*/

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].SetData(pokemons[i]);
            }
            else 
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a Pokemon.";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else 
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
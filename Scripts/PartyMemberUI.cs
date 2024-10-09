using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image image;
    [SerializeField] Image hudBg;
    [SerializeField] Color originalColor;
    //[SerializeField] Sprite hudBorder;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl. " + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
        hpBar.setHP((float) pokemon.HP / pokemon.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            hudBg.color = Color.blue;
        }
        else 
        {
            hudBg.color = originalColor;
        }
    }
}
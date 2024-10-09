using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color psnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;
    [SerializeField] Color brnColor;
    [SerializeField] GameObject xpBar;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.setHP((float) pokemon.HP / pokemon.MaxHp);
        SetXp();

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else 
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            if (statusText.text.Equals("PSN"))
            {
                statusText.color = psnColor;
            }
            else if (statusText.text.Equals("BRN"))
            {
                statusText.color = brnColor;
            }
            else if (statusText.text.Equals("PAR"))
            {
                statusText.color = parColor;
            }
            else if (statusText.text.Equals("FRZ"))
            {
                statusText.color = frzColor;
            }
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl. " + _pokemon.Level;
    }

    public void SetXp()
    {
        if (xpBar == null)
        {
            return;
        }

        float setXpVals = GetXpValues();

        xpBar.transform.localScale = new Vector3(setXpVals, 1, 1);
    }
    public IEnumerator XpAnimate(bool reset = false)
    {
        if (xpBar == null)
        {
            yield break;
        }

        if (reset)
        {
            xpBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float setXpVals = GetXpValues();
        yield return xpBar.transform.DOScaleX(setXpVals, 1.5f).WaitForCompletion();
    }

    float GetXpValues()
    {
        int currLvlXp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLvlXp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float initXp = (float)(_pokemon.XP - currLvlXp)/(nextLvlXp - currLvlXp);
        return Mathf.Clamp01(initXp);
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
        
    }
}
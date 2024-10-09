using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;
    [SerializeField] List<Text> actionText;
    [SerializeField] List<Text> moveText;
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Color normText;
    [SerializeField] Color waterText;
    [SerializeField] Color fireText;
    [SerializeField] Color grassText;
    [SerializeField] Color elecText;
    [SerializeField] Color grdText;
    [SerializeField] Color fightText;
    [SerializeField] Color iceText;
    [SerializeField] Color rockText;
    [SerializeField] Color bugText;
    [SerializeField] Color psnText;
    [SerializeField] Color psyText;
    [SerializeField] Color ghstText;
    [SerializeField] Color dragText;
    [SerializeField] Color steelText;
    [SerializeField] Color flyText;
    [SerializeField] Color darkText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    //private bool typeBool;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
        yield return new WaitForSeconds(0.7f);
        //typeBool = false;
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionText.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionText[i].color = Color.blue;
            }
            else
            {
                actionText[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveText.Count; ++i)
        {
            if (i == selectedMove)
            {
                moveText[i].color = Color.blue;
            }
            else
            {
                moveText[i].color = Color.black;
            }
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else 
        {
            ppText.color = Color.black;
        }
        /*
        if (move.Base.Type.ToString().Equals("Normal"))
        {
            typeText.color = normText;
        }
        else if (typeText.text.Equals("Water"))
        {
            typeText.color = waterText;
        }
        else if (typeText.text.Equals("Fire"))
        {
            typeText.color = fireText;
        }
        else if (typeText.text.Equals("Grass"))
        {
            typeText.color = grassText;
        }
        else if (typeText.text.Equals("Electric"))
        {
            typeText.color = elecText;
        }
        else if (typeText.text.Equals("Ground"))
        {
            typeText.color = grdText;
        }
        else if (typeText.text.Equals("Fighting"))
        {
            typeText.color = fightText;
        }
        else if (typeText.text.Equals("Ice"))
        {
            typeText.color = iceText;
        }
        else if (typeText.text.Equals("Rock"))
        {
            typeText.color = rockText;
        }
        else if (typeText.text.Equals("Bug"))
        {
            typeText.color = bugText;
        }
        else if (typeText.text.Equals("Poison"))
        {
            typeText.color = psnText;
        }
        else if (typeText.text.Equals("Psychic"))
        {
            typeText.color = psyText;
        }
        else if (typeText.text.Equals("Ghost"))
        {
            typeText.color = ghstText;
        }
        else if (typeText.text.Equals("Dragon"))
        {
            typeText.color = dragText;
        }
        else if (typeText.text.Equals("Steel"))
        {
            typeText.color = steelText;
        }
        else if (typeText.text.Equals("Flying"))
        {
            typeText.color = flyText;
        }
        else
        {
            typeText.color = Color.black;
        }*/
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i < moves.Count)
            {
                moveText[i].text = moves[i].Base.Name;
            }
            else
            {
                moveText[i].text = "-";
            }
        }
    }

    public void UpdateChoiceBox(bool yes)
    { 
        if (yes)
        {
            yesText.color = Color.blue;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = Color.blue;
        }
    }
}

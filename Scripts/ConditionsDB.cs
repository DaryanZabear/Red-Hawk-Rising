using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static void Init()
    {
        foreach (var kvp in Conditions) //key value pair
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;
            condition.Id = conditionId;
        }
    }
    public static Dictionary <ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {

        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = " was poisoned!",
                //Lambda function
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took damage from the poison!");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = " was burned!",
                //Lambda function
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took damage from its burn!");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyze",
                StartMessage = " was paralyzed!",
                //Lambda function
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if ((UnityEngine.Random.Range(1, 4)) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed! It can't move!");
                        return false; //cannot perform move
                    }
                    else 
                    {
                        return true;
                    }
                    
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = " is frozen!",
                //Lambda function
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if ((Random.Range(1, 5)) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thawed the ice!");
                        return false; //cannot perform move
                    }
                    else 
                    {
                        return true;
                    }
                    
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Asleep",
                StartMessage = " was put to sleep!",
                //Lambda function
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusCount = Random.Range(1,4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {   
                    if (pokemon.StatusCount <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }
                    pokemon.StatusCount--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is fast asleep...");
                    return false;
                }
            }
        },

        //Volatile
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confused",
                StartMessage = " is confused!",
                //Lambda function
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusCount = Random.Range(1,5);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {   
                    if (pokemon.VolatileStatusCount <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} isn't confused anymore!");
                        return true;
                    }
                    pokemon.VolatileStatusCount--;
                    //50% chance of successful move
                    if (Random.Range(1, 3) == 1)
                    {
                        return true;
                    }

                    //Hurt itself
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused!");
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to confusion!");
                    return false;
                }
            }
        },
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] bool alwaysHits;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    //[SerializeField] int effectChance;
    [SerializeField] MoveTarget target;
    /*
    [SerializeField] double twoHitChance;
    [SerializeField] double threeHitChance;
    [SerializeField] double fourHitChance;
    [SerializeField] double fiveHitChance;*/

    public string Name {
        get { return name; }
    }
    public PokemonType Type {
        get { return type; }
    }
    public int Power {
        get { return power; }
    }
    public int Accuracy {
        get { return accuracy; }
    }
    public int PP {
        get { return pp; }
    }
    /*public bool IsSpecial {
        get {
            if (type == PokemonType.Fire || type == PokemonType.Water || type == PokemonType.Grass || type == PokemonType.Ice || type == PokemonType.Electric || type == PokemonType.Dragon)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }*/
    public int Priority {
        get { return priority; }
    }
    /*public double TwoHitChance {
        get { return twoHitChance; }
    }
    public double ThreeHitChance {
        get { return threeHitChance; }
    }
    public double FourHitChance {
        get { return fourHitChance; }
    }
    public double FiveHitChance {
        get { return FiveHitChance; }
    }*/
    public MoveCategory Category {
        get { return category; }
    }
    public MoveEffects Effects {
        get { return effects; }
    }
    /*public int EffectChance {
        get { return effectChance;}
    }*/
    public MoveTarget Target {
        get { return target; }
    }
    public bool AlwaysHits {
        get { return alwaysHits; }
    }
    public List<SecondaryEffects> SecondaryEffects {
        get { return secondaryEffects; }
    }
    
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}
public enum MoveTarget
{
    Foe, 
    Self
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoost> Boosts {
        get { return boosts; }
    }
    public ConditionID Status {
        get { return status; }
    }
    public ConditionID VolatileStatus {
        get { return volatileStatus; }
    }

}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;
    
    public int Chance {
        get { return chance; }
    }
    public MoveTarget Target {
        get { return target; }
    }

}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
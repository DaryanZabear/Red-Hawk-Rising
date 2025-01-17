using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int baseXp;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] int minLevel;
    [SerializeField] int maxLevel;

    public static int MaxNumOfMoves { get; set; } = 4;


    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public Sprite FrontSprite {
        get { return frontSprite; }
    }

    public Sprite BackSprite {
        get { return backSprite; }
    }

    public PokemonType Type1 {
        get { return type1; }
    }

    public PokemonType Type2 {
        get { return type2; }
    }

    public int MaxHp {
        get { return maxHp; }
    }

    public int Attack {
        get { return attack; }
    }

    public int Defense {
        get { return defense; }
    }

    public int SpAttack {
        get { return spAttack; }
    }

    public int SpDefense {
        get { return spDefense; }
    }

    public int Speed {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }

    public int BaseXp => baseXp;

    public GrowthRate GrowthRate => growthRate;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level)/5;
        }
        else if (growthRate == GrowthRate.Medium)
        {
            return level * level * level;;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level)/4;
        }
        return -1;
    }
    

}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base {
        get { return moveBase; }
    }
    public int Level {
        get { return level; }
    }
}

public enum PokemonType
{
    None,
    Normal,
    Water,
    Fire,
    Grass,
    Electric,
    Ground,
    Fighting,
    Ice,
    Rock,
    Bug,
    Poison,
    Psychic,
    Ghost,
    Flying,
    Steel,
    Dragon,
    Dark
}

public enum GrowthRate 
{
    Fast, Medium, Slow
}

public enum Stat
{
    Attack, 
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accuracy,
    Evasion
}
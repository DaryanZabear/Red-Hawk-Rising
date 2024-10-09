using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]

public class Pokemon
{
    //public PokemonBase Base { get; set; }
    //public int Level { get; set; }
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base {
        get {
            return _base;
        }
    }
    public int Level {
        get {
            return level;
        }
        set{
        }
    }
    public int XP { get; set; }
    public double HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; } 
    public Dictionary<Stat, int> StatBoosts { get; private set; } 
    public Condition Status { get;  set; }
    public int StatusCount { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusCount { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }
    public event Action OnStatusChanged;
    // was public Pokemon(PokemonBase pBase, int pLevel)
    public void Init()
    {
        //Base = pBase;
        //Level = pLevel;
        
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
                break;
        }

        XP = Base.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHp;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;

    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level)/100f) + 7);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level)/100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level)/100f) + 7);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level)/100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level)/100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level)/100f) + 10 + Level;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else 
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }
        return statVal;
    }

    IEnumerator Seconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, - 6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} stat rose!");
                Seconds(2);
            }
            else if (boost < 0)
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} stat fell!");
                Seconds(2);
            }
            //Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public bool LevelUp()
    {
        if (XP > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    public LearnableMove AddMoves()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }
    public void LearnMove(LearnableMove moveLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves)
        {
            return;
        }
        Moves.Add(new Move(moveLearn.Base));
    }

    public int Attack {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense {
        get { return GetStat(Stat.Defense); }
    }
    public int SpAttack {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed {
        get { return GetStat(Stat.Speed); }
    }
    /*public int MaxHp { 
        get { return Mathf.FloorToInt((Base.MaxHp * Level)/100f) + 10; }
    }*/
    public int MaxHp { get; private set; }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        double type = TypeEffect(move, this.Base.Type1) * TypeEffect(move, this.Base.Type2);
        var damageDetails = new DamageDetails()
        {
            Type = type,
            Fainted = false
        };

        int attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        Debug.Log($"{attacker.Base.Name} attack is {attack}");
        int defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;
        Debug.Log($"Defense is {defense}");
        double damage;
        if (move.Base.Power == 0)
        {
            damage = 0;
        }
        else 
        {
            damage =  (double) ((((((2*attacker.Level)/5)+2) * move.Base.Power * ((double)  attack / (double) defense))/50) + 2)*type;
        }
        /*
        HP -= (int)productDmg;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }*/
        UpdateHP(damage);
        Debug.Log($"{HP}/{MaxHp}");

        return damageDetails;
    }

    public void UpdateHP(double damage)
    {
        HP -= damage;
        HP = (HP < 0)? 0: HP;
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    { 
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        return canPerformMove;
    }
    // for volatile statuses like confusion
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        //only invokes if not null
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
    
    public double TypeEffect(Move move, PokemonType enemyType)
    {
        string mvType = move.Base.Type.ToString();
        string enemyType1 = enemyType.ToString();
        double multiplier = 1.0;

        // Normal
        if (mvType.Equals("Normal"))
        {
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 0.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
        }
        // Fighting
        else if (mvType.Equals("Fighting"))
        {
            if (enemyType1.Equals("Normal"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 0.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Psychic"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ice"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Dark"))
            {
                multiplier *= 2.0;
            }
        }
        // Flying
        else if (mvType.Equals("Flying"))
        {
            if (enemyType1.Equals("Fighting"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Electric"))
            {
                multiplier *= 0.5;
            }
        }
        // Poison
        else if (mvType.Equals("Poison"))
        {
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 2.0;
            }
        }
        // Ground
        else if (mvType.Equals("Ground"))
        {
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 0;
            }
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Electric"))
            {
                multiplier *= 2.0;
            }
        }
        // Rock
        else if (mvType.Equals("Rock"))
        {
            if (enemyType1.Equals("Fighting"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ice"))
            {
                multiplier *= 2.0;
            }
        }
        // Bug
        else if (mvType.Equals("Bug"))
        {
            if (enemyType1.Equals("Fighting"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Psychic"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Dark"))
            {
                multiplier *= 2.0;
            }
        }
        // Ghost
        else if (mvType.Equals("Ghost"))
        {
            if (enemyType1.Equals("Normal"))
            {
                multiplier *= 0;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Psychic"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Dark"))
            {
                multiplier *= 0.5;
            }
        }
        // Steel
        else if (mvType.Equals("Steel"))
        {
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Electric"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ice"))
            {
                multiplier *= 2.0;
            }
        }
        // Fire
        else if (mvType.Equals("Fire"))
        {
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ice"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 0.5;
            }
        }
        // Water
        else if (mvType.Equals("Water"))
        {
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 0.5;
            }
        }
        // Water
        else if (mvType.Equals("Grass"))
        {
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Rock"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Bug"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 0.5;
            }
        }
        // Electric
        else if (mvType.Equals("Electric"))
        {
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 0;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Electric"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 0.5;
            }
        }
        // Psychic
        else if (mvType.Equals("Psychic"))
        {
            if (enemyType1.Equals("Fighting"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Poison"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Psychic"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dark"))
            {
                multiplier *= 0;
            }
        }
        // Ice
        else if (mvType.Equals("Ice"))
        {
            if (enemyType1.Equals("Flying"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ground"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Fire"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Water"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Grass"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Ice"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 2.0;
            }
        }
        // Dragon
        else if (mvType.Equals("Dragon"))
        {
            if (enemyType1.Equals("Steel"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Dragon"))
            {
                multiplier *= 2.0;
            }
        }
        // Dark
        else if (mvType.Equals("Dark"))
        {
            if (enemyType1.Equals("Fighting"))
            {
                multiplier *= 0.5;
            }
            if (enemyType1.Equals("Ghost"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Psychic"))
            {
                multiplier *= 2.0;
            }
            if (enemyType1.Equals("Dark"))
            {
                multiplier *= 0.5;
            }
        }
        else
        {
            multiplier *= 1;
        }
        return multiplier;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public double Type { get; set; }
}
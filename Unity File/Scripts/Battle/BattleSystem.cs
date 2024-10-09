using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, AboutToUse, BattleOver }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    public BattleUnit PlayerUnit {
        get { return playerUnit; }
    }
    public BattleUnit EnemyUnit {
        get { return enemyUnit; }
    }
    [SerializeField] BattleHud playerHud; 
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerSprite;
    public event Action<bool> OnBattleOver;
    bool aboutToUse = true;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    public PokemonParty PlayerParty{
        get { return playerParty; }
    }
    PokemonParty trainerParty;
    public PokemonParty TrainerParty {
        get { return trainerParty;}
    }
    Pokemon wildPokemon;
    GameController gameControl;
    public GameController GameControl{
        get { return gameControl; }
    }
    Pokemon pokemon;
    public Pokemon Pokemon {
        get { return pokemon; }
    }

    bool isTrainerBattle = false;
    PlayerControl player;
    Battler trainer;
    
 
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon) //rename to StartBattle() vid 12
    { 
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerControl>();
        isTrainerBattle = false;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerControl>();
        trainer = trainerParty.GetComponent<Battler>();

        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        Debug.Log("Over here");
        playerUnit.Clear();
        enemyUnit.Clear();
        if (!isTrainerBattle)
        {
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
        }
        else
        {
            Debug.Log("checking");

            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(true);

            playerSprite.sprite = player.Sprite;

            yield return dialogBox.TypeDialog($"You are challenged by {trainer.Name}!");
            //trainer
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} sent out {enemyPokemon.Base.Name}!");
            //player
            playerSprite.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}, I choose you!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        
        partyScreen.Init();
        ActionSelection();
    }
    void ActionSelection()
    {
        Debug.Log("Action Selection");
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action (use arrow keys): ");
        dialogBox.EnableActionSelector(true);
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());

        for (int i =0; i < playerParty.Pokemons.Count; i++){
            Pokemon obj = playerParty.Pokemons[i];
            Debug.Log(obj.HP + " " );
            obj.HP = obj.MaxHp;
            Debug.Log(obj.HP + " " + obj.Base.MaxHp + " " + obj.MaxHp);
            obj.Status = null;

            for (int j = 0; j < obj.Moves.Count; j++){
                obj.Moves[j].PP = obj.Moves[j].Base.PP;
            }
            
        }

        OnBattleOver(won);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        Debug.Log("About to busy");
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to battle with {newPokemon.Base.Name}. Switch your Pokemon?");
        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.PerformMove;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }
            

            var firstUnit = (playerGoesFirst)? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst)? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;
            //1st turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) 
            {
                yield break;
            }
            //2nd turn
            if (secondPokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) 
                {
                    yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                Debug.Log("About to busy 2");
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            } 
            else if (playerAction == BattleAction.Run)
            {
                yield return Escape();
            }   
            // enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) 
            {
                yield break;
            }
        }

        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //state = BattleState.PerformMove;
        //var move = playerUnit.Pokemon.Moves[currentMove];
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
            if (!canRunMove)
            {
                yield return ShowStatusChanges(sourceUnit.Pokemon);
                yield return sourceUnit.Hud.UpdateHP();
                yield break;
            }

            yield return ShowStatusChanges(sourceUnit.Pokemon);
            move.PP--;

            if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}.");
                yield return new WaitForSeconds(1f);
                sourceUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(1f);
                targetUnit.PlayHitAnimation();

                if (move.Base.Category == MoveCategory.Status)
                {
                    var effects = move.Base.Effects;
                    if (effects.Boosts != null)
                    {
                        yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
                    }
                }
                else 
                {
                    
                    var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                    yield return targetUnit.Hud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);
                    yield return new WaitForSeconds(1f);
                }

                if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
                {
                    foreach (var secondary in move.Base.SecondaryEffects)
                    {
                        var rnd = UnityEngine.Random.Range(1, 101);
                        if (rnd <= secondary.Chance)
                        {
                            yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                        }
                    }
                }

                if (targetUnit.Pokemon.HP <= 0)
                {
                    yield return HandlePokemonFainted(targetUnit);
                }
            }
            else 
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} avoided the attack!");
            }
    }
    //Turn analysis
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
        {
            yield break;
        }
        yield return new WaitUntil(() => state == BattleState.PerformMove);
        // for damage-dealing status (burn, psn)
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.PerformMove);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        //var effects = move.Base.Effects;
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else 
            {
                target.ApplyBoosts(effects.Boosts);
                /*source.PlayAttackAnimation();
                yield return new WaitForSeconds(1f);
                target.PlayHitAnimation();*/
            }
        }
        
        //Status Conditions
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        //Volatile Status
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
            

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;
        
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = source.StatBoosts[Stat.Evasion];
        var boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f};
        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else 
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else 
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} fainted...");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            int xp = (int) playerUnit.Pokemon.Base.BaseXp / 20;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle)? 1.5f : 1f;
            Debug.Log("XP GAIN: " + enemyLevel + " trainer Bonus" + trainerBonus + " xp " + xp);
            int xpGain = Mathf.FloorToInt((xp * enemyLevel * trainerBonus) / 6);
            Debug.Log("XP GAIN: " + xpGain);
            playerUnit.Pokemon.XP += xpGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {xpGain} xp.");
            yield return playerUnit.Hud.XpAnimate();

            while (playerUnit.Pokemon.LevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to Level {playerUnit.Pokemon.Level}!");

                var newMove = playerUnit.Pokemon.AddMoves();
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}!");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {

                    }
                }

                yield return playerUnit.Hud.XpAnimate(true);
            }


        }
        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else 
            {
                BattleOver(false);
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else 
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    StartCoroutine(AboutToUse(nextPokemon));
                }
                else
                {
                    BattleOver(true);
                }
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails) 
    {
        if (damageDetails.Type > 1)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
        }
        else if (damageDetails.Type < 1)
        {
            yield return dialogBox.TypeDialog("It's not very effective...");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
    }

    void HandleActionSelection()
    {
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Bag
            }
            else if (currentAction == 2)
            {
                //Pokemon
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0)
            {
                return;
            }
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"Your {selectedMember} doesn't have HP.");
                return;
            }

            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText($"Your {selectedMember} is already out!");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                Debug.Log("About to busy 3");
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
            /*Debug.Log("About to busy 4");
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));*/
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("Choose a Pokemon to continue.");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
            {
                ActionSelection();
            }
        }
    }

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUse = !aboutToUse;
        }

        dialogBox.UpdateChoiceBox(aboutToUse);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUse == true)
            {
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Good job {playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.Setup(newPokemon);
        //playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Get 'em {newPokemon.Base.Name}!");
        yield return new WaitForSeconds(1f);

        if (prevState == null)
        {
            state = BattleState.PerformMove;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            Debug.Log("switch pokemon here");
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    IEnumerator SendNextTrainerPokemon()
    {
        Debug.Log("trainer sent out pokemon here");
        state = BattleState.Busy;
        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} sent out {nextPokemon.Base.Name}.");
        
        state = BattleState.PerformMove;
    }

    IEnumerator Escape()
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can only run when facing wild Pokemon.");
            state = BattleState.PerformMove;
            yield break;
        }

        int playerSpd = playerUnit.Pokemon.Speed;
        int enemySpd = enemyUnit.Pokemon.Speed;
        if (playerSpd > enemySpd)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            if (UnityEngine.Random.Range(0,101)<75)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Couldn't run away successfully!");
                state = BattleState.PerformMove;
            }
        }
    }
}
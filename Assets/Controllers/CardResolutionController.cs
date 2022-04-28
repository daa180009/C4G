using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the process of resolving the action of a player playing a card
/// </summary>
public class CardResolutionController : MonoBehaviour
{
    public WorldController worldController;
    public WorldInfo worldInfo;
    public TargetSelectionController targetSelectionController;
    public CardZone DiscardZone;
    public CardZone ResolutionZone;

    Action<CardController> cbResolutionFinished;
    /// <summary>
    /// Register a function to be called when this card's resolution is complete
    /// </summary>
    public void RegisterResolutionFinished(Action<CardController> cb) { cbResolutionFinished += cb; }

    Action<CardController> cbCardAdded;
    /// <summary>
    /// Register a function to be called when a new active card is set.
    /// </summary>
    public void RegisterCardAdded(Action<CardController> cb) { cbCardAdded += cb; }

    Action cbCardRemoved;
    /// <summary>
    /// Register a function to be called when the active card is removed, such as upon resolution
    /// </summary>
    public void RegisterCardRemoved(Action cb) { cbCardRemoved += cb; }

    Action<int, int, bool> cbTargetCountChanged;
    /// <summary>
    /// Register a function to be called when the number of targets changes, returning the number of targets now, max number of targets, and if the user can submit yet
    /// </summary>
    public void RegisterTargetCountChanged(Action<int, int, bool> cb) { cbTargetCountChanged += cb; }

    /// <summary>
    /// The card that the controller is attempting to resolve
    /// </summary>
    CardController activeCard = null;
    /// <summary>
    /// The index of the effect currently being resolved on the active card
    /// </summary>
    int activeEffectIndex;
    /// <summary>
    /// The information on what targets have been chosen so far for the current effect
    /// </summary>
    TargetInfo currentTargetInfo;
    /// <summary>
    /// Information on previous effects that resolved on this same card
    /// </summary>
    ResolutionInfo resolutionInfo;

    public bool isLeft;

    /// <summary>
    /// Public access to tell whether a card is currently resolving
    /// </summary>
    public bool IsBusy
    {
        get
        {
            return activeCard != null;
        }
    }

    void Start()
    {
        targetSelectionController.RegisterSelectTileCB(OnSelectTile);
        targetSelectionController.RegisterSelectTowerCB(OnSelectTower);
        targetSelectionController.RegisterSelectEnemyCB(OnSelectEnemy);
    }
    
    /// <summary>
    /// Request for a card to be played.
    /// </summary>
    public bool PlayCard(CardController cardController)
    {
        if (!IsBusy)
        {
            isLeft = false;
            if (cbCardAdded != null)
                cbCardAdded(cardController);
            setActiveCard(cardController);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Request for the leftmost card to be played.
    /// </summary>
    public bool PlayLeft(CardController cardController)
    {
        if (!IsBusy)
        {
            isLeft = true;
            if (cbCardAdded != null)
                cbCardAdded(cardController);
            setActiveCard(cardController);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool checkedCondition;
    /// <summary>
    /// Try to resolve as much as a card as possible, usually until having to choose targets
    /// </summary>
    private void attemptResolution()
    {
        if (activeCard == null)
            return;

        if (activeCard.Data.Type == Card.CardType.Tower)
        {
            // resolving a tower is pretty different so use different function
            resolveTower();
            return;
        }

        if (activeCard.Data.hasLeftBonus && isLeft)
        {
            attemptResolutionLeft();
            return;
        }

        if (activeEffectIndex >= activeCard.Data.CardEffects.Count)
        {
            // have gone past last effect, so end resolution
            removeActiveCard();
            return;
        }

        // since we know the active effect is valid now, just get it by index for the rest of the method
        CardEffect activeEffect = activeCard.Data.CardEffects[activeEffectIndex];

        if (!checkedCondition)
        {
            // just got to this effect, so check if it is true
            if (activeEffect.condition.CheckCondition(worldInfo, resolutionInfo))
            {
                // is true, so skip this later
                checkedCondition = true;
                // set up new target info
                currentTargetInfo = new TargetInfo();
            }
            else
            {
                //not true, so skip this entire effect
                activeEffectIndex += 1;
                attemptResolution();
                return;
            }
        }

        int targetCount = currentTargetInfo.GetTargetCount(activeEffect.predicate.TargetType);
        if (targetCount < activeEffect.maxTargets && !currentTargetInfo.StoppedBeforeMax)
        {
            // not enough targets selected

            bool canSubmit = targetCount >= activeEffect.minTargets;
            targetSelectionController.StartTargetSelection(activeEffect.predicate.TargetType, activeEffect.targetQuality, resolutionInfo, canSubmit, activeEffect.predicate.AffectedArea);

            if (cbTargetCountChanged != null)
                cbTargetCountChanged(targetCount, activeEffect.maxTargets, canSubmit);

            return;
        }

        /// after all that we actually get to do the effect lol
        activeEffect.predicate.PerformPredicate(currentTargetInfo, worldInfo, resolutionInfo);
        activeEffectIndex += 1;
        attemptResolution();
    }

    private void attemptResolutionLeft()
    {
        Debug.Log("cardPlayedLeft");

        if (activeEffectIndex >= activeCard.Data.CardEffectsLeft.Count)
        {
            // have gone past last effect, so end resolution
            removeActiveCard();
            return;
        }

        // since we know the active effect is valid now, just get it by index for the rest of the method
        CardEffect activeEffect = activeCard.Data.CardEffectsLeft[activeEffectIndex];

        if (!checkedCondition)
        {
            Debug.Log("bingus");
            // just got to this effect, so check if it is true
            if (activeEffect.condition.CheckCondition(worldInfo, resolutionInfo))
            {
                Debug.Log("poggers");
                // is true, so skip this later
                checkedCondition = true;
                // set up new target info
                currentTargetInfo = new TargetInfo();
            }
            else
            {
                Debug.Log("sadge");
                //not true, so skip this entire effect
                activeEffectIndex += 1;
                attemptResolution();
                return;
            }
        }

        int targetCount = currentTargetInfo.GetTargetCount(activeEffect.predicate.TargetType);
        if (targetCount < activeEffect.maxTargets && !currentTargetInfo.StoppedBeforeMax)
        {
            // not enough targets selected

            bool canSubmit = targetCount >= activeEffect.minTargets;
            targetSelectionController.StartTargetSelection(activeEffect.predicate.TargetType, activeEffect.targetQuality, resolutionInfo, canSubmit, activeEffect.predicate.AffectedArea);

            if (cbTargetCountChanged != null)
                cbTargetCountChanged(targetCount, activeEffect.maxTargets, canSubmit);

            return;
        }

        /// after all that we actually get to do the effect lol
        activeEffect.predicate.PerformPredicate(currentTargetInfo, worldInfo, resolutionInfo);
        activeEffectIndex += 1;
        attemptResolution();
    }

    /// <summary>
    /// Separate function for handling a tower card since its not like spells/skills
    /// </summary>
    private void resolveTower()
    {
        if (activeCard.Data.hasLeftBonus && isLeft)
        {
            if (currentTargetInfo.Tiles == null || currentTargetInfo.Tiles.Count < 1)
            {
                currentTargetInfo = new TargetInfo();
                targetSelectionController.StartTowerPreview(activeCard.Data.LeftTowerPrefab, resolutionInfo);
                return;
            }

            worldController.SpawnTower(activeCard.Data.LeftTowerPrefab, currentTargetInfo.Tiles[0], currentTargetInfo.Direction);
            removeActiveCard();
        }
        else
        {
            if (currentTargetInfo.Tiles == null || currentTargetInfo.Tiles.Count < 1)
            {
                currentTargetInfo = new TargetInfo();
                targetSelectionController.StartTowerPreview(activeCard.Data.TowerPrefab, resolutionInfo);
                return;
            }

            worldController.SpawnTower(activeCard.Data.TowerPrefab, currentTargetInfo.Tiles[0], currentTargetInfo.Direction);
            removeActiveCard();
        }
    }

    public void cheatCard(CardController CardController)
    {
        removeActiveCard();
        setActiveCard(CardController);
    }

    /// <summary>
    /// Sets which card needs to be currently resolved
    /// </summary>
    private void setActiveCard(CardController CardController)
    {
        ResolutionZone.Add(CardController);

        activeCard = CardController;

        activeEffectIndex = 0;
        resolutionInfo = new ResolutionInfo();
        currentTargetInfo = new TargetInfo();
        checkedCondition = false;

        attemptResolution();
    }

    /// <summary>
    /// Removes the current active card likely because it has finished resolving
    /// </summary>
    private void removeActiveCard()
    {
        if(ResolutionZone.Remove(activeCard))
            DiscardZone.Add(activeCard);

        activeCard = null;

        if (cbCardRemoved != null)
            cbCardRemoved();
        if (cbTargetCountChanged != null)
            cbTargetCountChanged(0, 0, false);
    }

    /// <summary>
    /// Called when the player selects a tile to be targetted
    /// </summary>
    public void OnSelectTile(TileController tile, Tile.TileDirection direction)
    {
        if (tile != null)
            currentTargetInfo.Add(tile);
        else
            currentTargetInfo.StoppedBeforeMax = true;

        currentTargetInfo.Direction = direction;
        attemptResolution();
    }

    /// <summary>
    /// Called when the player selects a tower to be targetted
    /// </summary>
    public void OnSelectTower(TowerController tower)
    {
        if (tower != null)
            currentTargetInfo.Add(tower);
        else
            currentTargetInfo.StoppedBeforeMax = true;

        attemptResolution();
    }

    /// <summary>
    /// Called when the player selects an enemy to be targetted
    /// </summary>
    public void OnSelectEnemy(EnemyController enemy)
    {
        if (enemy != null)
            currentTargetInfo.Add(enemy);
        else
            currentTargetInfo.StoppedBeforeMax = true;

        attemptResolution();
    }
}

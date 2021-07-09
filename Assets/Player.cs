using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator; // The animator component
    Attack[] possibleHits; // The possible hits for the current attack
    ActionCommand[] availableAC; // The possible action command data for the current attack
    List<ActionCommand> activeAC = new List<ActionCommand>(); // The action commands in play
    List<ActionCommand> storedAC = new List<ActionCommand>(); // The stored action commands
    //bool inAttack; // true if currently attacking
    public Enemy target; // The target of the current attack
    int targetInd; // The index of the target in the Enemy.activeEnemies list
    public GameObject targetSelector; // A prefab of the arrow that appears when selecting a target
    GameObject activeTargetSelector; // The arrow when selecting a target
    //bool choosingTargets; // true if selecting a target to attack
    AttackMove preparingMove; // The chosen attack move
    string walkingToAnim; // animation to play when walking
    public float WALK_SPEED; // Speed to walk at
    float walkTarget; // The x value the player is walking to
    float initialSpot; // The spot the player started the battle at
    int currentlyWalking; // -1 if walking left, 1 if walking right, 0 if not moving
    public GameObject acRing; // A prefab of the ring that appears when an action command is coming up
    GameObject currentACRing; // The ring that appears when an action command is coming up

    private void Start()
    {
        animator = GetComponent<Animator>();
        initialSpot = transform.position.x;
    }

    private void Update()
    {
        if (currentlyWalking != 0)
        {
            transform.position += new Vector3(WALK_SPEED * currentlyWalking * Time.deltaTime, 0);
            if ((currentlyWalking > 0) == (transform.position.x > walkTarget))
            {
                transform.position = new Vector2(walkTarget, transform.position.y);
                animator.Play("empty", 1);
                animator.Play(walkingToAnim);
                currentlyWalking = 0;
                if (Util.gameState == GameState.playerWalkToTarget)
                    Util.gameState = GameState.playerAttack;
                else if (Util.gameState == GameState.playerWalkBack)
                    Util.gameState = GameState.enemyTurn;
            }
        }
    }
    /// <summary>
    /// Called when the Z key is pressed
    /// </summary>
    void OnZPress()
    {
        if (Util.gameState == GameState.playerAttack)
            for (int i = 0; i < activeAC.Count; i++)
            {
                if (activeAC[i].key == 'Z')
                {
                    if (activeAC[i].stored)
                        storedAC.Add(activeAC[i]);
                    else
                        animator.Play(activeAC[i].anim);
                    if (activeAC[i].good)
                        ACRing(0.5f, "success");
                    else
                        ACRing(0.5f, "fail");
                    EndAllAC();
                }
            }
        else if (Util.gameState == GameState.playerChooseTargets)
        {
            Debug.Log(Util.gameState);
            StartAttack(preparingMove);
            Destroy(activeTargetSelector);
        }
    }

    /// <summary>
    /// Called when the X key is pressed
    /// </summary>
    void OnXPress()
    {
        for (int i = 0; i < activeAC.Count; i++)
        {
            if (activeAC[i].key == 'X')
            {
                if (activeAC[i].stored)
                    storedAC.Add(activeAC[i]);
                else
                    animator.Play(activeAC[i].anim);
                if (activeAC[i].good)
                    ACRing(0.5f, "success");
                else
                    ACRing(0.5f, "fail");
                EndAllAC();
            }
        }
    }

    /// <summary>
    /// Called when the left arrow key is pressed
    /// </summary>
    void OnLeftPress()
    {
        if (Util.gameState == GameState.playerChooseTargets)
        {
            if (--targetInd == -1)
            {
                targetInd = Enemy.activeEnemies.Count - 1;
            }
            target = Enemy.activeEnemies[targetInd];
            activeTargetSelector.transform.position = target.transform.position + new Vector3(0, 1);
        }
    }

    /// <summary>
    /// Called when the right arrow key is pressed
    /// </summary>
    void OnRightPress()
    {
        if (Util.gameState == GameState.playerChooseTargets)
        {
            if (++targetInd == Enemy.activeEnemies.Count)
            {
                targetInd = 0;
            }
            target = Enemy.activeEnemies[targetInd];
            activeTargetSelector.transform.position = target.transform.position + new Vector3(0, 1);
        }
    }

    /// <summary>
    /// Starts a new action command 
    /// </summary>
    /// <param name="index">The index of the action command in the availableAC array</param>
    public void AC(int index)
    {
        for (int i = 0; i < activeAC.Count; i++)
        {
            if (activeAC[i].key == availableAC[index].key)
            {
                activeAC.RemoveAt(i);
                break;
            }
        }
        activeAC.Add(availableAC[index]);
    }

    /// <summary>
    /// Activates the animation for a stored action command
    /// </summary>
    /// <param name="key"></param>
    public void TriggerStoredAC(string key)
    {
        for (int i = 0; i < storedAC.Count; i++)
        {
            Debug.Log(key);
            if (storedAC[i].key == key.ToUpper()[0])
            {
                animator.Play(storedAC[i].anim);
                break;
            }
        }
        RemoveAllStoredAC();
    }

    /// <summary>
    /// deletes all stored action command animations
    /// </summary>
    public void RemoveAllStoredAC()
    {
        storedAC = new List<ActionCommand>();
    }

    /// <summary>
    /// Ends the active action command for the given key
    /// </summary>
    /// <param name="key"></param>
    public void EndAC(string key)
    {
        for (int i = activeAC.Count - 1; i >= 0; i--)
        {
            if (activeAC[i].key == key.ToUpper()[0])
            {
                activeAC.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Ends the action command window for all keys
    /// </summary>
    public void EndAllAC()
    {
        activeAC = new List<ActionCommand>();
    }

    /// <summary>
    /// Displays the ring around the player when an action command is coming up
    /// </summary>
    /// <param name="time">time for the ring to last, in seconds</param>
    public void ACRing(float time)
    {
        if (currentACRing != null)
            Destroy(currentACRing);
        currentACRing = Instantiate(acRing, transform.position, new Quaternion(0, 0, 0, 0));
        currentACRing.GetComponent<Animator>().speed = 1 / time;
        Destroy(currentACRing, time);
    }

    /// <summary>
    /// Displays the ring around the player when an action command is coming up
    /// </summary>
    /// <param name="time">time for the ring to last, in seconds</param>
    /// <param name="type">string indicating the animation for the ring to play</param>
    public void ACRing(float time, string type)
    {
        if (currentACRing != null)
            Destroy(currentACRing);
        currentACRing = Instantiate(acRing, transform.position, new Quaternion(0, 0, 0, 0));
        currentACRing.GetComponent<Animator>().speed = 1/time;
        currentACRing.GetComponent<Animator>().Play(type);
        Destroy(currentACRing, time);
    }

    /// <summary>
    /// Walks to a point relative to the targeted enemy
    /// </summary>
    /// <param name="offset">distance left or right of the enmy to move to</param>
    /// <param name="playWhenDone">animation state name to play when walking target is reached</param>
    public void MoveToRelativePoint(float offset, string playWhenDone)
    {
        WalkToAbsolutePoint(target.transform.position.x + offset, playWhenDone);
    }

    /// <summary>
    /// Walk back to where the player started
    /// </summary>
    /// <param name="playWhenDone">The animation to play when the point is reached</param>
    public void MoveToStartingPosition(string playWhenDone)
    {
        WalkToAbsolutePoint(initialSpot, playWhenDone);
    }

    /// <summary>
    /// Walk to the specified X coordinate
    /// </summary>
    /// <param name="newPosition">The X coordinate to walk to</param>
    /// <param name="playWhenDone">The animation to play when the point is reached</param>
    /// <param name="cancelOtherAnims">If false, continue playing other animations while walking</param>
    public void WalkToAbsolutePoint(float newPosition, string playWhenDone, bool cancelOtherAnims = true)
    {
        walkTarget = newPosition;
        walkingToAnim = playWhenDone;
        animator.Play("walk", 1);
        if (cancelOtherAnims)
            animator.Play("stand");
        if (transform.position.x > newPosition)
        {
            currentlyWalking = -1;
            return;
        }
        else if (transform.position.x < walkTarget)
        {
            currentlyWalking = 1;
            return;
        }
        // Already at the exact point. Unlikely, but possible
        animator.Play("empty", 1);
        animator.Play(playWhenDone);
    }

    /// <summary>
    /// Starts the animation for the specified move
    /// </summary>
    /// <param name="move">The move to start</param>
    public void StartAttack(AttackMove move)
    {
        if (Util.gameState == GameState.playerChooseTargets)
        {
            if (move.useRelativePosition)
            {
                Util.gameState = GameState.playerWalkToTarget;
                MoveToRelativePoint(move.relativePosition, move.startAnim);
            }
            else
            {
                Util.gameState = GameState.playerAttack;
                animator.Play(move.startAnim);
            }
            possibleHits = move.dmgVals;
            availableAC = move.actionCommands;
        }
    }

    /// <summary>
    /// Brings up the arrow for selecting a target
    /// </summary>
    /// <param name="move">The move to perform afterwards</param>
    public void LookForTargets(AttackMove move)
    {
        target = Enemy.activeEnemies[0];
        activeTargetSelector = Instantiate(targetSelector);
        activeTargetSelector.transform.position = target.transform.position + new Vector3(0, 1);
        Util.gameState = GameState.playerChooseTargets;
        preparingMove = move;
    }

    /// <summary>
    /// Damages the targeted enemy
    /// </summary>
    /// <param name="strengthIndex">The hit in the possibleHits array to attack with</param>
    public void DamageTarget(int strengthIndex)
    {
        target.GetAttacked(possibleHits[strengthIndex]);
    }

    /// <summary>
    /// Ends the current attack
    /// </summary>
    public void EndAttackAnim()
    {
        Util.gameState = GameState.playerWalkBack;
        MoveToStartingPosition("stand");
    }
}



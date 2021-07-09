using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp; // Current HP of the enemy
    public int baseDef; // Defense Power. Incoming attacks' powers will be reduced by this amount
    public static List<Enemy> activeEnemies; // A list of all the enemies in play
    public string hurtAnim; // Default animation to play when taking damage. Can be overridden by angle rules
    public AngleRule[] angleRules; // Special behaviors when getting hit from specific angles
    int hitsTaken; // The number of hits the enemy has taken on the current turn
    
    Animation anim; // The animation component

    private void Awake()
    {
        if (activeEnemies == null)
        {
            activeEnemies = new List<Enemy>();
            Debug.Log("a");
        }
        activeEnemies.Add(this);
        Debug.Log(activeEnemies.Count);
        anim = GetComponent<Animation>();
    }
    /// <summary>
    /// Deals damage to this enemy, using a raw attack value
    /// </summary>
    /// <param name="pwr">The attack's base damage, without defense or modifiers applied</param>
    /// <returns>true if the attack successfully deals any damage, false otherwise</returns>
    public bool GetAttacked(Attack atk)
    {
        int pwrChange = -baseDef;
        anim.Play(hurtAnim);
        foreach (AngleRule a in angleRules)
        {
            if (a.angle == atk.angle)
            {
                // Apply angle rule
                if (!a.animChange.Equals(""))
                {
                    anim.Play(a.animChange);
                }
                pwrChange += a.damageModifier;
            }
        }
        Debug.Log(pwrChange);
        if (-pwrChange < atk.pwr)
            TakeAbsoluteDamage(atk.pwr + pwrChange);
        else 
            TakeAbsoluteDamage(0);
        return true;
    }

    /// <summary>
    /// Deals damage to this enemy, ignoring defense, weaknesses or any invulnerabilities. Does not play an animation, but does show the amount of damage taken onscreen.
    /// </summary>
    /// <param name="pwr">Total damage to deal. Modifiers will NOT be applied.</param>
    public void TakeAbsoluteDamage(int pwr)
    {
        hp -= pwr;
        Util.ShowDamageNumber(transform.position, pwr, 1 + hitsTaken * 0.25f);
        hitsTaken++;
    }
}

/// <summary>
/// A rule for what to do when hit from a certain angle
/// </summary>
[System.Serializable]
public class AngleRule
{
    public AttackAngle angle;
    public string animChange;
    public int damageModifier;
}

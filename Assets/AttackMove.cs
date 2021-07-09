using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "TB Tools/Player Attack Move", order = 1)]
public class AttackMove : ScriptableObject
{
    public Attack[] dmgVals;
    public ActionCommand[] actionCommands;
    public float relativePosition;
    public bool useRelativePosition;
    public string startAnim;
}

[System.Serializable]
public class Attack
{
    public int pwr;
    public AttackAngle angle;
    public override string ToString()
    {
        return pwr + " " + angle;
    }
}

[System.Serializable]
public class ActionCommand
{
    public char key;
    public bool stored;
    public string anim;
    public bool good;
}

public enum AttackAngle
{
    unset,
    front,
    back,
    below,
    above
}


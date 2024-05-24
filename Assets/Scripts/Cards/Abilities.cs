using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : ScriptableObject
{   
    public bool hasRevealAbility;
    public bool hasStrikeAbility;
    public bool hasDeathAbility;
    public bool hasRoundStartAbility;

    public float revealAbilityTime;
    public float strikeAbilityTime;
    public float deathAbilityTime;
    public float roundStartAbilityTime;

    public virtual void reveal(GameObject me){}
    public virtual void strike(GameObject me){}
    public virtual void death(GameObject me){}
    public virtual void roundStart(GameObject me){}
}

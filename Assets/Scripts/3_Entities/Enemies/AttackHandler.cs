using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] float timeToAttack = 2;
    [SerializeField] int chargesToSpecial = 3;
    int currentCharges;

    Action TryToDoNormalAttack;
    Action DoNormalAttack;
    Func<bool> CanDoNormalAttack;

    Action TryToDoSpecialAttack;
    Action DoSpecialAttack;
    Func<bool> CanDoSpecialAttack;

    float cdTimer;
    bool inCD;


    public void Initialize(Action _TryToDoNormalAttack, Action _DoNormalAttack, Func<bool> _CanDoNormalAttack,
    Action _TryToDoSpecialAttack, Action _DoSpecialAttack, Func<bool> _CanDoSpecialAttack)
    {
        TryToDoNormalAttack = _TryToDoNormalAttack;
        DoNormalAttack = _DoNormalAttack;
        CanDoNormalAttack = _CanDoNormalAttack;

        TryToDoSpecialAttack = _TryToDoSpecialAttack;
        DoSpecialAttack = _DoSpecialAttack;
        CanDoSpecialAttack = _CanDoSpecialAttack;
    }
    bool attacking;

    public void Refresh()
    {
        if (attacking) return;
        if (inCD)
        {
            cdTimer += Time.deltaTime;
            if(cdTimer >= timeToAttack)
            {
                cdTimer = 0;
                inCD = false;
            }

            return;
        }

        if(currentCharges >= chargesToSpecial)
        {
            if (CanDoSpecialAttack())
            {
                DoSpecialAttack();
                currentCharges = 0;
                attacking = true;
            }
            else
                TryToDoSpecialAttack();

            return;
        }

        if (CanDoNormalAttack())
        {
            DoNormalAttack();
            currentCharges += 1;
            attacking = true;
        }
        else
            TryToDoNormalAttack();
    }

    public void EndAttack()
    {
        attacking = false;
        inCD = true;
    }

    public void ResetAttack()
    {
        inCD = false;
        cdTimer = 0;
        attacking = false;
    }

    public void ResetCharges()
    {
        currentCharges = 0;
    }
}

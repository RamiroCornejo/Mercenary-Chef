using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEffectComponent : EffectBase
{
    [SerializeField] bool startWithEfect = false;

    [SerializeField] Animator anim = null;

    [SerializeField] AnimatorOverrideController overrideAnimator = null;
    [SerializeField] Character character = null;
    [SerializeField] PlayerMovementComp movementComp = null;
    [SerializeField] EffectBase trueStunEffect = null;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        if (!startWithEfect)
        {
            CancelEffect();
            character.effectReceiver.RemoveEffect(EffectName.OnStun);
            trueStunEffect.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(WaitToApplyEffect());
        }
    }

    IEnumerator WaitToApplyEffect()
    {
        yield return new WaitForEndOfFrame();
        character.effectReceiver.TakeEffect(EffectName.OnStun);
    }

    void CancelEffect()
    {
        anim.runtimeAnimatorController = overrideAnimator;
        movementComp.SetSpeedByDivise(1);
    }

    protected override void OnEffect()
    {
        character.StartWHangover();
        anim.Play("WakeUp");
    }

    protected override void OffEffect()
    {
        CancelEffect();
        character.effectReceiver.RemoveEffect(EffectName.OnStun);
        trueStunEffect.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    protected override void OnTickEffect(float cdPercent)
    {
    }
}
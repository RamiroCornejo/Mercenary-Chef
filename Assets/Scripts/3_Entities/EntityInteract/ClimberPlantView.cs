using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimberPlantView : MonoBehaviour
{
    [SerializeField] DamageReceiver dmgReceiver = null;

    private void Start()
    {
        dmgReceiver.InvulnerabilityFeedback += OnInvulnerable;
    }


    void OnInvulnerable(DamageType type)
    {

    }

    public void OnHit()
    {

    }

    public void OnDead()
    {
        gameObject.SetActive(false);
    }
}

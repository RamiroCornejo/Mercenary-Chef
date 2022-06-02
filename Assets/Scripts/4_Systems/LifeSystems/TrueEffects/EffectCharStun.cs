using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCharStun : EffectBase
{
    protected override void OffEffect()
    {
        Character.TrackInput(true);
    }

    protected override void OnEffect()
    {
        Character.TrackInput(false);
    }

    protected override void OnTickEffect(float cdPercent)
    {
    }
}

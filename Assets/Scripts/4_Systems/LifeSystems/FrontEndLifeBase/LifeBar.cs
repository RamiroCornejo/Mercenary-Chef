﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : FrontendStatBase
{
    public Text val;
    public GenericBar genbar;

    public override void OnValueChange(int value, int max = 100, bool anim = false)
    {
        genbar.Configure(max);
        genbar.SetValue(value);
    }

    public override void OnValueChangeWithDelay(int value, float delay, int max = 100, bool anim = false){}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeBase : StatBase
{
    FrontendStatBase uilife;

    /////////////////////////////////////////////////////////////////////////////////
    /// CONSTRUCTORS
    /////////////////////////////////////////////////////////////////////////////////
    ///
    public LifeBase(int maxHealth, int initial_Life) : base(maxHealth, initial_Life)
    {
        
    }
    public LifeBase(int maxHealth, FrontendStatBase _uilife, int initial_Life) : base(maxHealth, initial_Life)
    {
        uilife = _uilife;
        if (uilife) uilife.OnValueChange(maxHealth, maxHealth);
    }
    public LifeBase(int maxHealth, Action<float, float> change, int initial_Life) : base(maxHealth, initial_Life)
    {
        this.lifechange = change;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /// PUBLIC: Event Listeners
    /////////////////////////////////////////////////////////////////////////////////
    ///
    public event Action gainlife;
    public event Action loselife;
    public event Action death;
    public event Action cannotAddMore = delegate { };
    public event Action cannotRemoveMore = delegate { };
    public event Action<float, float> lifechange = delegate { };
    public void AddEventListener_LoseLife(Action listener) { loselife += listener; }
    public void AddEventListener_GainLife(Action listener) { gainlife += listener; }
    public void AddEventListener_Death(Action listener) { death += listener; }
    public void AddEventListener_CannotAddMore(Action listener) { cannotAddMore += listener; }
    public void AddEventListener_CannotRemoveMore(Action listener) { cannotRemoveMore += listener; }
    public void AddEventListener_OnLifeChange(Action<float, float> listener) { lifechange += listener; }

    /////////////////////////////////////////////////////////////////////////////////
    /// This is From inheritance StatBase
    /////////////////////////////////////////////////////////////////////////////////
    ///
    protected override void OnAdd() { gainlife.Invoke(); }
    protected override void OnRemove() { loselife.Invoke(); }
    protected override void OnLoseAll() { death.Invoke(); }
    protected override void CanNotAddMore() { cannotAddMore.Invoke(); }
    protected override void CanNotRemoveMore() { cannotRemoveMore.Invoke(); }
    protected override void OnValueChange(int value, int max, string message)
    {
        lifechange.Invoke(value, max);
        if (uilife != null) uilife.OnValueChange(value, max);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Trap : MonoBehaviour
{
    bool activate;
    [SerializeField] UnityEvent ActivateEvent = new UnityEvent();
    [SerializeField] UnityEvent DesactivateEvent = new UnityEvent();

    private void Start()
    {
        OnInitialize();
    }

    public void ActivateTrap()
    {
        if (activate) return;
        activate = true;
        ActivateEvent.Invoke();
        OnActivate();
    }

    public void DesactivateTrap()
    {
        if (!activate) return;
        activate = false;
        DesactivateEvent.Invoke();
        OnDesactivate();
    }

    protected abstract void OnActivate();

    protected abstract void OnDesactivate();

    protected abstract void OnInitialize();

    private void Update()
    {
        if (!activate) return;
        OnUpdate();
    }

    protected abstract void OnUpdate();
}

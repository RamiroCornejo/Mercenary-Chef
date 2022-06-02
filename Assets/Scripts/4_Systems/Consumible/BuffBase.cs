using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    [SerializeField] protected bool isPermanent = false;
    [SerializeField] float buffTime = 5f;
    float timer;

    public void UseBuff(int quality)
    {
        OnUseBuff();
        if (isPermanent) Destroy(this.gameObject);
    }

    protected abstract void OnUseBuff();

    public void EndBuff()
    {
        OnEndBuff();
        Destroy(this.gameObject);
    }

    protected abstract void OnEndBuff();

    public void ResetTime()
    {
        timer = 0;
    }

    public void ChangeQuality(int quality)
    {

    }

    private void Update()
    {
        if (isPermanent) return;
        timer += Time.deltaTime;

        if (timer >= buffTime)
        {
            EndBuff();
        }
    }
}

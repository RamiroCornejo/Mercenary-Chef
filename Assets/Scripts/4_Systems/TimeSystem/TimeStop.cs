using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    [SerializeField] float time_stop_time = 0.1f;
    public static TimeStop instance;
    private void Awake() => instance = this;
    public static void Stop() => instance.StopTime();
    void StopTime()
    {
        StartCoroutine(DoStop());
    }

    IEnumerator DoStop()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time_stop_time);
        Time.timeScale = 1;
        yield return null;
    }
}

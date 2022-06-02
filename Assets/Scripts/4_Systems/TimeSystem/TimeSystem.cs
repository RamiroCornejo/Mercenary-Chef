using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] TimeSystem_UI time_ui;

    DateTime mydatetime;

    float timer;

    private void Start()
    {
        time_ui = GameManager.MainCanvas.InstanceNewObject<TimeSystem_UI>(time_ui.gameObject);
        Debug.Log("instanciado time system");
        mydatetime = new DateTime();

        time_ui.Hour = mydatetime.Hour.ToString("00") + ":" + mydatetime.Minute.ToString("00");
        time_ui.Day = mydatetime.DayOfWeek.ToString();
    }

    private void Update()
    {
        if (timer < 1)
        {
            timer = timer + 1 * Time.deltaTime;
        }
        else
        {
            timer = 0;
            mydatetime = mydatetime.AddHours(1);

            time_ui.Hour = mydatetime.Hour.ToString("00") + ":" + mydatetime.Minute.ToString("00");
            time_ui.Day = mydatetime.DayOfWeek.ToString();
        }
    }
}

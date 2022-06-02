using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeSystem_UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hour;
    [SerializeField] TextMeshProUGUI days;

    public string Hour { set => hour.text = value; }
    public string Day { set => days.text = value; }
}

using System.Collections;
using UnityEngine;
using System;

public class JoyStickSpin : MonoBehaviour
{
    [Header("Spinning Dectection Options")]
    [SerializeField] private float spinAngleCheckUpdateTimer = 0.1f;
    [SerializeField] [Range(0.0f, 180.0f)] private float spinValidAngleLimit = 30.0f; 
    [SerializeField] private int validSpinCheckRows = 5;

    private bool isSpinning = false;

    private Vector2 joyStickInput = Vector2.zero;
    private Vector2 lastJoyStickInput = Vector2.zero;
    private bool isCheckingSpinInput = false; 
    private int validSpinCheckCounter = 0;

    public Action StartSpin = delegate { };
    public Action EndSpin = delegate { };

    public bool IsSpinning
    {
        get { return isSpinning; }
        private set { }
    }

    private void OnEnable()
    {
        isCheckingSpinInput = false;
        isSpinning = false;
    }

    void Update()
    {
        SpinInput();
        CheckJoyStickSpinning();
    }

    private void SpinInput()
    {
        joyStickInput = new Vector2(Input.GetAxisRaw("HorizontalR"), Input.GetAxisRaw("VerticalR"));
    }

    private void CheckJoyStickSpinning()
    {
        if (joyStickInput != lastJoyStickInput && !isCheckingSpinInput)
        {
            isCheckingSpinInput = true;
            StartCoroutine(JoyStickSpinningDetection());
        }

        if (!isSpinning && validSpinCheckCounter == validSpinCheckRows)
        {
            isSpinning = true;
            StartSpin.Invoke();
        }
        else if(isSpinning && validSpinCheckCounter != validSpinCheckRows)
        {
            isSpinning = false;
            EndSpin.Invoke();
        }
    }

    IEnumerator JoyStickSpinningDetection()
    {
        lastJoyStickInput = joyStickInput;

        yield return new WaitForSeconds(spinAngleCheckUpdateTimer);

        if (Vector2.Angle(lastJoyStickInput, joyStickInput) >= spinValidAngleLimit)
        {
            validSpinCheckCounter++;

            validSpinCheckCounter = Mathf.Clamp(validSpinCheckCounter, 0, validSpinCheckRows);
        }
        else
        {
            validSpinCheckCounter = 0;
        }
        isCheckingSpinInput = false;
    }
}
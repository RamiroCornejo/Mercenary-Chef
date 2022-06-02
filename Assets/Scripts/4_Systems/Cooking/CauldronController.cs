using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronController : MonoBehaviour
{
    [SerializeField] Transform ladle = null;
    [SerializeField] float ladleRotationSpeedMin = 5;
    [SerializeField] float ladleRotationSpeedMax = 5;
    [SerializeField] float acceleration = 1.5f;

    [SerializeField] float currentSpeed;
    [SerializeField] JoyStickSpin spinner = null;

    [SerializeField] float timeToComplete = 4;
    float timer;
    Vector3 initRotation;

    private void Start()
    {
        initRotation = ladle.localEulerAngles;
        spinner.StartSpin += StartSpin;
        spinner.EndSpin += EndSpin;
    }

    private void Update()
    {
        if (spinner.IsSpinning)
        {
            timer += Time.deltaTime;

            currentSpeed += acceleration* Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, ladleRotationSpeedMin, ladleRotationSpeedMax);

            ladle.localEulerAngles += Vector3.up * currentSpeed * Time.deltaTime;

            if(timer >= timeToComplete)
            {
                CompleteSpin();
            }
        }
    }
    
    void StartSpin()
    {
        currentSpeed = ladleRotationSpeedMin;
    }

    void EndSpin()
    {
        ladle.localEulerAngles = initRotation;
        timer = 0;
    }

    void CompleteSpin()
    {
        Debug.Log("COMPLETADO");
    }
}

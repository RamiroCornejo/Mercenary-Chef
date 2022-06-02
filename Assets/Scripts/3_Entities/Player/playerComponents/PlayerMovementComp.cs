using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComp : MonoBehaviour
{
    [SerializeField] PlayerView view = null;

    [SerializeField] float range = 30;
    [SerializeField] LayerMask user_interactable;
    [SerializeField] Transform rotator = null;

    public Transform Rotator => rotator;

    [SerializeField] CharacterController controller = null;
    [SerializeField] PlayerComponent_GroundCheck checker = null;
    [SerializeField] float gravity = 20;
    [SerializeField] float initialSpeed = 5f;


    [SerializeField] float dashForce = 2;
    [SerializeField] float dashTime = 5;
    [SerializeField] AnimationCurve dash_gravity_over_time;

    public Vector3 InputDirection => inputDir;
    Vector3 inputDir;
    Vector3 moveComposed;
    float currentSpeed;

    public float CurrentSpeed => currentSpeed;

    #region Settings
    public void Initialize()
    {
        currentSpeed = initialSpeed;
    }

    public void SetSpeedByDivise(float divise) => currentSpeed = initialSpeed / divise;

    #endregion

    #region Move
    public void Move()
    {
        Vector3 transformDirection = transform.TransformDirection(inputDir).normalized * Mathf.Clamp(inputDir.magnitude, 0, 1);
        Vector3 moveflat = transformDirection * currentSpeed * Time.deltaTime;

        moveComposed = new Vector3(moveflat.x, moveComposed.y, moveflat.z);

        if (checker.IsGrounded)
        {
            moveComposed.y = 0;
        }
        else
        {
            moveComposed.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveComposed);
        if (inputDir != Vector3.zero) BlockRotationOn(inputDir);
    }

    public void ReceiveInputX(float x)
    {
        inputDir.x = x;
    }

    public void ReceiveInputZ(float z)
    {
        inputDir.z = z;
    }

    #endregion

    #region Jump
    float timerDash;
    bool isDashing;
    public bool IsDashing => isDashing;

    Vector3 dashDir;
    float dinamicGravity;


    public void DashFixedUpdate()
    {
        Vector3 dashfall;

        dashfall = new Vector3(0, dinamicGravity, 0);
        var dashresult = dashDir + dashfall;

        controller.Move(dashresult * Time.deltaTime * CurrentSpeed * dashForce);

        BlockRotationOn(dashDir);
    }

    public bool DashUpdate()
    {
        if (timerDash > 0)
        {
            timerDash = timerDash - 1 * Time.deltaTime;
            dinamicGravity = dash_gravity_over_time.Evaluate(timerDash);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartDash()
    {
        isDashing = true;
        timerDash = dashTime;
        dashDir = rotator.transform.forward;
        dashDir.Normalize();

        view.BeginDash();
    }

    public void EndDash()
    {
        view.EndDash();
        timerDash = dashTime;
        dinamicGravity = 1;
        isDashing = false;
    }

    #endregion


    #region Rotation
    public void BlockRotationOn(Vector3 dir)
    {
        rotator.transform.forward = dir;
    }
    public void BlockRotationOnWithSmooth(Vector3 dir)
    {
        Vector3 myforward = rotator.forward;
        rotator.transform.forward = Vector3.SmoothDamp(rotator.transform.forward, dir, ref myforward, 0.3f);
    }

    Vector3 destY = new Vector3();
    Vector3 currY = new Vector3();
    #endregion
}

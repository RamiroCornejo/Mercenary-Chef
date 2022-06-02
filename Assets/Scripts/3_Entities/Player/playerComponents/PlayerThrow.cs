using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] Transform rotator;
    [SerializeField] float time_to_throw = 1f;
    [SerializeField] float force_min_to_throw = 2f;
    [SerializeField] float force_max_to_throw = 7f;
    [SerializeField] float force_current_captured = 0;
    public float Force => force_current_captured;
    [SerializeField] float intensity = 1;

    [SerializeField] AnimEvent anim_event;
    [SerializeField] Animator myAnimator;
    [SerializeField] PlayerMovementComp player_rotator_component;
    [SerializeField] Rotate_by_anim_curve feedback_animation;

    [SerializeField] ParticleSystem particle_ready;

    [SerializeField] ObjectThrowable model;

    PlayerThrowProyection proyector;

    public Transform shootPoint;
    public Vector3 shoot_velocity;

    bool is_executing;

    bool begin_throw;
    float timer_throw;

    Action onEnd = delegate { };
    Action onBegin = delegate { };

    #region UnityEngine

    private void Start()
    {
        proyector = GetComponent<PlayerThrowProyection>();
        //me subscribo al evento de terminó el begin throw para saber cuando ya puedo lanzar
        //me subscribo al evento de animacion de Throw
        anim_event.ADD_ANIM_EVENT_LISTENER("throw_begin_ended", EVENT_MyHandIsUprise);
        anim_event.ADD_ANIM_EVENT_LISTENER("throw_Successful", EVENT_ReleaseMyHand);
        anim_event.ADD_ANIM_EVENT_LISTENER("throw_end_ended", EVENT_AnimationCoreIsFinished);

        SMB_ThrowBehaviour my_smb = myAnimator.GetBehaviour<SMB_ThrowBehaviour>();
        my_smb.SubscribeToThrowEnterAndExit(ON_SMB_Enter, ON_SMB_Exit);
    }


    float percent_for_lerp = 0;
    private void Update()
    {
        if (is_executing)
        {
            //player_rotator_component.Update_Rotation();

            if (begin_throw)
            {
                if (timer_throw < time_to_throw)
                {
                    timer_throw = timer_throw + 1 * Time.deltaTime;
                    percent_for_lerp = (timer_throw * 100 / time_to_throw) * 0.01f;
                    LerpAnimation(percent_for_lerp);

                }
                else
                {
                    particle_ready.Play();

                    ResetHotBehaviour();
                }
            }

            force_current_captured = Mathf.Lerp(force_min_to_throw, force_max_to_throw, percent_for_lerp) * intensity;
            //Debug.Log("entra otra vez aca y es:" + force_current_captured);
            shoot_velocity = shootPoint.transform.forward * force_current_captured;
        }
    }

    #endregion

    #region Publics

    public void ADD_CALLBACK_OnEnd(Action onEnd) => this.onEnd = onEnd;
    public void ADD_CALLBACK_OnBegin(Action onBegin) => this.onBegin = onBegin;

    public void INPUT_Press_Throw()
    {
        myAnimator.SetBool("throw", true); //acciono la animacion y nada mas, si la animacion respone hago el resto
    }
    public void INPUT_Press_Release()
    {
        myAnimator.SetBool("throw", false); //acciono la animacion y nada mas, si la animacion respone hago el resto
        is_executing = false;
        proyector.StopRender();
        feedback_animation.Graphics_HIDE();
        Debug.Log("IS RELEASE");
    }
    public void INPUT_CancelThrow()
    {
        proyector.StopRender();
        feedback_animation.Graphics_HIDE();
        myAnimator.SetTrigger("throw_cancel");
        onEnd.Invoke();
    }

    #endregion


    ////////////////////////////////////////////////////////////////// PRIVADOS

    void LerpAnimation(float lerp) => feedback_animation.Lerp(lerp); //Lerp 0-1

    void ResetHotBehaviour()
    {
        begin_throw = false;
        timer_throw = 0;
    }

    void StartHotBehaviour()
    {
        begin_throw = true;
        timer_throw = 0;
    }

    ////////////////////////////////////////////////////////////////// EVENTOS ANIMACION

    #region EVENTOS POR ANIMACION

    // STATE_MACHINE_BEHAVIOUR: Entró a la Sub State Machine
    void ON_SMB_Enter()
    {
        onBegin.Invoke();
        is_executing = true; //para el Update
        percent_for_lerp = 0;
        force_current_captured = 0;
        shoot_velocity = shootPoint.transform.forward * force_min_to_throw;
        ResetHotBehaviour();
    }
    // STATE_MACHINE_BEHAVIOUR: Salió de la Sub State Machine
    void ON_SMB_Exit()
    {
        is_executing = false; //para el Update
        proyector.StopRender();
        ResetHotBehaviour();
    }

    // ANIM FRAME: punto justo donde mi mano llega atras de todo
    void EVENT_MyHandIsUprise()
    {
        proyector.BeginRender();
        StartHotBehaviour();
        feedback_animation.Graphics_SHOW();
    }

    // ANIM FRAME: punto justo en el que mi mano deberia soltar el objeto con toda la fuerza
    void EVENT_ReleaseMyHand()
    {
        feedback_animation.Graphics_HIDE();
        var throwable = Instantiate(model);
        throwable.transform.position = shootPoint.position;
        throwable.Shoot(shoot_velocity *3);

        //aca hago el instanciamiento
        //aca le doy la fuerza
        //aca lo roto
    }

    // ANIM FRAME: Este es el punto en el que la animacion core completa terminó, 
    // queda un poquito aún, pero lo uso para transicionar
    void EVENT_AnimationCoreIsFinished()
    {
        onEnd.Invoke();
        is_executing = false;
    }

    #endregion
}

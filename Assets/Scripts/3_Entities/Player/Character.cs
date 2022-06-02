using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public class Character : MonoBehaviour
{
    public static Character instance;

    bool IsGrounded => groundcheck.IsGrounded;

    //region componentes varios
    public CharacterController controller = null;
    GenericInteractor interactor = null;
    PlayerView view = null;
    AnimEvent animevent = null;
    PlayerDamageComponentByTrigger playerDamageComponent = null;
    PlayerMovementComp playerMovement;
    LifeComponent lifeComponent;
    PlayerComponent_GroundCheck groundcheck;
    public EffectReceiver effectReceiver;
    public PhysicalEquip visualEquip;

    public int Life => 100; //zaraza para test

    public static void TrackInput(bool tracking_value) => instance.isTracking = tracking_value;

    [Header("Throw options")]
    [SerializeField] PlayerThrow player_throw;

    [Header("Condition Bools Debug")]
    bool hangover = false;
    bool isAttacking => playerAttack.IsAttacking;
    [SerializeField] bool isThrowing;
    [SerializeField] bool isTracking = true;

    [SerializeField] float debug_velocity;

    PlayerAttack playerAttack;

    #region UnityEngine
    private void Awake()
    {
        instance = this;
        playerMovement = GetComponent<PlayerMovementComp>();
        playerMovement.Initialize();
    }
    private void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        view = GetComponent<PlayerView>();
        controller = GetComponent<CharacterController>();
        interactor = GetComponentInChildren<GenericInteractor>();
        lifeComponent = GetComponent<LifeComponent>();
        groundcheck = GetComponentInChildren<PlayerComponent_GroundCheck>();
        visualEquip = GetComponent<PhysicalEquip>();
        effectReceiver = GetComponent<EffectReceiver>();
        interactor.InitializeInteractor();
        OnStart_Subscribe_Anim_Events();
        playerAttack.Initialize();
        player_throw.ADD_CALLBACK_OnBegin(() => isThrowing = true);
        player_throw.ADD_CALLBACK_OnEnd(() => isThrowing = false);


        animevent.ADD_ANIM_EVENT_LISTENER("SetIdle", SetIdle);

        SetStateMachine();

    }

    #region STATEMACHINE
    EventStateMachine<CharTransitions> sm;
    void SetStateMachine()
    {
        var onCinematic = new EState<CharTransitions>("Cinematic");
        var onDialogue = new EState<CharTransitions>("Dialogue");
        var idle = new EState<CharTransitions>("Idle");
        var move = new EState<CharTransitions>("Move");
        var jump = new EState<CharTransitions>("Jump");
        var death = new EState<CharTransitions>("Death");

        ConfigureState.Create(onCinematic)
            .SetTransition(CharTransitions.Idle, idle)
            .Done();

        ConfigureState.Create(onDialogue)
            .SetTransition(CharTransitions.Idle, idle)
            .Done();

        ConfigureState.Create(idle)
            .SetTransition(CharTransitions.Move, move)
            .SetTransition(CharTransitions.Jump, jump)
            .SetTransition(CharTransitions.DialogueStart, onDialogue)
            .SetTransition(CharTransitions.CinemataicStart, onCinematic)
            .SetTransition(CharTransitions.Dead, death)
            .Done();

        ConfigureState.Create(move)
            .SetTransition(CharTransitions.Idle, idle)
            .SetTransition(CharTransitions.Jump, jump)
            .SetTransition(CharTransitions.DialogueStart, onDialogue)
            .SetTransition(CharTransitions.CinemataicStart, onCinematic)
            .SetTransition(CharTransitions.Dead, death)
            .Done();

        ConfigureState.Create(jump)
            .SetTransition(CharTransitions.Idle, idle)
            .SetTransition(CharTransitions.Move, move)
            .SetTransition(CharTransitions.CinemataicStart, onCinematic)
            .SetTransition(CharTransitions.DialogueStart, onDialogue)
            .SetTransition(CharTransitions.Dead, death)
            .Done();

        ConfigureState.Create(death)
            .Done();

        sm = new EventStateMachine<CharTransitions>();
        new CharIdle(idle, sm, view, playerMovement);
        new CharMove(move, sm, view, playerMovement);
        new CharOnDialogue(onDialogue, sm);
        new CharCinematic(onCinematic, sm);
        new CharJump(jump, sm, playerMovement);
        new CharDead(death, sm);
        sm.StartSM(idle);
    }

    public void SetIdle() => sm.SendInput(CharTransitions.Idle);

    #endregion

    public void StartWHangover()
    {
        hangover = true;
        playerMovement.SetSpeedByDivise(2);
        sm.SendInput(CharTransitions.CinemataicStart);
    }

    public void EnterOnDialogue()
    {
        sm.SendInput(CharTransitions.DialogueStart);
    }

    void Repositionate()
    {
        if (PlayerStart.instance != null) controller.transform.position = PlayerStart.Position;
        else
        {
            controller.transform.position = new Vector3(0, 0, 0);
            controller.transform.eulerAngles = Vector3.zero;
        }
    }
    void FixedUpdate()
    {
        sm.FixedUpdate();
        #region Motor conditions
        if (isAttacking || !isTracking || isThrowing) return;//estoy deslizando, estoy cocinando, estoy en menues, etc etc etc etc
        #endregion


    }
    private void LateUpdate()
    {
        //PROVISIONAL => ME CAIGO DEL ESCENARIO 
        if (this.transform.position.y < -3)
        {
            Repositionate();
        }
    }
    private void Update()
    {
        sm.Update();
        view.ANIM_Velocity(controller.velocity.magnitude);
        effectReceiver.UpdateStates();

        #region DEBUGS Things
        if (lifeComponent)
        {
            if (Input.GetKeyDown(KeyCode.B)) { lifeComponent.ReceiveDamage(5); }
            if (Input.GetKeyDown(KeyCode.N)) { lifeComponent.Heal(5); }
            if (Input.GetKeyDown(KeyCode.M)) { lifeComponent.Add_To_Max(5); }
        }
        #endregion

        if (hangover) return;

        if (InputManagerShortcuts.RELEASE_Throw) { player_throw.INPUT_Press_Release(); }//RELEASE THROW
        if (Input.GetKeyDown(KeyCode.R)) { player_throw.INPUT_CancelThrow(); }
        //if (isThrowing)
        //{

        //}

        playerAttack.Update_WindowAttack();


        if (playerMovement.IsDashing || !isTracking) return;

        if (InputManagerShortcuts.PRESS_Throw) //PRESS THROW
        {
            player_throw.INPUT_Press_Throw();
        }


    }


    public void PRESS_Interact(KeyEventButon key)
    {
        interactor.Execute();
    }
    #endregion

    #region Combate

    public void PRESS_Attack(KeyEventButon key)
    {
        if (hangover) return;
        if (playerMovement.IsDashing || !isTracking || isThrowing || isAttacking) return;

        playerAttack.DoAttack();
    }
    #endregion



    public void PRESS_Jump(KeyEventButon key)
    {
        if (hangover) return;

        if (IsGrounded)
        {
            sm.SendInput(CharTransitions.Jump);
        }
    }
    #region ANIM EVENTS
    void OnStart_Subscribe_Anim_Events()
    {
        animevent = GetComponentInChildren<AnimEvent>();
        animevent.ADD_ANIM_EVENT_LISTENER("launch_jump", ANIM_EVENT_LaunchJump);
        animevent.ADD_ANIM_EVENT_LISTENER("land_jump", ANIM_EVENT_LandJump);
        //animevent.ADD_ANIM_EVENT_LISTENER("closewindow", ANIM_EVENT_CloseAttackWindow);
    }
    public void ANIM_EVENT_LaunchJump()
    {

    }
    public void ANIM_EVENT_LandJump()
    {

    }
   

    #endregion

    #region Deprecated
    void Feedback_OnHeal()
    {/*
        base.Feedback_OnHeal();*/
    }
    void Feedback_ReceiveDamage()
    {/*
        base.Feedback_ReceiveDamage();
        ScreenFeedback.instancia.PerderVida();
        view.PLay_Hit();
        view.Play_Clip_TakeDamage();*/
    }
    void OnDeath()
    {/*
        view.Play_Clip_Die();
        base.OnDeath();
        myThread.Death();
        GameLoop.Pause();
        GameLoop.Lose();*/
    }
    #endregion

}

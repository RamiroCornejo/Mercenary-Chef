using Tools.StateMachine;

public class EnemyIdle : StatesFunctions
{
    public EnemyIdle(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm) : base(myState, _sm)
    {
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
    }

    protected override void Exit(StateMachineInputs input)
    {
    }

    protected override void Update()
    {

    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

}

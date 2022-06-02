using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class SimpleFlee : MonoBehaviour
{
    NavMeshAgent agent;
    bool flee;

    float timer_eat;
    float Time_to_eat = 10f;

    float timer_relax;
    float Time_to_relax = 5f;

    float timer_recalculate;
    float Time_to_recalculate = 3f;

    public enum state_simple_flee { relax, search, flee }
    state_simple_flee states;

    BunnyPositions positions;
    public void SetZone(BunnyPositions pos) => positions = pos;

    [SerializeField] TextMeshPro debug;

    public float flee_speed;
    public float relax_speed;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = relax_speed;
        states = state_simple_flee.relax;
        GoToEat();
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public bool IsMoving => agent.velocity.magnitude > 0.1f;

    Vector3 pos_to_go;
    Vector3 cpos;
    Vector3 mypos;
    private void Update()
    {
        cpos = Character.instance.transform.position;
        mypos = this.transform.position;

        switch (states)
        {
            case state_simple_flee.relax:
                agent.speed = relax_speed;
                if (HasThreat)
                {
                    GoToHide();
                    states = state_simple_flee.flee;
                }
                else
                {
                    if (timer_eat < Time_to_eat)
                    {
                        timer_eat = timer_eat + 1 * Time.deltaTime;
                    }
                    else
                    {
                        timer_eat = 0;
                        GoToEat();
                    }
                }
                break;
            case state_simple_flee.search:
                agent.speed = flee_speed;

                if (HasThreat)
                {
                    GoToHide();
                    states = state_simple_flee.flee;
                }
                else
                {
                    if (timer_relax < Time_to_relax)
                    {
                        timer_relax = timer_relax + 1 * Time.deltaTime;
                    }
                    else
                    {
                        timer_relax = 0;
                        states = state_simple_flee.relax;
                        timer_eat = Time_to_eat;
                    }
                }

                break;
            case state_simple_flee.flee:

                agent.speed = flee_speed;
                if (HasThreat)
                {
                    if (IsLanded)
                    {
                        timer_recalculate = 0;
                        GoToHide();
                    }

                    if (timer_recalculate < Time_to_recalculate)
                    {
                        timer_recalculate = timer_recalculate + 1 * Time.deltaTime;
                    }
                    else
                    {
                        timer_recalculate = 0;
                        GoToHide();
                    }
                }
                else
                {
                    if (IsLanded)
                    {
                        states = state_simple_flee.search;
                    }
                    else
                    {
                        //que siga caminando
                    }
                }
                break;
        }

        debug.text = states.ToString();
    }

    bool IsLanded => Vector3.Distance(mypos, pos_to_go) < agent.stoppingDistance;
    bool HasThreat => Vector3.Distance(cpos, mypos) < 5;
    void GoToHide()
    {
        if (positions == null || positions.RandomHidePosition() == null) throw new System.Exception("NO TENES LOS TRANSFORMS O EL SCRIPT BUNNY POSITION");
        var pos = positions.RandomHidePosition().position;
        pos_to_go = pos;
        agent.SetDestination(pos);
    }
    void GoToEat()
    {
        if (positions == null || positions.RandomFoodPosition() == null) throw new System.Exception("NO TENES LOS TRANSFORMS O EL SCRIPT BUNNY POSITION");
        var pos = positions.RandomFoodPosition().position;
        pos_to_go = pos;
        agent.SetDestination(pos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EntusiastaFirstEncounter : MonoBehaviour
{
    [Header("Enter")]
    [SerializeField] GameObject entusiastaModel = null;
    [SerializeField] DialogueUtility dialog = null;
    [SerializeField] GameObject triggerEntry = null;

    [SerializeField] NavMeshAgent agentMovement = null;
    [SerializeField] float distanceToStop = 3;

    [SerializeField] Animator entusiastaAnim = null;

    [Header("Exit")]
    [SerializeField] Transform posToDash = null;
    [SerializeField] Transform posToSecondDash = null;
    [SerializeField] Transform posToShutDown = null;
    [SerializeField] float dashForce = 5;
    [SerializeField] float dashTimer = 1;

    public void TriggerEntusiasta()
    {
        entusiastaModel.SetActive(true);
        dialog.StartDialogue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            EntusiastaExit();
        }
    }

    public void EntusiastaEntry()
    {
        triggerEntry.SetActive(false);

        StartCoroutine(WaitOnDistance(Character.instance.transform, distanceToStop, ()=> { dialog.SkipDelay(); }));
    }

    IEnumerator WaitOnDistance(Transform target, float distance, Action OnEndWait)
    {
        agentMovement.isStopped = false;
        agentMovement.SetDestination(target.position);
        entusiastaAnim.SetBool("walking", true);
        while (true)
        {
            float dis = Vector2.Distance(target.position, agentMovement.transform.position);
            Debug.Log(dis);

            if(dis <= distance)
            {
                agentMovement.isStopped = true;
                agentMovement.velocity = Vector3.zero;
                entusiastaAnim.SetBool("walking", false);
                OnEndWait();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void EntusiastaExit()
    {        
        StartCoroutine(WaitOnDistance(posToDash, 0.4f, ExecuteFirstDash));
    }
    void ExecuteFirstDash() => StartCoroutine(DashCoroutine(GoToSecondPos));

    void GoToSecondPos() => StartCoroutine(WaitOnDistance(posToSecondDash, 0.4f, ExecuteSecondDash));

    void ExecuteSecondDash() => StartCoroutine(DashCoroutine(GoToThirdPos));

    void GoToThirdPos() => StartCoroutine(WaitOnDistance(posToShutDown, 0.4f, EntusiastaShutDown));

    IEnumerator DashCoroutine(Action OnEndDash)
    {
        float timer = 0;
        agentMovement.enabled = false;
        while (timer < dashTimer)
        {
            timer += Time.deltaTime;

            entusiastaModel.transform.localPosition += entusiastaModel.transform.forward * Time.deltaTime * dashForce;
            yield return new WaitForEndOfFrame();
        }
        agentMovement.enabled = true;
        OnEndDash();
    }

    void EntusiastaShutDown()
    {
        dialog.SkipDelay();
        gameObject.SetActive(false);
    }
}

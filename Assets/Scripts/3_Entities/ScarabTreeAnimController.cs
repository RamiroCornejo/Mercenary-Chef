using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarabTreeAnimController : MonoBehaviour
{
    [SerializeField] float minTimeToAnimation = 5;
    [SerializeField] float maxTimeToAnimation = 10;
    [SerializeField] Animator anim = null;
    [SerializeField] GameObject[] apples = new GameObject[0];
    [SerializeField] Renderer render = null;

    public GameObject currentApple;

    float currentTime;
    float timer;

    bool inAnimation;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (inAnimation) return;

        timer += Time.deltaTime;

        if (timer >= currentTime)
        {
            render.enabled = true;
            SelectApple();
            anim.Play("ScarabAppear");
            inAnimation = true;
        }
    }

    void StartTimer()
    {
        render.enabled = false;
        timer = 0;
        inAnimation = false;
        currentTime = Random.Range(minTimeToAnimation, maxTimeToAnimation);
    }

    public void StopAnimation()
    {
        gameObject.SetActive(false);
    }
    void EatApple()
    {
        currentApple.SetActive(false);
    }

    void AppearApple()
    {
        if(currentApple != null) currentApple.SetActive(true);
    }

    void SelectApple()
    {
        currentApple = apples[Random.Range(0, apples.Length)];
        Vector3 dir = (currentApple.transform.position - transform.position).normalized;

        transform.forward = dir;
    }
}

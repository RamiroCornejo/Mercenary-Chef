using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DialogueUIDisplay : MonoBehaviour
{
    public static DialogueUIDisplay instance { get; private set; }

    [SerializeField] TextMeshProUGUI[] titleText = new TextMeshProUGUI[0];
    [SerializeField] TextMeshProUGUI mainText = null;
    [SerializeField] Image[] spritesImg = new Image[0];

    [SerializeField] GameObject dialogHUD = null;
    [SerializeField] AnswerButton[] answersUI = new AnswerButton[0];
    [SerializeField] GameObject presToNext = null;
    bool isOpen;
    bool inAnimation = false;

    Action<int> next;

    Action DoWhenEndDelay;

    float delayTime;
    bool onDelay;
    float timer;
    bool inDialog;
    bool onAnswers;

    [SerializeField] float animSpeed = 0.01f;
    string originalText;
    char[] charPerChar;

    private void Awake()
    {
        instance = this;
    }

    public void ShowDialog(DialogParameters parameters, Action<int> nextIndex)
    {

        originalText = parameters.mainText;
        mainText.text = "";
        charPerChar = originalText.ToCharArray();
        inAnimation = true;
        StartCoroutine(TextAnimation());


        if (parameters.mySprite == null)
        {
            for (int i = 0; i < spritesImg.Length; i++)
                spritesImg[i].gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < spritesImg.Length; i++)
            {
                if (parameters.spriteDir == i)
                {
                    spritesImg[i].gameObject.SetActive(true);
                    spritesImg[i].sprite = parameters.mySprite;
                }
                else
                    spritesImg[i].gameObject.SetActive(false);
            }
        }
        next = nextIndex;
        inDialog = true;
        titleText[parameters.spriteDir].text = parameters.titleText;
    }

    IEnumerator TextAnimation()
    {
        for (int i = 0; i < charPerChar.Length; i++)
        {
            mainText.text += charPerChar[i];

            yield return new WaitForSeconds(animSpeed);
        }

        EndAnimation();
    }

    IEnumerator RefreshButtons()
    {
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();
        for (int i = 0; i < answersUI.Length; i++)
        {
            if (!answersUI[i].gameObject.activeSelf) continue;
            answersUI[i].transform.GetComponent<ContentSizeFitter>().enabled = false;
            answersUI[i].transform.GetComponent<ContentSizeFitter>().enabled = true;
        }
    }

    public void ForceEndDelay()
    {
        if (onDelay)
            EndDelay();
    }

    void EndAnimation()
    {
        presToNext.SetActive(true);
        inAnimation = false;
        mainText.text = originalText;
    }

    public void DisplayDelay(float _delayTime, Action _OnEndDelay)
    {
        delayTime = _delayTime;
        DoWhenEndDelay = _OnEndDelay;
        onDelay = true;
        timer = 0;
    }

    public void ShowAnswers(List<string> answers)
    {

        for (int i = 0; i < answers.Count; i++)
        {
            answersUI[i].gameObject.SetActive(true);
            answersUI[i].SetAnswer(answers[i], SelectAnswer, i);
        }

        onAnswers = true;
        StartCoroutine(RefreshButtons());
    }

    void SelectAnswer(int index)
    {
        for (int i = 0; i < answersUI.Length; i++)
        {
            answersUI[i].gameObject.SetActive(false);
        }

        onAnswers = false;
        next(index);
    }

    public void OpenDialog()
    {
        isOpen = true;
        dialogHUD.SetActive(true);
        for (int i = 0; i < spritesImg.Length; i++)
        {
            spritesImg[i].gameObject.SetActive(false);
            titleText[i].text = "";
        }
        mainText.text = "";

        Character.instance.EnterOnDialogue();
        PauseManager.instance.Pause();
    }

    public void CloseDialog()
    {
        Character.instance.SetIdle();
        isOpen = false;
        dialogHUD.SetActive(false);
        PauseManager.instance.Resume();
    }

    void EndDelay()
    {
        timer = 0;
        onDelay = false;
        DoWhenEndDelay.Invoke();
    }

    private void Update()
    {
        if (onAnswers) return;

        if (onDelay)
        {
            timer += Time.deltaTime;

            if(timer >= delayTime)
            {
                EndDelay();
            }

            return;
        }

        if (!isOpen) return;

        if (Input.GetButtonDown("Submit"))
        {
            if (!inDialog) return;

            if (inAnimation)
            {
                StopAllCoroutines();
                EndAnimation();
                return;
            }

            inDialog = false;
            next?.Invoke(-1);
            presToNext.SetActive(false);
        }
    }
}

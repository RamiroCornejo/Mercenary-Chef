using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FoodResultUI : MonoBehaviour
{
    [SerializeField] Animator myAnimator = null;
    [SerializeField] Image resultImg = null;
    [SerializeField] Image starResult = null;
    [SerializeField] Sprite[] myStars = null;
    [SerializeField] TextMeshProUGUI resultName = null;
    [SerializeField] Image barResult = null;
    [SerializeField] TextMeshProUGUI textResult;
    [SerializeField] float barSpeed = 0.3f;
    [SerializeField] GameObject useButton = null;
    EventSystem eventSystem;
    BuffResult buff;
    [SerializeField] CanvasGroup myGroup;

    bool open;

    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        myGroup.alpha = 0;
        myGroup.blocksRaycasts = false;
        myGroup.interactable = false;
    }

    public void Open(RecipeResult result, float myScore, float maxScore, string resultString, int resulting)
    {

        open = true;
        myGroup.alpha = 1;
        myGroup.blocksRaycasts = true;
        myGroup.interactable = true;
        myAnimator.Play("Open");
        resultImg.sprite = result.Element_Image;
        resultName.text = result.Element_Name;
        starResult.sprite = myStars[resulting];

        if (result.GetType() == typeof(BuffResult))
        {
            buff = result as BuffResult;
            useButton.SetActive(true);
        }
        else
            useButton.SetActive(false);

        StartCoroutine(BarAnimation(myScore, maxScore, resultString));
    }

    IEnumerator BarAnimation(float myScore, float maxScore, string resultString)
    {
        float timer = 0;
        barResult.fillAmount = 0;
        textResult.text = "";
        while (timer < 1)
        {
            timer += Time.deltaTime * barSpeed;

            var lerp = Mathf.Lerp(0, myScore / maxScore, timer);

            barResult.fillAmount = lerp;

            yield return new WaitForEndOfFrame();
        }

        textResult.text = resultString;
    } 

    public void Close()
    {
        if (!open) return;

        open = false;
        Character.TrackInput(true);

        myAnimator.Play("Close");

        StopAllCoroutines();

        eventSystem.SetSelectedGameObject(null);
    }

    public void UseBuff()
    {
        if (!open) return;
        PlayerInventory.UseBuff(buff);
        Close();
    }
}
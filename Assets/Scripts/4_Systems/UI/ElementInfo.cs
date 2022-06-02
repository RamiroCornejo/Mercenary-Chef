using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ElementInfo : MonoBehaviour
{
    public static ElementInfo instance;
    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI title;
    public Image img;
    public TextMeshProUGUI description;
    public TextMeshProUGUI quality;
    public TextMeshProUGUI weight;

    [SerializeField] Color color_normal;
    [SerializeField] Color color_good;
    [SerializeField] Color color_Epic;

    CanvasGroup group;
    private void Start()
    {
        group = GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;
    }

    public static void Show(StackedPile stack) => instance.ShowInfo(stack);
    public static void Hide() => instance.HideInfo();

    void ShowInfo(StackedPile stack)
    {
        if (stack.IsEmpty) return;
        group.alpha = 1;
        title.text = stack.Element.Element_Name;
        img.sprite = stack.Element.Element_Image;
        description.text = stack.Element.Description;
        quality.color = stack.Quality == 1 ? color_normal : (stack.Quality == 2 ? color_good : color_Epic);
        quality.text = stack.Quality == 1 ? "normal" : (stack.Quality == 2 ? "muy buena" : "epica");
        weight.text = stack.Element.Weight.ToString();
    }

    void HideInfo()
    {
        group.alpha = 0;
    }
}


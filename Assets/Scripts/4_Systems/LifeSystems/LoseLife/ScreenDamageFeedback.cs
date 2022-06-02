using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenDamageFeedback : MonoBehaviour
{
    public static ScreenDamageFeedback instancia;

    public float cuantoTarda = 0.2f;
    public Image imgFeedbackPierdeVida;

    public Color ColorLastimado;
    public Color ColorTrasnparente;

    void Awake() { instancia = this; }

    public static void Hit() => instancia.HitFeedback();

    void HitFeedback()
    {
        imgFeedbackPierdeVida.color = ColorLastimado;
        StartCoroutine(Golpe());
    }

    IEnumerator Golpe()
    {
        float timer = 0.0f;
        while (timer <= 1.0f)
        {
            imgFeedbackPierdeVida.color = Color.Lerp(ColorLastimado, ColorTrasnparente, timer);
            timer += Time.deltaTime / cuantoTarda;
            yield return null;
        }
        imgFeedbackPierdeVida.color = ColorTrasnparente;
    }
}

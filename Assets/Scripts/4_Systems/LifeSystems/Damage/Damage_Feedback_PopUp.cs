using UnityEngine;
using TMPro;

public class Damage_Feedback_PopUp : MonoBehaviour
{
    public TextMeshProUGUI valor;
    public Animator anim_valor;

    public void PopUp(float damage)
    {
        valor.text = ((int)damage).ToString();
        anim_valor.Play("Anim_Rapida_Texto_PopPup");
    }
}

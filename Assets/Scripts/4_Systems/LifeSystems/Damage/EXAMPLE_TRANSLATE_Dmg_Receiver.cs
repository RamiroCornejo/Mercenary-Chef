using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Structs;
using TMPro;

public class EXAMPLE_TRANSLATE_Dmg_Receiver : MonoBehaviour
{
    Rigidbody myrig;
    public LifeComponent life;

    public TextMeshProUGUI valor;
    public Animator anim_valor;

    public void PhysicalDamage(float damage)
    {
        life.ReceiveDamage((int)damage);
        valor.text = ((int)damage).ToString();
        anim_valor.Play("Anim_Rapida_Texto_PopPup");
    }

    public void KnockBack(float owner_force, Vector3 owner_position)
    {
        myrig = GetComponent<Rigidbody>();
        if (myrig == null) return;

        var dir = this.transform.position - owner_position;
        dir.Normalize();

        myrig.AddForce(dir * owner_force, ForceMode.VelocityChange);
    }
}

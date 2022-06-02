using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeartComponent : MonoBehaviour
{
    [SerializeField] Image Bkg;
    [SerializeField] Image myImage;
    [SerializeField] ParticleSystem particle;
    [SerializeField] Animator anim;
    public float Background {
        set
        {
            //Bkg.fillAmount = value;//por fill amount mas simple

            //sino por graficos custom

            //if (value == 0f) { myImage.sprite = CharacterLifeFrontEnd.AppleBase; }
            //if (value == 0.25f) { Bkg.sprite = CharacterLifeFrontEnd.Apple025; }
            //if (value == 0.50f) { Bkg.sprite = CharacterLifeFrontEnd.Apple050; }
            //if (value == 0.75f) { Bkg.sprite = CharacterLifeFrontEnd.Apple075; }
            //if (value == 1f) { Bkg.sprite = CharacterLifeFrontEnd.Apple100; }
        }
    }


    float lastvalue;
    public float ImageValue
    {
        set
        {
            myImage.fillAmount = value;
            //if (value == 0f) { myImage.sprite = CharacterLifeFrontEnd.Apple000; }
            //if (value == 0.25f) { myImage.sprite = CharacterLifeFrontEnd.Apple025; }
            //if (value == 0.50f) { myImage.sprite = CharacterLifeFrontEnd.Apple050; }
            //if (value == 0.75f) { myImage.sprite = CharacterLifeFrontEnd.Apple075; }
            //if (value == 1f) { myImage.sprite = CharacterLifeFrontEnd.Apple100; }

            if (value != lastvalue)
            {
                //if (value > lastvalue)
                //{
                //    anim.Play("Gain_Life");
                //}
                //else
                //{
                //    anim.Play("Lose_Life");
                //}
                lastvalue = value;
            }
        }
    }

    public void PlayAddParticle()
    {
        particle.Play();
    }
}

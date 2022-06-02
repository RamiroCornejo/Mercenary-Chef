using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLifeFrontEnd : FrontendStatBase
{
    public static CharacterLifeFrontEnd instance;
    private void Awake() => instance = this;

    [SerializeField] UI_HeartComponent heart_model;
    List<UI_HeartComponent> hearts = new List<UI_HeartComponent>();
    [SerializeField] Transform parent_container;

    [Header("DataBase")]
    [SerializeField] Sprite sp_apple_base; public static Sprite AppleBase => instance.sp_apple_base;
    [SerializeField] Sprite sp_apple_000; public static Sprite Apple000 => instance.sp_apple_000;
    [SerializeField] Sprite sp_apple_025; public static Sprite Apple025 => instance.sp_apple_025;
    [SerializeField] Sprite sp_apple_050; public static Sprite Apple050 => instance.sp_apple_050;
    [SerializeField] Sprite sp_apple_075; public static Sprite Apple075 => instance.sp_apple_075;
    [SerializeField] Sprite sp_apple_100; public static Sprite Apple100 => instance.sp_apple_100;

    int current_value = -1;

    int divisions = 4; // por cuanto se devide los corazones
    int value_part = 5;//cada 1/4 de corazon vale este valor

    public override void OnValueChangeWithDelay(int value, float delay, int max = 100, bool anim = false) { }

    public override void OnValueChange(int value, int max = 100, bool anim = false)
    {

        #region Deprecated
        //if (current_value != max)
        //{
        //    for (int i = 0; i < hearts.Count; i++)
        //    {
        //        GameObject.Destroy(hearts[i].gameObject);
        //    }
        //    hearts.Clear();

        //    for (int i = 0; i < max; i++)
        //    {
        //        var heart = GameObject.Instantiate(heart_model, parent_container);
        //        hearts.Add(heart);
        //    }
        //    //refresh completo
        //}
        //else
        //{
        //    //refresh cantidad
        //}
        #endregion

        if (current_value != max) //si el maximo cambió
        {

            #region Limpio la lista
            for (int i = 0; i < hearts.Count; i++)
            {
                GameObject.Destroy(hearts[i].gameObject);
            }
            hearts.Clear();
            #endregion

            #region Draw Background
            float bkg_raw_iterations = (float)max / (divisions * value_part); //calculo en float las iteraciones

            int bkg_iterations = Mathf.FloorToInt(bkg_raw_iterations); //lo flooreo para pasarlo a int
            float bkg_remain = bkg_raw_iterations - bkg_iterations; //capturo el restito que me quedó
            if (bkg_remain > 0) bkg_iterations = bkg_iterations + 1; //si tengo un resto le sumo +1 al iterador
            for (int i = 0; i < bkg_iterations; i++)
            {
                var heart = GameObject.Instantiate(heart_model, parent_container);
                heart.Background = 1;
                heart.ImageValue = 0;
                if (i == bkg_iterations - 1)
                {
                    if (bkg_remain > 0)
                    {
                        heart.Background = bkg_remain;
                    }
                    heart.PlayAddParticle();
                }
                hearts.Add(heart);
            }
            #endregion

            current_value = max;
            //Debug.Log("Cambio el maximo");
        }

        for (int i = 0; i < hearts.Count; i++) hearts[i].ImageValue = 0;

        #region Draw Value
        Debug.Log(value);
        float value_raw_iterations = (float)value / (divisions * value_part); //calculo en float las iteraciones
        int value_iterations = Mathf.FloorToInt(value_raw_iterations); //lo flooreo para pasarlo a int
        float value_remain = value_raw_iterations - value_iterations; //capturo el restito que me quedó
        if (value_remain > 0) value_iterations = value_iterations + 1; //si tengo un resto le sumo +1 al iterador
        for (int i = 0; i < value_iterations; i++)
        {
            hearts[i].ImageValue = 1;
            if (value_remain > 0 && i == value_iterations - 1)
            {
                //Debug.Log("Es el ultimo, value remain es: " + value_remain);
                hearts[i].ImageValue = value_remain;
            }
        }
        #endregion
    }

}

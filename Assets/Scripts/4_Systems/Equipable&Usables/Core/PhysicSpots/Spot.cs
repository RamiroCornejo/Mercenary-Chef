using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//esto va a ir en el parent de donde vamos a instanciar el equipable
//lo vamos a necesitar para hacer las busquedas, ya que
public class Spot : MonoBehaviour
{
    public SpotType spotType;
    public Transform spotparent;

    #region Current Object Logic
    GameObject current;
    public bool ContainsSomething => current != null;
    public GameObject Current => current;
    public void Equip_Visuals(GameObject _model)
    {
        if (current == null)
        {
            GameObject go = Instantiate(_model, spotparent);
            current = go;
        }
        else
        {
            if (current != _model)
            {
                DestroyImmediate(current);
                GameObject go = Instantiate(_model, spotparent);
                current = go;
            }
        }
    }
    public void Empty()
    {
        DestroyImmediate(current);
        current = null;
    }
    #endregion
}

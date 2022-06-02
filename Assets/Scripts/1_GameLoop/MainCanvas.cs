using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] Transform parent;

    public T InstanceNewObject<T>(GameObject _model)
    {
        GameObject newObject = GameObject.Instantiate(_model, parent);
        var aux = newObject.GetComponent<T>();
        if (aux == null) throw new System.Exception("Error, el objeto instanciado no contiene este tipo de dato");
        else return aux;
    }
}

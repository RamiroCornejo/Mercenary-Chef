using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake() { if (instance == null) instance = this; else DestroyImmediate(this); }

    [SerializeField] Character character;
    public static Character Character { get => instance.character; }

    [SerializeField] MainCanvas mainCanvas;
    public static MainCanvas MainCanvas { get => instance.mainCanvas; }
}

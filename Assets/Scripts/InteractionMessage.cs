using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMessage : MonoBehaviour
{
    public static InteractionMessage Instance { get; private set; }
    Text _text;
    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            _text = GetComponent<Text>();
        }
    }
    public void WriteText(string text)
    {
        if (Instance)
        {
            _text.text = text;
        }
        else
        {
            Debug.LogWarning($"{nameof(InteractionMessage)}がシーン上に無いので変更を加えることができません");
        }
    }
}

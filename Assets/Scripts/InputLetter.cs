using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InputLetter : MonoBehaviour
{
    InputField _inputField;
    void Start()
    {
        _inputField = GetComponent<InputField>();
        _inputField.OnEndEditAsObservable().Subscribe(_ => InteractionMessage.Instance.WriteText(_inputField.text));
    }
}

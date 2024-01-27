using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class InputLetter : MonoBehaviour
    {
        InputField _inputField;
        void Start()
        {
            _inputField = GetComponent<InputField>();
            _inputField.OnEndEditAsObservable().Subscribe(_ => InteractionMessage.Instance.WriteText(_inputField.text));
        }
    }
}

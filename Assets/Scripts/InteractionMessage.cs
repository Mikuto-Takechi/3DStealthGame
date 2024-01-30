using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class InteractionMessage : MonoBehaviour
    {
        public static InteractionMessage Instance { get; private set; }
        Text _text;
        ContentSizeFitter _csf;
        void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
                _csf = gameObject.GetComponent<ContentSizeFitter>();
                _text = GetComponent<Text>();
            }
        }
        public void WriteText(string text)
        {
            if (Instance)
            {
                _text.text = text;
                _csf.SetLayoutHorizontal();
            }
            else
            {
                Debug.LogWarning($"{nameof(InteractionMessage)}がシーン上に無いので変更を加えることができません");
            }
        }
    }
}

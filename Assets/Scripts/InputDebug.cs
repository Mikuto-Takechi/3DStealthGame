using UnityEngine;

namespace MonstersDomain
{
    public class InputDebug : MonoBehaviour
    {
        [ContextMenu("Bind")]
        void Bind()
        {
            InputProvider.Instance.BindCharacterInput();
            InputProvider.Instance.BindGUIInput();
        }
        [ContextMenu("UnBind")]
        void UnBind()
        {
            InputProvider.Instance.UnBindCharacterInput();
            InputProvider.Instance.UnBindGUIInput();
        }
    }
}

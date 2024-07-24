using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// スカイボックスを回転させるためのクラス。
    /// </summary>
    public class RotateSkyBox : MonoBehaviour
    {
        /// <summary>回転速度</summary>
        [SerializeField] float _rotateSpeed = 1.5f;
        float _rot;

        void Update()
        {
            _rot += Time.deltaTime * _rotateSpeed;
            if (_rot >= 360.0f) // 0～360°の範囲におさめたい
                _rot -= 360.0f;
            RenderSettings.skybox.SetFloat("_Rotation", _rot); // 回す
        }
    }
}
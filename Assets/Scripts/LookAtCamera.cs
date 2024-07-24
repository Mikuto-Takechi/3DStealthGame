using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// 自身をカメラの方向に向かせるクラス。(ビルボード)
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] float _lookSpeed = 3f;
        Camera _main;

        void Awake()
        {
            _main = Camera.main;
        }

        void Update()
        {
            transform.rotation =
                Quaternion.Slerp(transform.rotation, _main.transform.rotation, _lookSpeed * Time.deltaTime);
        }
    }
}
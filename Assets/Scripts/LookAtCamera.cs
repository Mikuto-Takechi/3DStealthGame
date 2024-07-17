using UnityEngine;

namespace MonstersDomain
{
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
using Cinemachine;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// プレイヤーの視点を管理するクラス。
    /// </summary>
    public class FirstPersonViewController : MonoBehaviour
    {
        static readonly int Dance = Animator.StringToHash("Dance");
        [SerializeField] CinemachineVirtualCamera _virtualCamera;
        [SerializeField] GameObject _head;
        [SerializeField, Tooltip("縦方向の角度の最大値")] float _maxVertical = 90f;
        [SerializeField, Tooltip("縦方向の角度の最小値")] float _minVertical = -90f;
        [SerializeField, Tooltip("一人称視点時に表示される腕のアニメーター")] Animator _armsAnimator;
        [SerializeField, Tooltip("fps視点のオブジェクトのレイヤー")] LayerMask _fpsLayers = 1 << 7 | 1 << 11;
        CinemachineBasicMultiChannelPerlin _headBob;
        [SerializeField, Tooltip("横方向の視点移動の感度")] float _xSensitivity = 3f;
        [SerializeField, Tooltip("縦方向の視点移動の感度")] float _ySensitivity = 3f;
        Vector3 _headEulerAngle, _bodyEulerAngle;
        public Transform Head => _head.transform;
        public bool FreePov { get; set; } = true;
        void Awake()
        {
            _bodyEulerAngle = transform.localRotation.eulerAngles;
            _headEulerAngle = _head.transform.localRotation.eulerAngles;
            _headBob = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            InputProvider.Instance.DanceTrigger.Subscribe(_ => _armsAnimator.SetTrigger(Dance)).AddTo(this);
        }
        
        void Update()
        {
            if (FreePov)
            {
                var xRot = Input.GetAxis("Mouse X") * _xSensitivity;
                var yRot = Input.GetAxis("Mouse Y") * _ySensitivity;

                _headEulerAngle.x -= yRot;
                _headEulerAngle.x = Mathf.Clamp(_headEulerAngle.x, _minVertical, _maxVertical);
                _bodyEulerAngle.y += xRot;
                _head.transform.localRotation = Quaternion.Euler(_headEulerAngle);
                transform.localRotation = Quaternion.Euler(_bodyEulerAngle);
            }
        }

        /// <summary>POVの角度を外部から設定する</summary>
        public void SetRotation(float xRot, float yRot)
        {
            _headEulerAngle.x = xRot;
            _headEulerAngle.x = Mathf.Clamp(_headEulerAngle.x, _minVertical, _maxVertical);
            _bodyEulerAngle.y = yRot;
            _head.transform.localRotation = Quaternion.Euler(_headEulerAngle);
            transform.localRotation = Quaternion.Euler(_bodyEulerAngle);
        }

        void SetHeadBob(float amplitudeGain, float frequencyGain)
        {
            _headBob.m_AmplitudeGain = amplitudeGain;
            _headBob.m_FrequencyGain = frequencyGain;
        }
        /// <summary>
        /// 1人称視点のオブジェクトの表示を許可・禁止する。
        /// </summary>
        public void AllowFirstPersonViewObjects(bool condition)
        {
            if (condition)
            {
                //  FPS視点ののレイヤーをカメラのマスクに追加する
                Camera.main.cullingMask |= _fpsLayers;
            }
            else
            {
                //  FPS視点ののレイヤーをカメラのマスクから除外する
                Camera.main.cullingMask &= ~_fpsLayers;
            }
        }

        public void UpdateView(PlayerState state, bool isMove, bool isGrounded)
        {
            if (!isGrounded)
            {
                SetHeadBob(0f, 0f);
                return;
            }

            if (isMove)
            {
                _armsAnimator.SetInteger("MovingLevel", 1);
                if (state == PlayerState.Crouch)
                {
                    SetHeadBob(0.25f, 0.5f);
                }
                else if (state == PlayerState.Walk)
                {
                    SetHeadBob(0.5f, 0.75f);
                }
                else if (state == PlayerState.Run)
                {
                    SetHeadBob(1.4f, 1f);
                    _armsAnimator.SetInteger("MovingLevel", 2);
                }
            }
            else
            {
                SetHeadBob(0.25f, 0.5f);
                _armsAnimator.SetInteger("MovingLevel", 0);
            }
        }
    }
}
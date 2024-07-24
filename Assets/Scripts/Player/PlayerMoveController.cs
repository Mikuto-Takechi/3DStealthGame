using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField, Tooltip("移動速度")]  float _moveSpeed = 5f;
        [SerializeField, Tooltip("ジャンプする力")]  float _jumpPower = 5f;
        [SerializeField, Tooltip("しゃがみ時に計算に加える除数")] float _crouchingSpeedDivisor = 3f;
        [SerializeField, Tooltip("ダッシュ時に計算に加える乗数")] float _dashSpeedMultiplier = 3f;
        [SerializeField, Tooltip("走るために必要なスタミナ")] float _maxStamina = 20f;
        [SerializeField, Tooltip("走った時のスタミナ減少量")] float _decreaseStamina = 3f;
        [SerializeField, Tooltip("スタミナ回復量")]  float _recoveryStamina = 3f;
        [SerializeField, Tooltip("スタミナゲージを表示するUI")] ShrinkBar _staminaBar;
        Rigidbody _rb;
        float _currentMoveSpeed;
        float _currentStamina;
        float _jumpTimer;
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _currentStamina = _maxStamina;
        }
        /// <summary>
        /// 移動を許可・禁止する。
        /// </summary>
        public void AllowMovement(bool condition)
        {
            if (condition)
            {
                //  Rigidbodyによる回転だけ止める
                _rb.constraints = RigidbodyConstraints.FreezeRotation;

            }
            else
            {
                //  Rigidbodyによる移動を完全に止める
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        public void RecoveryStamina()
        {
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _recoveryStamina * Time.deltaTime;
                _staminaBar.SetFill(_maxStamina, _currentStamina);
            }
        }
        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public void Jumping()
        {
            //  ジャンプ直後の0.1秒は接地していないことにする。
            _jumpTimer = 0.1f;
            var velocity = _rb.velocity;
            velocity.y = _jumpPower;
            _rb.velocity = velocity;
        }
        /// <summary>
        /// 平面移動処理
        /// </summary>
        public void UpdateMovement(ReactiveProperty<PlayerState> state, bool isMove, Vector3 inputVector, CheckGround checkGround)
        {
            _jumpTimer = Mathf.Clamp(_jumpTimer - Time.deltaTime, 0, float.MaxValue);
            if (isMove) //  移動量が0より大きかったらカメラの揺れを大きくする
            {
                if (state.Value == PlayerState.Crouch)
                {
                    _currentMoveSpeed = _moveSpeed / _crouchingSpeedDivisor;
                }
                else if (state.Value == PlayerState.Run && _currentStamina > 0)
                {
                    _currentMoveSpeed = _moveSpeed * _dashSpeedMultiplier;
                    _currentStamina -= _decreaseStamina * Time.deltaTime;
                    _staminaBar.SetFill(_maxStamina, _currentStamina);
                }
                else
                {
                    state.Value = PlayerState.Walk;
                    _currentMoveSpeed = _moveSpeed;
                }
            }
            else
            {
                if (state.Value != PlayerState.Crouch)
                    state.Value = PlayerState.Idle;
            }
            
            //  入力ベクトルをプレイヤーの向きに合わせる。
            inputVector = transform.TransformDirection(inputVector);
            if (checkGround.IsGrounded.Value && _jumpTimer <= 0) //  接地しているなら法線ベクトルで地形に沿ったベクトルを出す
            {
                _rb.velocity = Vector3.ProjectOnPlane(inputVector, checkGround.NormalVector) * _currentMoveSpeed;
            }
            else
            {
                inputVector *= _currentMoveSpeed;
                inputVector.y = _rb.velocity.y;
                _rb.velocity = inputVector;
            }
        }
    }
}
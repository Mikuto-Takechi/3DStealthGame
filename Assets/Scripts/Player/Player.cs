using System;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class Player : MonoBehaviour
    {
        static readonly int IsWalking = Animator.StringToHash("IsWalking");
        static readonly int Dance = Animator.StringToHash("Dance");
        [SerializeField, Tooltip("移動速度")]  float _moveSpeed = 5f;
        [SerializeField, Tooltip("ジャンプする力")]  float _jumpPower = 5f;
        [SerializeField, Tooltip("しゃがみ時に計算に加える除数")] float _crouchingSpeedDivisor = 3f;
        [SerializeField, Tooltip("ダッシュ時に計算に加える乗数")] float _dashSpeedMultiplier = 3f;
        [SerializeField, Tooltip("走るために必要なスタミナ")] float _maxStamina = 20f;
        [SerializeField, Tooltip("走った時のスタミナ源少量")] float _decreaseStamina = 3f;
        [SerializeField, Tooltip("スタミナ回復量")]  float _recoveryStamina = 3f;
        [SerializeField, Tooltip("接地判定")]  CheckGround _checkGround;
        [SerializeField, Tooltip("視点管理")]  PovController _povController;
        [SerializeField, Tooltip("一人称視点時に表示される腕のアニメーター")] Animator _armsAnimator;
        [SerializeField, Tooltip("立っている時にアクティブになるコライダー")] Collider _bodyCollider;
        [SerializeField, Tooltip("レイキャストから除外するレイヤー")] LayerMask _ignoreLayer;
        [SerializeField, Tooltip("しゃがみ時に立ち上がれる高さかを確認するための距離")] float _checkCeilingDistance = 1.9f;
        [SerializeField, Tooltip("スタミナゲージを表示するUI")] ShrinkBar _staminaBar;
        [SerializeField, Tooltip("レイを飛ばす場所")]  Transform _stepRayUpper;
        [SerializeField, Tooltip("レイを飛ばす高さ")]  float _stepHeight = 0.3f;
        [SerializeField, Tooltip("段差を飛ばす距離")]  float _stepSmooth = 0.1f;
        [SerializeField, Tooltip("しゃがみ歩き時の音の距離、半径")] float _crouchSoundDistance = 1;
        [SerializeField, Tooltip("歩き時の音の距離、半径")] float _walkSoundDistance = 5;
        [SerializeField, Tooltip("走り時の音の距離、半径")] float _runSoundDistance = 15;
        [SerializeField, Tooltip("しゃがみ音の間隔")] float _crouchInterval = 0.2f;
        [SerializeField, Tooltip("歩行音の間隔")] float _walkInterval = 0.2f;
        [SerializeField, Tooltip("ダッシュ音の間隔")] float _runInterval = 0.2f;
        readonly ReactiveProperty<Vector3> _updatePosition = new();
        public readonly ReactiveProperty<PlayerState> State = new(PlayerState.Idle);
        float _currentStamina;
        float _jumpTimer;
        float _footStepsTimer;
        IDisposable _disposable;
        FootStepsSource _footStepsSource;
        CinemachineBasicMultiChannelPerlin _headBob;
        Tween _headTween;
        bool _isPaused;
        Rigidbody _rb;
        public PovController PovController => _povController;
        public bool IsDied { get; set; }

        void Awake()
        {
            _footStepsSource = GetComponent<FootStepsSource>();
            State.SkipLatestValueOnSubscribe().Subscribe(Hiding).AddTo(this);
            _rb = GetComponent<Rigidbody>();
            _headBob = PovController.VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _currentStamina = _maxStamina;
            InputProvider.Instance.JumpTrigger.Where(_ => _checkGround.IsGrounded.Value).Subscribe(Jumping).AddTo(this);
            InputProvider.Instance.CrouchSwitch.Subscribe(Crouching).AddTo(this);
            InputProvider.Instance.RunTrigger.Subscribe(_ =>
            {
                if (State.Value == PlayerState.Walk)
                    State.Value = PlayerState.Run;
                else if (State.Value == PlayerState.Run)
                    State.Value = PlayerState.Walk;
            }).AddTo(this);
            InputProvider.Instance.DanceTrigger.Subscribe(_ => _armsAnimator.SetTrigger(Dance)).AddTo(this);
            _stepRayUpper.transform.localPosition = new Vector3(0, _stepHeight, 0);
            _checkGround.HitWall += StepClimb;
            _checkGround.IsGrounded.Subscribe(flag =>
            {
                if (flag)
                {
                    _footStepsSource.PlayLandingSE(_checkGround.HitGameObject, 1, 1);
                }
            }).AddTo(this);
        }

        void Start()
        {
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void Update()
        {
            _jumpTimer = Mathf.Clamp(_jumpTimer - Time.deltaTime, 0, float.MaxValue);
            switch (State.Value)
            {
                case PlayerState.Crouch:
                    if (_footStepsTimer <= _crouchInterval) break;
                    _footStepsTimer = 0f;
                    _footStepsSource.PlayFootSteps(_checkGround.HitGameObject, 0.8f, 1f);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _crouchSoundDistance);
                    break;
                case PlayerState.Walk:
                    if (_footStepsTimer <= _walkInterval) break;
                    _footStepsTimer = 0f;
                    _footStepsSource.PlayFootSteps(_checkGround.HitGameObject, 1, 1);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _walkSoundDistance);
                    break;
                case PlayerState.Run:
                    if (_footStepsTimer <= _runInterval) break;
                    _footStepsTimer = 0f;
                    _footStepsSource.PlayFootSteps(_checkGround.HitGameObject, 1.4f, 1);
                    HearingManager.Instance.OnSoundEmitted(transform.position, _runSoundDistance);
                    break;
            }
            if (_isPaused) return;
            if (State.Value == PlayerState.Hide)
                RecoveryStamina();
            else
                Movement();
            //  座標更新を発行する
            _updatePosition.Value = transform.position;
            AreaManager.Instance.UpdatePlayerLocation(transform.position);
        }

        void OnDisable()
        {
            _checkGround.HitWall -= StepClimb;
            GameManager.Instance.OnPause -= OnPause;
            GameManager.Instance.OnResume -= OnResume;
        }

        void OnPause()
        {
            _povController.FreePov = false;
            _isPaused = true;
        }

        void OnResume()
        {
            _povController.FreePov = true;
            _isPaused = false;
        }

        void Hiding(PlayerState playerState)
        {
            if (playerState == PlayerState.Hide)
            {
                //  Rigid bodyによる移動を完全に止める
                _rb.constraints = RigidbodyConstraints.FreezeAll;
                //  FPS視点ののレイヤーをカメラのマスクから除外する
                Camera.main.cullingMask &= ~(1 << 7 | 1 << 11);
            }
            else
            {
                //  Rigid bodyによる回転だけ止める
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
                //  FPS視点ののレイヤーをカメラのマスクに追加する
                Camera.main.cullingMask |= 1 << 7 | 1 << 11;
            }
        }

        /// <summary>
        ///     しゃがみ状態の切り替え
        /// </summary>
        void Crouching(bool b)
        {
            if (b) //  しゃがみ状態に入る
            {
                State.Value = PlayerState.Crouch;
                _headTween?.Kill();
                _headTween = _povController.Head.DOLocalMoveY(1, 0.1f).OnComplete(() => _bodyCollider.isTrigger = true)
                    .SetLink(gameObject);
            }
            else if (_disposable == null) //  しゃがみ状態解除待機開始
            {
                _disposable = _updatePosition.SkipLatestValueOnSubscribe()
                    .Where(_ => !Physics.SphereCast(new Ray(transform.position, Vector3.up), 0.3f,
                        _checkCeilingDistance, ~_ignoreLayer))
                    .First().Subscribe(_ =>
                    {
                        _disposable = null;
                        State.Value = PlayerState.Idle;
                        _headTween?.Kill();
                        _headTween = _povController.Head.DOLocalMoveY(1.6f, 0.1f)
                            .OnComplete(() => _bodyCollider.isTrigger = false).SetLink(gameObject);
                    }).AddTo(this);
                //  一度だけ値を変えずに_updatePositionのOnNext()を呼ぶ
                _updatePosition.SetValueAndForceNotify(_updatePosition.Value);
            }
        }

        /// <summary>
        ///     ジャンプ処理
        /// </summary>
        void Jumping(Unit _)
        {
            _jumpTimer = 0.1f;
            var velo = _rb.velocity;
            velo.y = _jumpPower;
            _rb.velocity = velo;
            _footStepsSource.PlayJumpingSE(_checkGround.HitGameObject, 1, 1);
        }

        /// <summary>
        ///     平面移動処理
        /// </summary>
        void Movement()
        {
            var inputVector = InputProvider.Instance.MoveDirection;
            inputVector = transform.TransformDirection(inputVector);
            //inputVector = inputVector.normalized;
            var isMoving = inputVector.sqrMagnitude > 0;
            var moveSpeed = _moveSpeed;
            if (isMoving) //  移動量が0より大きかったらカメラの揺れを大きくする
            {
                if (State.Value == PlayerState.Crouch)
                {
                    _headBob.m_AmplitudeGain = 0.25f;
                    _headBob.m_FrequencyGain = 0.5f;
                    _armsAnimator.SetInteger("MovingLevel", 1);
                    RecoveryStamina();
                    moveSpeed /= _crouchingSpeedDivisor;
                }
                else if (State.Value == PlayerState.Run && _currentStamina > 0)
                {
                    _headBob.m_AmplitudeGain = 1.4f;
                    _headBob.m_FrequencyGain = 1f;
                    _armsAnimator.SetInteger("MovingLevel", 2);
                    moveSpeed *= _dashSpeedMultiplier;
                    _currentStamina -= _decreaseStamina * Time.deltaTime;
                    _staminaBar.SetFill(_maxStamina, _currentStamina);
                }
                else
                {
                    _headBob.m_AmplitudeGain = 0.5f;
                    _headBob.m_FrequencyGain = 0.75f;
                    _armsAnimator.SetInteger("MovingLevel", 1);
                    RecoveryStamina();
                    State.Value = PlayerState.Walk;
                }
                _headBob.m_NoiseProfile.GetSignal(Time.time, out var pos, out _);
                if(_checkGround.IsGrounded.Value) _footStepsTimer += Time.deltaTime;
            }
            else
            {
                _headBob.m_AmplitudeGain = 0.25f;
                _headBob.m_FrequencyGain = 0.5f;
                _armsAnimator.SetInteger("MovingLevel", 0);
                if (State.Value != PlayerState.Crouch) State.Value = PlayerState.Idle;
                RecoveryStamina();
                _footStepsTimer = 0f;
            }
            if (_checkGround.IsGrounded.Value && _jumpTimer <= 0) //  接地しているなら法線ベクトルで地形に沿ったベクトルを出す
            {
                var dir = Vector3.ProjectOnPlane(inputVector, _checkGround.NormalVector);
                _rb.velocity = dir * moveSpeed;
            }
            else
            {
                inputVector *= moveSpeed;
                inputVector.y = _rb.velocity.y;
                _rb.velocity = inputVector;
            }

            if (!_checkGround.IsGrounded.Value)
            {
                _headBob.m_AmplitudeGain = 0f;
                _headBob.m_FrequencyGain = 0f;
            }
        }

        void RecoveryStamina()
        {
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _recoveryStamina * Time.deltaTime;
                _staminaBar.SetFill(_maxStamina, _currentStamina);
            }
        }

        void StepClimb()
        {
            RaycastHit hitLower;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitLower,
                    0.6f, ~_ignoreLayer))
            {
                RaycastHit hitUpper;
                if (!Physics.Raycast(_stepRayUpper.position, transform.TransformDirection(Vector3.forward),
                        out hitUpper,
                        0.7f, ~_ignoreLayer))
                    _rb.position += new Vector3(0, _stepSmooth, 0);
            }
        }
    }

    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Crouch,
        Hide
    }
}
using System;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Player : MonoBehaviour
{
    static readonly int IsWalking = Animator.StringToHash("IsWalking");
    static readonly int Dance = Animator.StringToHash("Dance");
    [SerializeField,Tooltip("移動速度")] float _moveSpeed = 5f;
    [SerializeField,Tooltip("ジャンプする力")] float _jumpPower = 5f;
    [SerializeField, Tooltip("しゃがみ時に計算に加える除数")] float _crouchingSpeedDivisor = 3f;
    [SerializeField, Tooltip("ダッシュ時に計算に加える乗数")] float _dashSpeedMultiplier = 3f;
    [SerializeField, Tooltip("走るために必要なスタミナ")] float _maxStamina = 20f;
    [SerializeField, Tooltip("走った時のスタミナ源少量")] float _decreaseStamina = 3f;
    [SerializeField,Tooltip("接地判定")] CheckGround _checkGround;
    [SerializeField,Tooltip("視点管理")] PovController _povController;
    [SerializeField,Tooltip("一人称視点時に表示される腕のアニメーター")] Animator _armsAnimator;
    [SerializeField,Tooltip("立っている時にアクティブになるコライダー")] Collider _bodyCollider;
    [SerializeField,Tooltip("レイキャストから除外するレイヤー")] LayerMask _layerMask;
    [SerializeField,Tooltip("しゃがみ時に立ち上がれる高さかを確認するための距離")] float _checkCeilingDistance = 1.9f;

    readonly IntReactiveProperty _footSteps = new();
    public readonly ReactiveProperty<PlayerState> State = new(PlayerState.Idle);
    IDisposable _disposable;
    CinemachineBasicMultiChannelPerlin _headBob;
    Tween _headTween;
    Rigidbody _rb;
    public PovController PovController => _povController;
    bool _isDashed = false;
    float _currentStamina = 0;

    void Awake()
    {
        _footSteps.Where(n => n < 0).Subscribe(_ => AudioManager.Instance.PlayFootSteps(FootSteps.Player)).AddTo(this);
        State.SkipLatestValueOnSubscribe().Where(s => s == PlayerState.Hide).Subscribe(_ => Hiding()).AddTo(this);
        State.SkipLatestValueOnSubscribe().Where(s => s != PlayerState.Hide).Subscribe(_ => StopHiding()).AddTo(this);
        _rb = GetComponent<Rigidbody>();
        _headBob = PovController.VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _currentStamina = _maxStamina;
    }

    void Update()
    {
        if (Input.GetButtonDown("Dance")) _armsAnimator.SetTrigger(Dance);
        if (Input.GetButtonDown("Dash"))
            _isDashed = !_isDashed;
        if (State.Value != PlayerState.Hide) //  隠れているならRigid bodyによる移動を完全に止める
            Movement();
    }

    void Hiding()
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        //  FPS視点ののレイヤーをカメラのマスクから除外する
        Camera.main.cullingMask &= ~(1 << 7);
    }

    void StopHiding()
    {
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        //  FPS視点ののレイヤーをカメラのマスクに追加する
        Camera.main.cullingMask |= 1 << 7;
    }

    void Crouching()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            State.Value = PlayerState.Crouch;
            _headTween?.Kill();
            _headTween = _povController.Head.DOLocalMoveY(1, 0.1f).OnComplete(() => _bodyCollider.isTrigger = true)
                .SetLink(gameObject);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            if (_disposable == null)
                _disposable = this.FixedUpdateAsObservable()
                    .Where(_ => !Physics.SphereCast(new Ray(transform.position, Vector3.up), 0.3f,
                        _checkCeilingDistance, ~_layerMask))
                    .First().Subscribe(_ =>
                    {
                        _disposable = null;
                        State.Value = PlayerState.Idle;
                        _headTween?.Kill();
                        _headTween = _povController.Head.DOLocalMoveY(1.6f, 0.1f)
                            .OnComplete(() => _bodyCollider.isTrigger = false).SetLink(gameObject);
                    }).AddTo(this);
        }
    }

    void Movement()
    {
        Crouching();
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var inputVector = new Vector3(horizontal, 0, vertical);
        var isWalking = inputVector.magnitude > 0;
        if (isWalking) //  移動量が0より大きかったらカメラの揺れを大きくする
        {
            var moveSpeed = _moveSpeed;
            if (State.Value == PlayerState.Crouch)
            {
                moveSpeed /= _crouchingSpeedDivisor;
            }
            else if (_isDashed && _currentStamina > 0)
            {
                moveSpeed *= _dashSpeedMultiplier;
                _currentStamina -= _decreaseStamina * Time.deltaTime;
            }

            inputVector.x *= moveSpeed;
            inputVector.z *= moveSpeed;
            _headBob.m_AmplitudeGain = 1;
            _headBob.m_FrequencyGain = 1;
            _headBob.m_NoiseProfile.GetSignal(Time.time, out var pos, out _);
            _footSteps.Value = Math.Sign(pos.y) * 1;
            _armsAnimator.SetBool(IsWalking, true);
        }
        else
        {
            _headBob.m_AmplitudeGain = 0.25f;
            _headBob.m_FrequencyGain = 0.5f;
            _armsAnimator.SetBool(IsWalking, false);
        }

        inputVector = transform.TransformDirection(inputVector); //  ベクトルを自分の向きに合わせる
        inputVector.y = _rb.velocity.y;
        if (_checkGround.IsGrounded) //  接地しているなら法線ベクトルで地形に沿ったベクトルを出す
        {
            var inputMagnitudeOnNormal = Vector3.Dot(inputVector, _checkGround.NormalVector);
            var onNormal = inputVector - inputMagnitudeOnNormal * _checkGround.NormalVector;
            var onPlane = inputVector - onNormal;
            _rb.velocity = onPlane + onNormal;
            if (Input.GetButtonDown("Jump"))
            {
                _checkGround.IsGrounded = false;
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }
        }
        else
        {
            inputVector.y = _rb.velocity.y;
            _rb.velocity = inputVector;
        }
    }
}

public enum PlayerState
{
    Idle,
    Crouch,
    Hide
}
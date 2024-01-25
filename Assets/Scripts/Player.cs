using System;
using Cinemachine;
using UniRx;
using UnityEngine;

public class Player : MonoBehaviour
{
    static readonly int IsWalking = Animator.StringToHash("IsWalking");
    static readonly int Dance = Animator.StringToHash("Dance");
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] CheckGround _checkGround;
    [SerializeField] PovController _povController;
    [SerializeField] Animator _armsAnimator;
    readonly IntReactiveProperty _footSteps = new();
    public readonly ReactiveProperty<PlayerState> State = new(PlayerState.Idle);
    CinemachineBasicMultiChannelPerlin _headBob;
    Rigidbody _rb;
    public PovController PovController => _povController;

    void Awake()
    {
        _footSteps.Where(n => n < 0).Subscribe(_ => AudioManager.Instance.PlayFootSteps(FootSteps.Player)).AddTo(this);
        State.SkipLatestValueOnSubscribe().Where(s => s == PlayerState.Hide).Subscribe(_ => Hiding()).AddTo(this);
        State.SkipLatestValueOnSubscribe().Where(s => s != PlayerState.Hide).Subscribe(_ => StopHiding()).AddTo(this);
        _rb = GetComponent<Rigidbody>();
        _headBob = PovController.VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Dance")) _armsAnimator.SetTrigger(Dance);
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

    void Movement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal") * _moveSpeed;
        var vertical = Input.GetAxisRaw("Vertical") * _moveSpeed;
        var inputVector = new Vector3(horizontal, 0, vertical);
        var isWalking = inputVector.magnitude > 0;
        if (isWalking) //  移動量が0より大きかったらカメラの揺れを大きくする
        {
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
    Walk,
    Hide
}
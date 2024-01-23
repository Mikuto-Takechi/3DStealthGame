using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] CheckGround _checkGround;
    [SerializeField] POVController _povController;
    [SerializeField] Animator _armsAnimator;
    public POVController POVController { get { return _povController; } }
    Rigidbody _rb;
    float _totalFallTime = 0f;
    public PlayerState State = PlayerState.Idle;
    CinemachineBasicMultiChannelPerlin _headBob;
    IntReactiveProperty _footSteps = new();
    void Awake()
    {
        _footSteps.Where(n => n < 0).Subscribe(_ => AudioManager.Instance.PlayFootStep()).AddTo(this);
        _rb = GetComponent<Rigidbody>();
        _headBob = POVController.VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {
        if(State == PlayerState.Hide)   //  �B��Ă���Ȃ�Rigidbody�ɂ��ړ������S�Ɏ~�߂�
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            Movement();
        }
    }
    void Movement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal") * _moveSpeed;
        var vertical = Input.GetAxisRaw("Vertical") * _moveSpeed;
        Vector3 inputVector = new Vector3(horizontal, 0, vertical);
        bool isWalking = inputVector.magnitude > 0;
        if (isWalking)   //  �ړ��ʂ�0���傫��������J�����̗h���傫������
        {
            _headBob.m_AmplitudeGain = 1;
            _headBob.m_FrequencyGain = 1;
            _headBob.m_NoiseProfile.GetSignal(Time.time, out Vector3 pos, out Quaternion rot);
            _footSteps.Value = Math.Sign(pos.y) * 1;
            _armsAnimator.SetBool("IsWalking", isWalking);
        }
        else
        {
            _headBob.m_AmplitudeGain = 0.25f;
            _headBob.m_FrequencyGain = 0.5f;
            _armsAnimator.SetBool("IsWalking", isWalking);
        }
        inputVector = transform.TransformDirection(inputVector);    //  �x�N�g���������̌����ɍ��킹��
        if (_checkGround.IsGrounded)    //  �ڒn���Ă���Ȃ�@���x�N�g���Œn�`�ɉ������x�N�g�����o��
        {
            _totalFallTime = 0;
            var onPlane = Vector3.ProjectOnPlane(inputVector, _checkGround.NormalVector);
            _rb.velocity = onPlane;
        }
        else    //  �ڒn���Ă��Ȃ����y����velocity�����X�ɑ��₷
        {
            _totalFallTime += Time.deltaTime;
            inputVector.y = _gravity * _totalFallTime;
            _rb.velocity = inputVector;
        }
    }
}
public enum PlayerState
{
    Idle,
    Run,
    Hide,
}
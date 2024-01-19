using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] CheckGround _checkGround;
    [SerializeField] POVController _povController;
    public POVController POVController { get { return _povController; } }
    Rigidbody _rb;
    float _totalFallTime = 0f;
    public PlayerAnimation PlayerAnim { get; private set; }
    public PlayerState State = PlayerState.Idle;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        PlayerAnim = transform.GetComponentInChildren<PlayerAnimation>();
    }
    void Update()
    {
        if(State == PlayerState.Hide)
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
        inputVector = transform.TransformDirection(inputVector);
        if (_checkGround.IsGrounded)
        {
            _totalFallTime = 0;
            var onPlane = Vector3.ProjectOnPlane(inputVector, _checkGround.NormalVector);
            _rb.velocity = onPlane;
        }
        else
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
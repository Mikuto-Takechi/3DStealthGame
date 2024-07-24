using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class Player : MonoBehaviour
    {
        [SerializeField, Tooltip("接地判定")] CheckGround _checkGround;
        [SerializeField, Tooltip("立っている時にアクティブになるコライダー")] Collider _bodyCollider;
        [SerializeField, Tooltip("しゃがみ時に立ち上がれる高さかを確認するための距離")] float _checkCeilingDistance = 1.9f;
        [SerializeField, Tooltip("レイキャストから除外するレイヤー")] LayerMask _ignoreLayer = 1 << 3 | 1 << 8 | 1 << 9;
        [SerializeField] PlayerMoveController _moveController;
        readonly ReactiveProperty<Vector3> _updatePosition = new();
        public readonly ReactiveProperty<PlayerState> State = new();
        IDisposable _disposable;
        FootStepsSource _footStepsSource;
        FirstPersonViewController _firstPersonViewController;
        Tween _headTween;
        bool _isPaused;
        public FirstPersonViewController FirstPersonViewController => _firstPersonViewController;
        public bool IsDied { get; set; }

        void Awake()
        {
            _firstPersonViewController = GetComponent<FirstPersonViewController>();
            _footStepsSource = GetComponent<FootStepsSource>();
            //  隠れていなければ動くことを許可する。
            State.SkipLatestValueOnSubscribe().Subscribe(state =>
            {
                var condition = state != PlayerState.Hide;
                _moveController.AllowMovement(condition);
                _firstPersonViewController.AllowFirstPersonViewObjects(condition);
            }).AddTo(this);
            //  接地している時に入力するとジャンプ処理を呼ぶ
            InputProvider.Instance.JumpTrigger.Where(_ => _checkGround.IsGrounded.Value).Subscribe(_=>
            {
                _moveController.Jumping();
                _footStepsSource.PlayJumpingSE(_checkGround.HitGameObject, 1, 1);
            }).AddTo(this);
            InputProvider.Instance.CrouchSwitch.Subscribe(Crouching).AddTo(this);
            InputProvider.Instance.RunTrigger.Subscribe(_ =>
            {
                State.Value = State.Value switch
                {
                    PlayerState.Walk => PlayerState.Run,
                    PlayerState.Run => PlayerState.Walk,
                    _ => State.Value
                };
            }).AddTo(this);
            _checkGround.IsGrounded.Where(flag=>flag)
                .Subscribe(_ => _footStepsSource.PlayLandingSE(_checkGround.HitGameObject, 1, 1)).AddTo(this);
        }

        void Start()
        {
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void Update()
        {
            if (_isPaused) return;
            if (State.Value != PlayerState.Run)
            {
                _moveController.RecoveryStamina();
            }
            if(State.Value != PlayerState.Hide)
            {
                var inputVector = InputProvider.Instance.MoveDirection;
                var isMove = inputVector.sqrMagnitude > 0;
                _firstPersonViewController.UpdateView(State.Value, isMove, _checkGround.IsGrounded.Value);
                _footStepsSource.UpdateFootSteps(State.Value, isMove, _checkGround);
                _moveController.UpdateMovement(State, isMove, inputVector, _checkGround);
            }
                
            //  座標更新を発行する
            _updatePosition.Value = transform.position;
            AreaManager.Instance.UpdatePlayerLocation(transform.position);
        }

        void OnDisable()
        {
            GameManager.Instance.OnPause -= OnPause;
            GameManager.Instance.OnResume -= OnResume;
        }

        void OnPause()
        {
            _firstPersonViewController.FreePov = false;
            _isPaused = true;
        }

        void OnResume()
        {
            _firstPersonViewController.FreePov = true;
            _isPaused = false;
        }

        /// <summary>
        /// しゃがみ状態の切り替え
        /// </summary>
        void Crouching(bool flag)
        {
            if (flag) //  しゃがみ状態に入る
            {
                State.Value = PlayerState.Crouch;
                _headTween?.Kill();
                _headTween = _firstPersonViewController.Head.DOLocalMoveY(1, 0.1f).OnComplete(() => _bodyCollider.isTrigger = true)
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
                        _headTween = _firstPersonViewController.Head.DOLocalMoveY(1.6f, 0.1f)
                            .OnComplete(() => _bodyCollider.isTrigger = false).SetLink(gameObject);
                    }).AddTo(this);
                //  一度だけ値を変えずに_updatePositionのOnNext()を呼ぶ
                _updatePosition.SetValueAndForceNotify(_updatePosition.Value);
            }
        }
    }
}
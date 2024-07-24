using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonstersDomain
{
    [Flags]
    public enum ActionType
    {
        Move = 1 << 0,
        Jump = 1 << 2,
        Crouch = 1 << 4,
        Run = 1 << 6,
        Dance = 1 << 8,
        Use = 1 << 10,
        Interact = 1 << 12,
        Drop = 1 << 14,
        SelectHotbar = 1 << 16,
        Pause = 1 << 18
    }
    /// <summary>
    /// 入力を提供するクラス
    /// </summary>
    public class InputProvider
    {
        readonly Dictionary<ActionType, (InputAction, Action<InputAction.CallbackContext>)> _callBackDictionary = new();

        readonly IDisposable _disposable;

        readonly GameInputs _gameInputs;

        readonly Subject<ActionType> _notifyBindInput = new();

        public readonly ActionType AllActionType = ActionType.Move | ActionType.Jump | ActionType.Crouch |
                                                   ActionType.Run |
                                                   ActionType.Dance | ActionType.Use | ActionType.Interact |
                                                   ActionType.Drop |
                                                   ActionType.SelectHotbar | ActionType.Pause;

        ActionType _currentBindInput;

        InputProvider()
        {
            if (_instance == null)
            {
                _instance = this;
                _gameInputs = new GameInputs();
                _callBackDictionary.Add(ActionType.Move, (_gameInputs.InGame.Move, OnMove));
                _callBackDictionary.Add(ActionType.Jump, (_gameInputs.InGame.Jump, OnJump));
                _callBackDictionary.Add(ActionType.Crouch, (_gameInputs.InGame.Crouch, OnCrouch));
                _callBackDictionary.Add(ActionType.Run, (_gameInputs.InGame.Run, OnRun));
                _callBackDictionary.Add(ActionType.Dance, (_gameInputs.InGame.Dance, OnDance));
                _callBackDictionary.Add(ActionType.Use, (_gameInputs.InGame.Use, OnUse));
                _callBackDictionary.Add(ActionType.Interact, (_gameInputs.InGame.Interact, OnInteract));
                _callBackDictionary.Add(ActionType.Drop, (_gameInputs.InGame.Drop, OnDrop));
                _callBackDictionary.Add(ActionType.SelectHotbar, (_gameInputs.InGame.SelectHotbar, OnSelectHotbar));
                _callBackDictionary.Add(ActionType.Pause, (_gameInputs.InGame.Pause, OnPause));
                _disposable = _notifyBindInput.Subscribe(_ => UpdateBindInput());
                CurrentBindInput |= AllActionType;
                _gameInputs.Enable();
            }
        }

        public ActionType CurrentBindInput
        {
            get => _currentBindInput;
            set
            {
                if (!_currentBindInput.Equals(value))
                {
                    _currentBindInput = value;
                    _notifyBindInput.OnNext(value);
                }
            }
        }

        public Vector3 MoveDirection { get; private set; }
        public Subject<Unit> JumpTrigger { get; } = new();
        public BoolReactiveProperty CrouchSwitch { get; } = new();
        public Subject<Unit> RunTrigger { get; } = new();
        public Subject<Unit> DanceTrigger { get; } = new();
        public Subject<float> SelectHotbarAxis { get; } = new();
        public Subject<Unit> UseTrigger { get; } = new();
        public Subject<Unit> InteractTrigger { get; } = new();
        public Subject<Unit> DropTrigger { get; } = new();
        public Subject<Unit> PauseTrigger { get; } = new();

        void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var temp = context.ReadValue<Vector2>();
                MoveDirection = new Vector3(temp.x, 0, temp.y);
            }
            else if (context.canceled)
            {
                MoveDirection = Vector3.zero;
            }
        }

        void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed) JumpTrigger.OnNext(Unit.Default);
        }

        void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed)
                CrouchSwitch.Value = true;
            else if (context.canceled)
                CrouchSwitch.Value = false;
        }

        void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed) RunTrigger.OnNext(Unit.Default);
        }

        void OnDance(InputAction.CallbackContext context)
        {
            if (context.performed) DanceTrigger.OnNext(Unit.Default);
        }

        void OnSelectHotbar(InputAction.CallbackContext context)
        {
            if (context.performed) SelectHotbarAxis.OnNext(context.ReadValue<float>());
        }

        void OnUse(InputAction.CallbackContext context)
        {
            if (context.performed) UseTrigger.OnNext(Unit.Default);
        }

        void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) InteractTrigger.OnNext(Unit.Default);
        }

        void OnDrop(InputAction.CallbackContext context)
        {
            if (context.performed) DropTrigger.OnNext(Unit.Default);
        }

        void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed) PauseTrigger.OnNext(Unit.Default);
        }

        void UpdateBindInput()
        {
            //Debug.Log(Convert.ToString((int)_currentBindInput, 2));
            foreach (var callback in _callBackDictionary)
                //  指定されたビットフラグが立っているかつそのビット<<1が立っていないとき
                if ((int)(_currentBindInput & callback.Key) != 0
                    && (int)(_currentBindInput & (ActionType)((int)callback.Key << 1)) == 0)
                {
                    //Debug.Log("Add" + Enum.GetName(typeof(ActionType), callback.Key));
                    _currentBindInput |= (ActionType)((int)callback.Key << 1);
                    callback.Value.Item1.performed += callback.Value.Item2;
                    callback.Value.Item1.canceled += callback.Value.Item2;
                }
                else if ((int)(_currentBindInput & callback.Key) == 0
                         && (int)(_currentBindInput & (ActionType)((int)callback.Key << 1)) != 0)
                {
                    //  指定されたビットが立っていないかつそのビット<<1が立っているとき
                    //  指定されたビットの<<1のビットを倒す
                    //Debug.Log("Remove" + Enum.GetName(typeof(ActionType), callback.Key));
                    _currentBindInput &= ~(ActionType)((int)callback.Key << 1);
                    callback.Value.Item1.performed -= callback.Value.Item2;
                    callback.Value.Item1.canceled -= callback.Value.Item2;
                    if (callback.Key == ActionType.Move) MoveDirection = Vector3.zero;
                    if (callback.Key == ActionType.Crouch) CrouchSwitch.Value = false;
                }

            //Debug.Log(Convert.ToString((int)_currentBindInput, 2));
        }

        ~InputProvider()
        {
            _gameInputs?.Dispose();
            _disposable?.Dispose();
        }

        #region PureSingleton

        static InputProvider _instance;

        public static InputProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InputProvider();
                    return _instance;
                }

                return _instance;
            }
        }

        #endregion
    }
}
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

        //BindedMove = 1 << 1,
        Jump = 1 << 2,

        //BindedJump = 1 << 3,
        Crouch = 1 << 4,

        //BindedCrouch = 1 << 5,
        Run = 1 << 6,

        //BindedRun = 1 << 7,
        Dance = 1 << 8,

        //BindedDance = 1 << 9,
        Use = 1 << 10,

        //BindedUse = 1 << 11,
        Interact = 1 << 12,

        //BindedInteract = 1 << 13,
        Drop = 1 << 14,

        //BindedDrop = 1 << 15,
        SelectHotbar = 1 << 16
        //BindedSelectHotbar = 1 << 17,
    }

    public class InputProvider
    {
        readonly Dictionary<ActionType, (InputAction, Action<InputAction.CallbackContext>)> _callBackDictionary = new();

        readonly IDisposable _disposable;

        readonly GameInputs _gameInputs;

        readonly Subject<ActionType> _notifyBindInput = new();
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
                _disposable = _notifyBindInput.Subscribe(_ => UpdateBindInput());
                CurrentBindInput = (ActionType)87381;
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
                    //  指定されたビット<<1のビットをたおす
                    //Debug.Log("Remove" + Enum.GetName(typeof(ActionType), callback.Key));
                    _currentBindInput &= ~(ActionType)((int)callback.Key << 1);
                    callback.Value.Item1.performed -= callback.Value.Item2;
                    callback.Value.Item1.canceled -= callback.Value.Item2;
                    if(callback.Key == ActionType.Move) MoveDirection = Vector3.zero;
                    if(callback.Key == ActionType.Crouch) CrouchSwitch.Value = false;
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
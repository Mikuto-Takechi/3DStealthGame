using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonstersDomain
{
    public class InputProvider
    {
        readonly GameInputs _gameInputs;
        bool _isBindCharacterInput;
        bool _isBindGUIInput;

        InputProvider()
        {
            if (_instance == null)
            {
                _instance = this;
                _gameInputs = new GameInputs();
                BindCharacterInput();
                BindGUIInput();
                _gameInputs.Enable();
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

        public void BindCharacterInput()
        {
            if (_isBindCharacterInput) return;
            _gameInputs.InGame.Move.performed += OnMove;
            _gameInputs.InGame.Move.canceled += OnMove;
            _gameInputs.InGame.Jump.performed += OnJump;
            _gameInputs.InGame.Crouch.performed += OnCrouch;
            _gameInputs.InGame.Crouch.canceled += OnCrouch;
            _gameInputs.InGame.Run.performed += OnRun;
            _gameInputs.InGame.Dance.performed += OnDance;
            _gameInputs.InGame.Use.performed += OnUse;
            _gameInputs.InGame.Interact.performed += OnInteract;
            _isBindCharacterInput = true;
        }

        public void UnBindCharacterInput()
        {
            if (!_isBindCharacterInput) return;
            _gameInputs.InGame.Move.performed -= OnMove;
            _gameInputs.InGame.Move.canceled -= OnMove;
            _gameInputs.InGame.Jump.performed -= OnJump;
            _gameInputs.InGame.Crouch.performed -= OnCrouch;
            _gameInputs.InGame.Crouch.canceled -= OnCrouch;
            _gameInputs.InGame.Run.performed -= OnRun;
            _gameInputs.InGame.Dance.performed -= OnDance;
            _gameInputs.InGame.Use.performed -= OnUse;
            _gameInputs.InGame.Interact.performed -= OnInteract;
            _isBindCharacterInput = false;
            MoveDirection = Vector3.zero;
            CrouchSwitch.Value = false;
        }

        public void BindGUIInput()
        {
            if (_isBindGUIInput) return;
            _gameInputs.InGame.SelectHotbar.performed += OnSelectHotbar;
            _isBindGUIInput = true;
        }
        public void UnBindGUIInput()
        {
            if (!_isBindGUIInput) return;
            _gameInputs.InGame.SelectHotbar.performed -= OnSelectHotbar;
            _isBindGUIInput = false;
        }

        ~InputProvider()
        {
            _gameInputs?.Dispose();
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
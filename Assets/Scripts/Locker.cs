using System;
using System.Linq;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace MonstersDomain
{
    public class Locker : Interactable
    {
        [SerializeField] GameObject _dummyPlayer;
        [SerializeField] PlayableDirector[] _directors;
        Player _player;
        bool _isInteract = false;
        IDisposable _enterSubscription, _exitSubscription;
        void Awake()
        {
            _dummyPlayer?.SetActive(false);
            BindMainCamera();
        }

        void OnDisable()
        {
            _enterSubscription?.Dispose();
            _exitSubscription?.Dispose();
        }

        protected override void Disengage(Player player)
        {
            if (_isInteract) return;
            InteractionMessage.Instance.WriteText(string.Empty);
            _enterSubscription?.Dispose();
        }
        protected override void Interact(Player player)
        {
            if (_isInteract) return;
            InteractionMessage.Instance.WriteText("[E] 隠れる");
            _enterSubscription = InputProvider.Instance.InteractTrigger.First().Subscribe(_ =>
            {
                _isInteract = true;
                _player = player;
                player.State.Value = PlayerState.Hide;
                _dummyPlayer?.SetActive(true);
                EnterTheLocker();
            }).AddTo(this);
        }
        void EnterTheLocker()
        {
            InputProvider.Instance.CurrentBindInput &= ~(ActionType.Move | ActionType.Crouch | ActionType.Drop | ActionType.Jump);
            _enterSubscription?.Dispose();
            _directors[0].Play();
            _directors[0].stopped += HidingInLocker;
            InteractionMessage.Instance.WriteText(string.Empty);
        }
        public void HidingInLocker(PlayableDirector pd)
        {
            _directors[0].stopped -= HidingInLocker;
            _player.PovController.FreePov = false;
            _player.PovController.SetRotation(0, transform.localEulerAngles.y);
            _directors[1].Play();
            InteractionMessage.Instance.WriteText("[E] 出る");
            _exitSubscription = InputProvider.Instance.InteractTrigger.Take(1).Subscribe(ExitFromLocker).AddTo(this);
        }
        void ExitFromLocker(Unit _)
        {
            _exitSubscription?.Dispose();
            _directors[1].Stop();
            _directors[2].Play();
            _directors[2].stopped += Exit;
            InteractionMessage.Instance.WriteText(string.Empty);
        }
        void Exit(PlayableDirector playableDirector)
        {
            InputProvider.Instance.CurrentBindInput |= ActionType.Move | ActionType.Crouch | ActionType.Drop | ActionType.Jump;
            _directors[2].stopped -= Exit;
            _player.State.Value = PlayerState.Idle;
            _player.PovController.FreePov = true;
            _dummyPlayer?.SetActive(false);
            _player = null;
            _isInteract = false;
        }
        void BindMainCamera()
        {
            CinemachineBrain main = Camera.main.GetComponent<CinemachineBrain>();
            _directors.ToList().ForEach(director => 
            {
                // TimelineからCinemachineTrackのトラックへの参照を取得して
                // MainCameraの情報を流し込む
                var binding = director.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
                director.SetGenericBinding(binding.sourceObject, main);
            });
        }
    }
}

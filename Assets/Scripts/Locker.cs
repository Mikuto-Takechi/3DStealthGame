using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Playables;

namespace MonstersDomain
{
    public class Locker : Interactable
    {
        [SerializeField] GameObject _dummyPlayer;
        [SerializeField] PlayableDirector[] _directors;
        List<IDisposable> _subscriptions = new();
        Player _player;
        bool _isInteract = false;
        void Awake()
        {
            _dummyPlayer?.SetActive(false);
            BindMainCamera();
        }
        protected override void Disengage(Player player)
        {
            InteractionMessage.Instance.WriteText(string.Empty);
            _subscriptions.ForEach(sub => sub.Dispose());
            _subscriptions.Clear();
            _isInteract = false;
        }
        protected override void Interact(Player player)
        {
            if (_isInteract) return;
            _isInteract = true;
            InteractionMessage.Instance.WriteText("[E] 隠れる");
            this.UpdateAsObservable().First(_ => Input.GetButtonDown("Interact"))
                .Subscribe(_ => 
                {
                    _player = player;
                    player.State.Value = PlayerState.Hide;
                    _dummyPlayer?.SetActive(true);
                    EnterTheLocker();
                }).AddTo(_subscriptions);
        }
        void EnterTheLocker()
        {
            _subscriptions.ForEach(sub => sub.Dispose());
            _directors[0].Play();
            _directors[0].stopped += HidingInLocker;
            InteractionMessage.Instance.WriteText(string.Empty);
        }
        public void HidingInLocker(PlayableDirector pd)
        {
            _subscriptions.ForEach(sub => sub.Dispose());
            _directors[0].stopped -= HidingInLocker;
            _player.PovController.Enabled = false;
            _player.PovController.SetRotation(0, transform.localEulerAngles.y);
            _directors[1].Play();
            InteractionMessage.Instance.WriteText("[E] 出る");
            this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Interact")).Take(1)
                .Subscribe(ExitFromLocker).AddTo(_subscriptions);
        }
        void ExitFromLocker(Unit _)
        {
            _directors[1].Stop();
            _directors[2].Play();
            _directors[2].stopped += Exit;
            InteractionMessage.Instance.WriteText(string.Empty);
        }
        void Exit(PlayableDirector playableDirector)
        {
            _directors[2].stopped -= Exit;
            _player.State.Value = PlayerState.Idle;
            _player.PovController.Enabled = true;
            _dummyPlayer?.SetActive(false);
            _player = null;
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

using UnityEngine;

namespace MonstersDomain
{
    public abstract class BaseAsyncMover : MonoBehaviour
    {
        protected Coroutine _coroutine;
        protected bool _isPaused;
        public Coroutine Coroutine => _coroutine;
        public MoveState Current { get; set; } = MoveState.Ready;

        void Start()
        {
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void OnPause()
        {
            _isPaused = true;
        }

        void OnResume()
        {
            _isPaused = false;
        }

        public virtual void StopMove()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                if (Current != MoveState.Complete) Current = MoveState.Abort;
            }

            _coroutine = null;
        }
    }
}
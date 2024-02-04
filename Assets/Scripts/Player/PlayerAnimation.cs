using DG.Tweening;
using UnityEngine;

namespace MonstersDomain
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] Rigidbody _rb;
        [SerializeField] Transform _body;
        public Transform HandIKTarget;
        Animator _animator;
        void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        public void EnterTheLocker(Vector3 position)
        {
            _rb.isKinematic = true;
            _animator.Play("OpenDoor");
            _animator.Play("Walking");
            _body.DOMove(position, 4.03f).SetLink(gameObject);
        }
        public void ResetIK()
        {
            HandIKTarget = null;
        }
        void OnAnimatorIK(int layerIndex)
        {
            if (HandIKTarget)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.05f);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, HandIKTarget.position);
            }
        }
    }
}

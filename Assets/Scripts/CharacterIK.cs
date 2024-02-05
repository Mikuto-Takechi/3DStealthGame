using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    [RequireComponent(typeof(Animator))]
    public class CharacterIK : MonoBehaviour
    {
        [SerializeField] AvatarIKGoal _goal;  // どの部位のIKを使用するか
        [SerializeField] AvatarIKHint _hint;  // goalと同じ部位のヒントを選択
        [SerializeField] Transform _goalTransform;// 最終的な位置
        [SerializeField] Transform _hintTransform;// 肘や膝の位置のヒント

        [SerializeField][Range(0, 1)] float _weight, _hintWeight;

        Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void OnAnimatorIK(int layerIndex)
        {
            _animator.SetIKPosition(_goal, _goalTransform.position);
            _animator.SetIKHintPosition(_hint, _hintTransform.position);

            _animator.SetIKHintPositionWeight(_hint, _hintWeight);
            _animator.SetIKPositionWeight(_goal, _weight);
        }
    }

}

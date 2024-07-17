using System;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    ///     接地判定クラス
    /// </summary>
    public class CheckGround : MonoBehaviour
    {
        [SerializeField, Tooltip("レイキャストから除外するレイヤー")] 
        LayerMask _ignoreLayer;

        [SerializeField] float _raycastDistance = 25f;
        [SerializeField] float _maxSlopeAngle = 45f;
        [SerializeField] float _sphereRadius = 5f;
        bool _exitGround;
        public Action HitWall;
        public bool IsGrounded { get; set; }
        public Vector3 NormalVector { get; private set; } = Vector3.up;

        void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts)
                if (_maxSlopeAngle >= Vector3.Angle(Vector3.up, contact.normal))
                {
                    NormalVector = contact.normal;
                    IsGrounded = true;
                }
                else
                {
                    HitWall?.Invoke();
                }
        }
    }
}
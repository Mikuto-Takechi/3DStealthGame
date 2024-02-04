using System;
using UnityEngine;

namespace MonstersDomain
{
    public class CheckGround : MonoBehaviour
    {
        [SerializeField] float _maxSlopeAngle = 45f;
        public bool IsGrounded { get; set; } = false;

        public Vector3 NormalVector { get; private set; } = Vector3.up;

        private ContactPoint _lastContactPoint;
        public Action HitWall;

        void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                if (_maxSlopeAngle >= Vector3.Angle(Vector3.up, contact.normal))
                {
                    NormalVector = contact.normal;
                    IsGrounded = true;
                }
                else
                {
                    HitWall?.Invoke();
                }
                _lastContactPoint = contact;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (Vector3.Angle(_lastContactPoint.normal, Vector3.up) < _maxSlopeAngle)
            {
                IsGrounded = false;
            }
        }
    }
}

using System;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using MonstersDomain;
using UnityEditor;
/// <summary>
/// 地面に対する法線ベクトルが取れているかを確認するためのエディタークラス
/// </summary>
[CustomEditor(typeof(CheckGround))]
public class CheckGroundEditor : Editor
{
    CheckGround _target;
    void OnEnable()
    {
        _target = target as CheckGround;
    }

    void OnSceneGUI()
    {
        Handles.DrawLine(_target.transform.position, _target.transform.position + _target.NormalVector * 10f);
    }
}
#endif

namespace MonstersDomain
{
    /// <summary>
    ///     接地判定クラス
    /// </summary>
    public class CheckGround : MonoBehaviour
    {
        [SerializeField, Tooltip("レイキャストから除外するレイヤー")] LayerMask _ignoreLayer;
        [SerializeField] float _raycastDistance = 25f;
        [SerializeField] float _maxSlopeAngle = 45f;
        [SerializeField] float _sphereRadius = 5f;
        [SerializeField] float _hitMin = 1f;
        RaycastHit[] _raycastHits = new RaycastHit[100];
        Ray _ray;
        public Action HitWall;
        public Vector3 NormalVector { get; private set; } = Vector3.up;
        public readonly BoolReactiveProperty IsGrounded = new();
        public GameObject HitGameObject { get; private set; }
        
        void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(transform.position,_sphereRadius 
                ,Vector3.down ,_raycastHits, _raycastDistance, ~_ignoreLayer, 
                QueryTriggerInteraction.Ignore);
            bool isHit = false;
            float minDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (_raycastHits[i].distance <= _hitMin)
                {
                    if (Vector3.Angle(Vector3.up, _raycastHits[i].normal) < _maxSlopeAngle)
                    {
                        IsGrounded.Value = true;
                        isHit = true;
                        if (minDistance > _raycastHits[i].distance)
                        {
                            NormalVector = _raycastHits[i].normal;
                            HitGameObject = _raycastHits[i].collider.gameObject;
                            minDistance = _raycastHits[i].distance;
                        }
                    }
                    else
                    {
                        HitWall?.Invoke();
                    }
                }
            }

            if (!isHit)
            {
                IsGrounded.Value = false;
            }
        }
    }
}
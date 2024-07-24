using System;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// 接地判定クラス
    /// </summary>
    public class CheckGround : MonoBehaviour
    {
        [SerializeField, Tooltip("レイキャストから除外するレイヤー")] LayerMask _ignoreLayer = 1 << 3 | 1 << 8 | 1 << 9;
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
            //  真下にレイキャストを飛ばして当たったオブジェクトを配列に格納して数を受け取る。
            var count = Physics.SphereCastNonAlloc(transform.position,_sphereRadius 
                ,Vector3.down ,_raycastHits, _raycastDistance, ~_ignoreLayer, 
                QueryTriggerInteraction.Ignore);
            bool isHit = false;
            float minDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (_raycastHits[i].distance <= _hitMin)
                {
                    //  ワールド座標の上ベクトルと平面の法線ベクトルの角度で設置しているかを判定する。
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
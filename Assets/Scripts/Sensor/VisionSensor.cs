using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace MonstersDomain
{
    public class VisionSensor : MonoBehaviour
    {
        /// <summary>視野角</summary>
        [SerializeField] float _sightAngle = 45f;
        [SerializeField] SphereCollider _visionRange;
        [SerializeField] Color _visionRangeColor = Color.yellow;
        [SerializeField] LayerMask _ignoreRaycast;
        [SerializeField] Transform _headAnchor;
        HashSet<Player> _playerHashSet = new();
        public float SightAngle => _sightAngle;
        public float VisionRange => _visionRange.radius;
        public Color VisionRangeColor => _visionRangeColor;
        public bool InSightTarget(ref Player referencePlayer)
        {
            if (_playerHashSet.Count <= 0) return false;
            Player player = _playerHashSet.First();
            Vector3 posDelta = player.transform.position - _headAnchor.position;
            float targetAngle = Vector3.Angle(transform.forward, posDelta);
            if(targetAngle < _sightAngle)
            {
                //Debug.DrawRay (_headAnchor.position, posDelta.normalized * posDelta.magnitude, Color.red, 0.1f, false);
                if(Physics.Raycast(_headAnchor.position,posDelta,out RaycastHit hit, posDelta.magnitude, ~_ignoreRaycast))
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        referencePlayer = player;
                        return true;
                    }
                }
            }

            return false;
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _playerHashSet.Add(player);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            { 
                _playerHashSet.Remove(player);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(VisionSensor))]
    public class VisionSensorEditor : Editor
    {
        public void OnSceneGUI()
        {
            var visionSensor = target as VisionSensor;
            // work out the start point of the vision cone
            Vector3 startPoint = Mathf.Cos(-visionSensor.SightAngle * Mathf.Deg2Rad) * visionSensor.transform.forward +
                                 Mathf.Sin(-visionSensor.SightAngle * Mathf.Deg2Rad) * visionSensor.transform.right;

            // draw the vision cone
            Handles.color = visionSensor.VisionRangeColor;
            Handles.DrawSolidArc(visionSensor.transform.position, Vector3.up, startPoint, visionSensor.SightAngle * 2f, visionSensor.VisionRange);
        }
    }
#endif // UNITY_EDITOR
}
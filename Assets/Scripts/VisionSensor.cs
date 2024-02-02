using System;
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
        public Subject<Player> DetectionTarget = new();
        public float SightAngle => _sightAngle;
        public float VisionRange => _visionRange.radius;
        public Color VisionRangeColor => _visionRangeColor;
        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Vector3 posDelta = other.transform.position - _headAnchor.position;
                float targetAngle = Vector3.Angle(transform.forward, posDelta);
                if(targetAngle < _sightAngle)
                {
                    Debug.DrawRay (_headAnchor.position, posDelta.normalized * posDelta.magnitude, Color.red, 0.1f, false);
                    if(Physics.Raycast(_headAnchor.position,posDelta,out RaycastHit hit, posDelta.magnitude, ~_ignoreRaycast))
                    {
                        if (hit.collider.Equals(other))
                        {
                            DetectionTarget.OnNext(other.GetComponent<Player>());
                        }
                    }
                }
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
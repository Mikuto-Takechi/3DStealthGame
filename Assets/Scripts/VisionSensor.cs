using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    /// <summary>����p</summary>
    [SerializeField] float _sightAngle = 45f;
    [SerializeField] SphereCollider _visionRange;
    [SerializeField] Color _visionRangeColor = Color.yellow;
    public float SightAngle { get { return _sightAngle; } }
    public float VisionRange { get { return _visionRange.radius; } }
    public Color VisionRangeColor { get { return _visionRangeColor; } }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 posDelta = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(transform.forward, posDelta);
            if(targetAngle < _sightAngle)
            {
                if(Physics.Raycast(transform.position,new Vector3(posDelta.x, 0, posDelta.z),out RaycastHit hit))
                {
                    if (hit.collider.Equals(other))
                    {
                        Debug.Log("�v���C���[��������");
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [SerializeField] float _maxSlopeAngle = 45f;
    bool _isGrounded = false;
    Vector3 _normalVector = Vector3.up;
    public bool IsGrounded { get { return _isGrounded; } }
    public Vector3 NormalVector { get { return _normalVector; } }
    void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (_maxSlopeAngle >= Vector3.Angle(Vector3.up, contact.normal))
            {
                _normalVector = contact.normal;
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        _isGrounded = true;

    }
    void OnCollisionExit(Collision collision)
    {
        
    }
    void OnTriggerExit(Collider other)
    {
        _isGrounded = false;
        _normalVector = Vector3.up;
    }
}

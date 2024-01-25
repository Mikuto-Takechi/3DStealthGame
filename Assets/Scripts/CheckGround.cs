using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [SerializeField] float _maxSlopeAngle = 45f;
    bool _isGrounded = false;
    Vector3 _normalVector = Vector3.up;
    public bool IsGrounded => _isGrounded;
    public Vector3 NormalVector => _normalVector;
    private ContactPoint _lastContactPoint;

    void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (_maxSlopeAngle >= Vector3.Angle(Vector3.up, contact.normal))
            {
                _normalVector = contact.normal;
                _isGrounded = true;
            }
            _lastContactPoint = contact;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (Vector3.Angle(_lastContactPoint.normal, Vector3.up) < _maxSlopeAngle)
        {
            _isGrounded = false;
        }
    }
}

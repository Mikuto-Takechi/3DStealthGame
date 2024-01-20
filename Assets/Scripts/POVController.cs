using Cinemachine;
using UnityEngine;

public class POVController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] bool _cursorLock = true;
    [SerializeField] GameObject _head;
    public CinemachineVirtualCamera VirtualCamera { get { return _virtualCamera; } }
    public bool Enabled { get; set; } = true;
    Quaternion _headRotation, _bodyRotation;
    float _xSensitivity = 3f, _ySensitivity = 3f;
    //�ϐ��̐錾(�p�x�̐����p)
    float _minX = -90f, _maxX = 90f;

    void Start()
    {
        _bodyRotation = transform.localRotation;
        _headRotation = transform.localRotation;
    }

    void Update()
    {
        if (Enabled)
        {
            float xRot = Input.GetAxis("Mouse X") * _ySensitivity;
            float yRot = Input.GetAxis("Mouse Y") * _xSensitivity;

            _headRotation *= Quaternion.Euler(-yRot, 0, 0);
            _bodyRotation *= Quaternion.Euler(0, xRot, 0);

            //Update�̒��ō쐬�����֐����Ă�
            _headRotation = ClampRotation(_headRotation);
            _head.transform.localRotation = _headRotation;
            transform.localRotation = _bodyRotation;
        }
        UpdateCursorLock();
    }
    void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            _cursorLock = true;
        }


        if (_cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!_cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //�p�x�����֐��̍쐬
    Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,z�̓x�N�g���i�ʂƌ����j�Fw�̓X�J���[�i���W�Ƃ͖��֌W�̗ʁj)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, _minX, _maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }
    /// <summary>POV�̊p�x���O������ݒ肷��</summary>
    public void SetRotation(float xRot, float yRot)
    {
        _headRotation = Quaternion.Euler(xRot, 0, 0);
        _bodyRotation = Quaternion.Euler(0, yRot, 0);
        _headRotation = ClampRotation(_headRotation);
        _head.transform.localRotation = _headRotation;
        transform.localRotation = _bodyRotation;
    }
}

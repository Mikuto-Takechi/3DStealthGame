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
    //変数の宣言(角度の制限用)
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

            //Updateの中で作成した関数を呼ぶ
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

    //角度制限関数の作成
    Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, _minX, _maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }
    /// <summary>POVの角度を外部から設定する</summary>
    public void SetRotation(float xRot, float yRot)
    {
        _headRotation = Quaternion.Euler(xRot, 0, 0);
        _bodyRotation = Quaternion.Euler(0, yRot, 0);
        _headRotation = ClampRotation(_headRotation);
        _head.transform.localRotation = _headRotation;
        transform.localRotation = _bodyRotation;
    }
}

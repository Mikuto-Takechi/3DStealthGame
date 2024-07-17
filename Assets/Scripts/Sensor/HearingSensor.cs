using UnityEngine;

namespace MonstersDomain
{
    public class HearingSensor : MonoBehaviour
    {
        [SerializeField] Transform _headAnchor;
        public Vector3 CheckLocation { get; set; } = Vector3.zero;

        void Start()
        {
            HearingManager.Instance.Register(this);
        }

        void OnDisable()
        {
            HearingManager.Instance.Deregister(this);
        }

        public void OnHeardSound(Vector3 location, float hearingDistance)
        {
            //  発信源とセンサーの距離が音の距離以下なら
            if ((_headAnchor.position - location).sqrMagnitude <= hearingDistance * hearingDistance)
                CheckLocation = location;
        }
    }
}
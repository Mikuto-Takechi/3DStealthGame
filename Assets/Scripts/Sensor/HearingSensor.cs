using UnityEngine;

namespace MonstersDomain
{
    public class HearingSensor : MonoBehaviour
    {
        [SerializeField] Transform _headAnchor;
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
            {
                Detected();
            }
        }

        public void Detected()
        {
            print("確認するために近づく。");
        }
    }
}

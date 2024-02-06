using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class AreaManager : MonoBehaviour
    {
        public static AreaManager Instance;
        [SerializeField, Tooltip("1階に入ったと判定される高さ")] float _firstFloorThreshold = 5f;
        [SerializeField, Tooltip("2階に入ったと判定される高さ")] float _secondFloorThreshold = 9.8f;
        public BoolReactiveProperty ReportSwitchFloor { get; } = new(false);    //  false : 1階 true : 2階
        void Awake()
        {
            Instance ??= this;
        }

        void OnDisable()
        {
            Instance = null;
        }

        public void ReportPlayerLocation(float height)
        {
            if (height <= _firstFloorThreshold)
            {
                ReportSwitchFloor.Value = false;
            }
            else if (height >= _secondFloorThreshold)
            {
                ReportSwitchFloor.Value = true;
            }
        }
    }
}

using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// 敵が徘徊するエリアや巡回座標を提供するクラス。
    /// </summary>
    public class AreaManager : SingletonBase<AreaManager>
    {
        [SerializeField] List<AreaData> _areaDataList;
        int _currentAreaIndex;
        int _patrolIndex;
        public List<AreaData> AreaDataList => _areaDataList;
        public Subject<Vector3> SwitchAreaSubject { get; } = new();

        protected override void AwakeFunction()
        {
        }

        public void UpdatePlayerLocation(Vector3 location)
        {
            for (var i = 0; i < _areaDataList.Count; i++)
                if (_areaDataList[i].CheckInArea(location))
                {
                    //  今と違うエリアなら
                    if (_currentAreaIndex != i)
                    {
                        _patrolIndex = 0;
                        _currentAreaIndex = i;
                        SwitchAreaSubject.OnNext(_areaDataList[i].WarpPoint);
                    }

                    break;
                }
        }

        public Vector3 GetDestination()
        {
            return _areaDataList[_currentAreaIndex].PatrolAnchors[_patrolIndex];
        }

        public void NextPatrolIndex()
        {
            _patrolIndex++;
            _patrolIndex %= _areaDataList[_currentAreaIndex].PatrolAnchors.Count;
        }
    }
}
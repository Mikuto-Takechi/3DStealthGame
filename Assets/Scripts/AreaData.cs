using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    [Serializable]
    public class AreaData
    {
        [SerializeField] Bounds _floorBounds;
        [SerializeField] Vector3 _warpPoint;
        [SerializeField] List<Vector3> _patrolAnchors;

        public Bounds FloorBounds
        {
            get => _floorBounds;
            set => _floorBounds = value;
        }

        public Vector3 WarpPoint
        {
            get => _warpPoint;
            set => _warpPoint = value;
        }

        public List<Vector3> PatrolAnchors => _patrolAnchors;

        public bool CheckInArea(Vector3 pos)
        {
            return _floorBounds.Contains(pos);
        }
    }
}
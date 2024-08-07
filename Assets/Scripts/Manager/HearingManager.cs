using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// 音が鳴った座標を管理して各センサーに伝達するためのクラス。
    /// </summary>
    public class HearingManager : SingletonBase<HearingManager>
    {
        public List<HearingSensor> AllSensors { get; } = new();

        protected override void AwakeFunction()
        {
        }

        public void Register(HearingSensor sensor)
        {
            AllSensors.Add(sensor);
        }

        public void Deregister(HearingSensor sensor)
        {
            AllSensors.Remove(sensor);
        }

        public void OnSoundEmitted(Vector3 location, float hearingDistance)
        {
            // 登録されている全てのセンサーに通知を送る
            foreach (var sensor in AllSensors) sensor.OnHeardSound(location, hearingDistance);
        }
    }
}
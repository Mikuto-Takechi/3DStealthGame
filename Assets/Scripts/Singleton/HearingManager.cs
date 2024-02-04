using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class HearingManager : SingletonBase<HearingManager>
    {
        public List<HearingSensor> AllSensors { get; private set; } = new List<HearingSensor>();
        
        protected override void AwakeFunction() { }
        
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
            foreach(var sensor in AllSensors)
            {
                sensor.OnHeardSound(location, hearingDistance);
            }
        }
    }
}

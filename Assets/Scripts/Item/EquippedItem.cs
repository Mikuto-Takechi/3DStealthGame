using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class EquippedItem : InstancedItem
    {
        [SerializeField] protected List<ItemParameter> _parametersToModify;
        [SerializeField] protected List<ItemParameter> _requiredParameters;
        public List<ParameterUI> ParametersUI { get; private set; } = new();
        public new void InheritParameters(List<ItemParameter> inheritParametersList, bool callByReference = false)
        {
            base.InheritParameters(inheritParametersList, callByReference);
            if (CurrentParametersList is { Count: > 0 })
            {
                foreach (var parameter in CurrentParametersList)
                {
                }
            }
        }
        protected void ModifyParameters(float deltaTime = 1)
        {
            foreach (var modify in _parametersToModify)
            {
                if (CurrentParametersList.Contains(modify))
                {
                    int index = CurrentParametersList.IndexOf(modify);
                    var modifiedParam = CurrentParametersList[index];
                    if (modifiedParam.Value > 0)
                    {
                        modifiedParam.Value += modify.Value * deltaTime;
                        int defaultIndex = _itemData.DefaultParametersList.IndexOf(modify);
                        modifiedParam.Value = Mathf.Clamp(modifiedParam.Value, 0, _itemData.DefaultParametersList[defaultIndex].Value);
                        CurrentParametersList[index] = modifiedParam;
                    }
                }
            }
        }
        /// <summary>
        /// 現在のパラメーター量が必要パラメーター以下ならfalse、<br/>
        /// 必要パラメーターより大きいならばtrueを返す。
        /// </summary>
        protected bool RequiredParameters()
        {
            foreach (var required in _requiredParameters)
            {
                //  requiredと同じIDのパラメーターが含まれているか
                if (CurrentParametersList.Contains(required))
                {
                    int index = CurrentParametersList.IndexOf(required);
                    if (CurrentParametersList[index].Value <= required.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

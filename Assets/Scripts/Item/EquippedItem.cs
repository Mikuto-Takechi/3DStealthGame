using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class EquippedItem : InstancedItem
    {
        [SerializeField] protected List<ItemParameter> _parametersToModify;
        [SerializeField] protected List<ItemParameter> _requiredParameters;
        public List<DisplayParameter> ParametersUI { get; } = new();
        public void InheritParameters(List<ItemParameter> inheritParametersList, bool callByReference = false, Transform parameterUIAnchor = null)
        {
            base.InheritParameters(inheritParametersList, callByReference);
            if (CurrentParametersList is { Count: > 0 } && parameterUIAnchor)
            {
                foreach (var parameter in CurrentParametersList)
                {
                    var displayParameterIds = _itemData.DisplayParameters.Select(p => p.ID).ToList();
                    if(displayParameterIds.Contains(parameter.ID))
                    {
                        int displayIndex = displayParameterIds.IndexOf(parameter.ID);
                        int currentParamIndex = CurrentParametersList.IndexOf(parameter);
                        int defaultParamIndex = _itemData.DefaultParameters.IndexOf(parameter);
                        var instance = Instantiate(_itemData.DisplayParameters[displayIndex].UI, parameterUIAnchor);
                        instance.Value = CurrentParametersList[currentParamIndex].Value /
                                         _itemData.DefaultParameters[defaultParamIndex].Value;
                        ParametersUI.Add(new(){ID = _itemData.DisplayParameters[displayIndex].ID, UI = instance});
                    }
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
                        int defaultIndex = _itemData.DefaultParameters.IndexOf(modify);
                        modifiedParam.Value = Mathf.Clamp(modifiedParam.Value, 0, _itemData.DefaultParameters[defaultIndex].Value);
                        CurrentParametersList[index] = modifiedParam;
                        var displayParameterIds = ParametersUI.Select(p => p.ID).ToList();
                        if(displayParameterIds.Contains(modifiedParam.ID))
                        {
                            int displayIndex = displayParameterIds.IndexOf(modifiedParam.ID);
                            ParametersUI[displayIndex].UI.Value = modifiedParam.Value / _itemData.DefaultParameters[defaultIndex].Value;
                        }
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

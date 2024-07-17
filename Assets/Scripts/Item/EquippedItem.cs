using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonstersDomain
{
    public class EquippedItem : InstancedItem
    {
        [SerializeField, Range(1, 2)] int _holdType = 1;
        [SerializeField] protected List<ItemParameter> _parametersToModify;
        [SerializeField] protected List<ItemParameter> _requiredParameters;
        public List<DisplayParameter> ParametersUI { get; } = new();
        public int HoldType => _holdType;

        public void InheritParameters(List<ItemParameter> inheritParametersList, bool callByReference = false,
            Transform parameterUIAnchor = null)
        {
            base.InheritParameters(inheritParametersList, callByReference);
            if (CurrentParametersList is { Count: > 0 } && parameterUIAnchor)
                foreach (var parameter in CurrentParametersList)
                {
                    var displayParameterIds = _itemData.DisplayParameters.Select(p => p.ID).ToList();
                    if (displayParameterIds.Contains(parameter.ID))
                    {
                        var displayIndex = displayParameterIds.IndexOf(parameter.ID);
                        var currentParamIndex = CurrentParametersList.IndexOf(parameter);
                        var defaultParamIndex = _itemData.DefaultParameters.IndexOf(parameter);
                        var instance = Instantiate(_itemData.DisplayParameters[displayIndex].UI, parameterUIAnchor);
                        instance.Value = CurrentParametersList[currentParamIndex].Value /
                                         _itemData.DefaultParameters[defaultParamIndex].Value;
                        ParametersUI.Add(new DisplayParameter
                            { ID = _itemData.DisplayParameters[displayIndex].ID, UI = instance });
                    }
                }
        }

        protected void ModifyParameters(float deltaTime = 1)
        {
            foreach (var modify in _parametersToModify)
                if (CurrentParametersList.Contains(modify))
                {
                    var index = CurrentParametersList.IndexOf(modify);
                    var modifiedParam = CurrentParametersList[index];
                    if (modifiedParam.Value > 0)
                    {
                        modifiedParam.Value += modify.Value * deltaTime;
                        var defaultIndex = _itemData.DefaultParameters.IndexOf(modify);
                        modifiedParam.Value = Mathf.Clamp(modifiedParam.Value, 0,
                            _itemData.DefaultParameters[defaultIndex].Value);
                        CurrentParametersList[index] = modifiedParam;
                        var displayParameterIds = ParametersUI.Select(p => p.ID).ToList();
                        if (displayParameterIds.Contains(modifiedParam.ID))
                        {
                            var displayIndex = displayParameterIds.IndexOf(modifiedParam.ID);
                            ParametersUI[displayIndex].UI.Value =
                                modifiedParam.Value / _itemData.DefaultParameters[defaultIndex].Value;
                        }
                    }
                }
        }

        /// <summary>
        ///     現在のパラメーター量が必要パラメーター以下ならfalse、<br />
        ///     必要パラメーターより大きいならばtrueを返す。
        /// </summary>
        protected bool RequiredParameters()
        {
            foreach (var required in _requiredParameters)
                //  requiredと同じIDのパラメーターが含まれているか
                if (CurrentParametersList.Contains(required))
                {
                    var index = CurrentParametersList.IndexOf(required);
                    if (CurrentParametersList[index].Value <= required.Value) return false;
                }

            return true;
        }
    }
}
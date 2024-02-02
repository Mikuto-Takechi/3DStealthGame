using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public abstract class InstancedItem : MonoBehaviour
    {
        [SerializeField] protected ItemData _itemData;
        protected List<ItemParameter> CurrentParametersList;
        
        /// <summary>
        ///     アイテムのパラメーターを引き継ぐためのメソッド<br/>
        ///     Instantiate後に呼ぶ。その場合Awakeの後に実行される。
        /// </summary>
        public void InheritParameters(List<ItemParameter> inheritParametersList, bool callByReference = false)
        {
            if (!callByReference)
                CurrentParametersList = new(inheritParametersList);
            else
                CurrentParametersList = inheritParametersList;
        }
    }
}

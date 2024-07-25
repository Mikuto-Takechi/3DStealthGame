using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MonstersDomain
{
    public class Battery : EquippedItem, IConsumable
    {
        [SerializeField] ItemDataBase _itemDataBase;

        public bool Consume(Collection<(ItemId, List<ItemParameter>)> container)
        {
            var isConsumed = false;
            for (var i = 0; i < container.Count; i++)
            {
                //  インベントリのアイテムのパラメーターがnullなら次に飛ばす
                if (container[i].Item2 == null) continue;
                for (var j = 0; j < container[i].Item2.Count; j++)
                    //  インベントリの1つのアイテムの1つのパラメーターがこの消耗品が持つパラメーター変化リストに含まれていたら
                    if (_parametersToModify.Contains(container[i].Item2[j]))
                    {
                        var index = _parametersToModify.IndexOf(container[i].Item2[j]);
                        var modifiedParam = container[i].Item2[j];
                        modifiedParam.Value += _parametersToModify[index].Value;
                        var defaultParam = _itemDataBase[container[i].Item1].DefaultParameters
                            .First(p => p.Equals(container[i].Item2[j]));
                        if (modifiedParam.Value <= defaultParam.Value || container[i].Item2[j].Value <= 0)
                        {
                            //  パラメーターの変化を適用する
                            container[i].Item2[j] = modifiedParam;
                            isConsumed = true;
                        }
                        else
                        {
                            //  変化値がそのアイテムのパラメーターの最大値より大きければ消費せずに変化値も適用しない
                            return false;
                        }
                    }
            }

            return isConsumed;
        }
    }
}
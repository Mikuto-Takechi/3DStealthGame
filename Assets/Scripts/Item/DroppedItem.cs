using System.Collections.Generic;

namespace MonstersDomain
{
    public class DroppedItem : InstancedItem
    {
        void Awake()
        {
            //  SOのリストの値をコピーする。
            CurrentParametersList = new List<ItemParameter>(_itemData.DefaultParameters);
        }

        public void PickUp()
        {
            AudioManager.Instance.PlaySE(SE.Drop);
            InteractionMessage.Instance.WriteText("");
            if (_itemData.DefaultParameters.Count > 0)
                ItemManager.Instance.Add(_itemData.Id, CurrentParametersList);
            else
                ItemManager.Instance.Add(_itemData.Id);
            Destroy(gameObject);
        }
    }
}
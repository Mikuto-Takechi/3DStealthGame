using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonstersDomain
{
    public interface IConsumable
    {
        bool Consume(Collection<(ItemId, List<ItemParameter>)> container);
    }
}

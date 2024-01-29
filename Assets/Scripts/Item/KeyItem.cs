using UnityEngine;

namespace MonstersDomain
{
    public class KeyItem : MonoBehaviour, IPickable
    {
        public bool IsPickedUp { get; set; }
        public void PickUp()
        {
            print(gameObject.name + "を拾ったよ!");
        }
    }
}

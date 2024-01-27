using UnityEngine;

namespace MonstersDomain
{
    public abstract class Interactable : MonoBehaviour
    {
        protected abstract void Interact(Player player);
        protected abstract void Disengage(Player player);
        void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Player player))
            {
                Interact(player);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                Disengage(player);
            }
        }
    }
}

using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain
{
    public abstract class Interactable : MonoBehaviour
    {
        protected Parasite Parasite;
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                Interact(player);
            }
            else if (other.TryGetComponent(out Parasite parasite))
            {
                Parasite = parasite;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                Disengage(player);
            }
            else if(other.TryGetComponent(out Parasite _))
            {
                Parasite = null;
            }
        }

        protected abstract void Interact(Player player);
        protected abstract void Disengage(Player player);
    }
}
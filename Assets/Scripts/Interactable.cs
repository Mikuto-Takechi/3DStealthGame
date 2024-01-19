using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

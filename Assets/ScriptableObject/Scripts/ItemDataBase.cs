using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "ScriptableObjects/ItemDataBase", order = 1)]
public class ItemDataBase : ScriptableObject
{
    public Item[] Items;
}
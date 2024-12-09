using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public float damage;
    public float weight;
    public string[] properties;
    public float hungerFill;
    public float thirstFill;
}

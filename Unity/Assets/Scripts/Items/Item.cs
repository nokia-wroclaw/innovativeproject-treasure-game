
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public GameObject item;
    new public string name;
    public Sprite icon;

    public abstract void Use();

}

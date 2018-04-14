using System;
using System.Collections;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    new public string name;
    public Sprite icon;

    public abstract IEnumerator Use(Action<bool> result);

}

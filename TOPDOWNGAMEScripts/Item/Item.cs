using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拓展，该工程未使用
/// </summary>

public enum ItemType
{
    Consumable,
    Armor,
    Weapon,
}

public class Item
{
    public int id { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public string icon { get; private set; }

    public Item(int id,string name,string description,string icon)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.icon = icon;
    }
}

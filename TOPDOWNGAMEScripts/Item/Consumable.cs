using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item {

    //拓展，该工程未使用
    public int count;
    public int recovery;

    public Consumable(int id, string name, string description, string icon,int count,int recovery) : 
        base(id, name, description, icon)
    {
        this.count = count;
        this.recovery = recovery;
    }

   
}

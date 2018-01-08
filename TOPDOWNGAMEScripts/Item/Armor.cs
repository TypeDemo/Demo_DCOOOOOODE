using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item {

    //拓展，该工程未使用

    public int defence { get; private set; }

    public Armor(int id, string name, string description, string icon, int defence) :
        base(id, name, description, icon)
    {
        this.defence = defence;
    }


}

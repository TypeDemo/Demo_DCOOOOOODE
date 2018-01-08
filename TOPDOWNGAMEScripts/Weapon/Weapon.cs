using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType
{
    Melee,
    Shoot,
}

public abstract class Weapon : MonoBehaviour
{
    public GameObject enemyInfoUI;

    public Transform leftHandIK;
    public Transform rightHandIK;

    [HideInInspector]
    public WeaponType type;

    private Character _user;
    public Character user
    {
        get
        {
            if (_user == null)
                _user = GetComponent<Character>();
            if (!_user)
                _user = GetComponentInParent<Character>();
            return _user;
        }
    }
    
}

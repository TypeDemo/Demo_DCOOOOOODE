using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour {

    public float offset;

    public GameObject target;
    Slider slider;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.maxValue = target.GetComponent<PlayerCharacter>().startHP;
        slider.value = slider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        var pos = target.transform.position;
        transform.position = new Vector3(pos.x, pos.y + offset, pos.z);

        transform.LookAt(Camera.main.transform.position);

        slider.value = target.GetComponent<PlayerCharacter>().startHP;

    }
}

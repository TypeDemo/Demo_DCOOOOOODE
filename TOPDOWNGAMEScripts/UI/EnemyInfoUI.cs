using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoUI : MonoBehaviour
{
    public float offheight = 30.0f;
    private Text text;
    private Slider slider;
    private float time;
    private bool isShow = false;
    RectTransform trans;

    // Use this for initialization
    void Start()
    {
        text = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
        trans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isShow)
        {
            if (Time.time - time > 1.0f)
            {
                isShow = false;
                trans.position = new Vector2(trans.position. x, Screen.height+ offheight);
            }
        }

    }


    public void ShowInfo(string name, float maxHealth, float health)
    {
        time = Time.time;
        GetName(name);
        GetBloodLine(maxHealth, health);
        trans.position = new Vector2(trans.position.x, Screen.height - offheight);
        isShow = true;
    }

    private void GetName(string name)
    {
        text.text = "Zombie";
    }

    private void GetBloodLine(float maxHealth, float health)
    {
        slider.maxValue = maxHealth;
        slider.value = health;
    }
}

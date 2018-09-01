using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
   

    const float MAX_HP = 100f;
    const float MAX_LT = 100f;
    public static float lightLevel = 100f;
    public static float hp = 100f;
    public static int orbs = 0;
    public delegate void UIUpdate();
    public static UIUpdate update;
    public CharacterController player;
    static Stats instance;

    public static void AddOrbs()
    {
        orbs++;
        update.Invoke();
    }
    public static void HpManipulation(float amount)
    {

        hp -= amount;
        if(hp > MAX_HP)
        {
            hp = MAX_HP;
        }
        if(hp <= 0)
        {
            print("mitä vittuu");
            instance.player.Death();
            hp = 0;
        }
        update.Invoke();
    }
    public static void LightLevels(float amount)
    {
        lightLevel += amount;
        if(lightLevel > MAX_LT)
        {
            lightLevel = MAX_LT;
        }
        if(lightLevel <= 0)
        {
            lightLevel = 0;
        }
        update.Invoke();
    }
    public static float lightRatio
    {
        get
        {
            return lightLevel / MAX_LT;
        }
    }
    public static float hpRatio
    {
        get
        {
            return hp / MAX_HP;
        }
    }

    public Text orbDisplay;
    public Image lightDisplay;
    public Image hpDisplay;

    private void Start()
    {
        instance = this;
        update += UpdateUI;
        UpdateUI();
    }
    private void UpdateUI()
    {
        orbDisplay.text = "Orbs: " + orbs.ToString();
        lightDisplay.fillAmount = lightRatio;
        hpDisplay.fillAmount = hpRatio;
    }
    private void OnDisable()
    {
        update = null;
        lightLevel = MAX_LT;
        hp = MAX_HP;

    }
}

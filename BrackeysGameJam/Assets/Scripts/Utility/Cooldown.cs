using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Cooldown
{
    public float delay;
    private float c;
    public Cooldown(float _d, bool init = false)
    {
        delay = _d;
        c = init ? _d : 0;
    }
    public void CountDown()
    {
        c -= Time.deltaTime;
    }
    public void Reset()
    {
        c = delay;
    }
    public void Zero()
    {
        c = 0;
    }
    public void Set(float d)
    {
        c = d;
    }
    public bool TriggerReady(bool reset = true, float transformingTrigger=0f)
    {
        if(c <= 0)
        {
            if (reset)
            {
                if(transformingTrigger != 0f)
                {
                    c = transformingTrigger;
                }
                else
                {
                    c = delay;
                }
                
            }
          
            return true;
        }
        return false;
    }
	public bool LowerThan(float value)
    {
        return c < value;
    }
}

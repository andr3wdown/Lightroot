using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public SpriteRenderer[] lights;
    public LightType[] types;
    public Vector2[] multLimits = { new Vector2(80f, 100f), new Vector2(20f, 40f), new Vector2(2f, 4f) };
    public float universalMultiplier = 1f;
    float[] offsets;
    float[] multipliers;
    float value = 0;
    private void Start()
    {
        InitializeLights();
    }
    private void Update()
    {
        Fluctuation();
        value += Time.deltaTime * universalMultiplier;
    }
    void InitializeLights()
    {
        offsets = new float[lights.Length];
        multipliers = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            offsets[i] = Random.Range(-10000000f, 10000000f);
            switch (types[i])
            {
                case LightType.fast:
                    multipliers[i] = Random.Range(multLimits[2].x, multLimits[2].y);
                    break;

                case LightType.medium:
                    multipliers[i] = Random.Range(multLimits[1].x, multLimits[1].y);
                    break;

                case LightType.slow:
                    multipliers[i] = Random.Range(multLimits[0].x, multLimits[0].y);
                    break;
            }
        }
    }
    void Fluctuation()
    {
        for(int i = 0; i < lights.Length; i++)
        {
            Color c = lights[i].color;
            
            c.a = Mathf.Lerp(c.a, GetSinValue(i), 5f * Time.deltaTime);
            lights[i].color = c;
        }
    }
    float GetSinValue(int index)
    {
        return (((Mathf.Sin((Time.time * multipliers[index] + offsets[index])) + 1f) / 2f) + 0.5f) + 0.5f;
    }
    public enum LightType
    {
        slow,
        medium,
        fast
    }
}

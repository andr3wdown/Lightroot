using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSound : MonoBehaviour
{

    bool visible = true;
    private void OnBecameInvisible()
    {
        visible = false;
    }
    private void OnBecameVisible()
    {
        visible = true;
    }
    public void PlayBossSound()
    {
        if (visible)
        {
            AudioController.PlayAudio(transform.position, "bossStep");
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public SoundBank soundBank;
    const int START_AMOUNT = 5;
    static AudioController instance;
    bool isInitialized = false;
    List<AudioSource> speakers = new List<AudioSource>();
    GameObject parent;
    private void Start()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        soundBank.Init();
        GeneratePool();
        isInitialized = true;
    }
    void GeneratePool()
    {
        parent = new GameObject("SpeakerPool");
        for(int i = 0; i < START_AMOUNT; i++)
        {
            GameObject go = new GameObject("Speaker");
            go.transform.SetParent(parent.transform);
            AudioSource ac = go.AddComponent<AudioSource>();
            go.AddComponent<Speaker>();
            go.SetActive(false);
            speakers.Add(ac);
        }      
    }
    AudioSource GetFreeSpeaker()
    {
        for(int i = 0; i < speakers.Count; i++)
        {
            if (!speakers[i].gameObject.activeInHierarchy)
            {
                speakers[i].gameObject.SetActive(true);
                return speakers[i];
            }
        }
        GameObject go = new GameObject("Speaker");
        go.transform.SetParent(parent.transform);
        AudioSource ac = go.AddComponent<AudioSource>();
        go.AddComponent<Speaker>();
        speakers.Add(ac);
        return ac;
    }
    public static void PlayAudio(Vector3 pos, AudioClip audio, float volume = 1f, float pitch = 1f)
    {
        if (instance.isInitialized)
        {
            AudioSource ac = instance.GetFreeSpeaker();
            ac.transform.position = pos;
            ac.pitch = pitch;
            ac.volume = volume;
            ac.PlayOneShot(audio);
        }
      
    }
    public static void PlayAudio(Vector3 pos, string keyword, float volume = 1f, float pitch = 1f)
    {
        if (instance.isInitialized)
        {
            AudioSource ac = instance.GetFreeSpeaker();
            ac.transform.position = pos;
            ac.pitch = pitch;
            ac.volume = volume;
            ac.PlayOneShot(instance.soundBank.GetClip(keyword));
        }
    }
}

[System.Serializable]
public class SoundBank
{
    public SoundData soundData;
    Dictionary<string, List<AudioClip>> bank;
    bool initialized = false;
    public SoundBank(SoundData s)
    {
        soundData = s;
        bank = soundData.GetData();
        initialized = true;
    }
    public void Init()
    {
        bank = soundData.GetData();
        initialized = true;
    }
    public AudioClip GetClip(string keyword)
    {
        List<AudioClip> soundList;
        bank.TryGetValue(keyword, out soundList);
        if(soundList.Count > 0)
        {
            return soundList[Random.Range(0, soundList.Count - 1)];
        }
        Debug.LogError("Invalid keyword used in SoundBank.cs!");
        return null;
    }
    [System.Serializable]
    public class SoundData
    {
        public SoundObject[] clips;
        public Dictionary<string, List<AudioClip>> GetData()
        {
            Dictionary<string, List<AudioClip>> data = new Dictionary<string, List<AudioClip>>();
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].files.Count < 1)
                {
                    break;
                }
                data.Add(clips[i].name, clips[i].files);              
            }
            return data;
        }
        [System.Serializable]
        public class SoundObject
        {
            public string name;
            public List<AudioClip> files;
        }
    }
}


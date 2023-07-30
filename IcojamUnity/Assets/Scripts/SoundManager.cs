using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] sons;
    private AudioSource source;

    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            for (int i = 0; i < sons.Length; ++i)
                clips[sons[i].name] = sons[i];
            sons = null;
            source = GetComponent<AudioSource>();
        }
        else
            Destroy(this);
    }

    public static void Play(string name)
    {
        Instance.source.PlayOneShot(Instance.clips[name]);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

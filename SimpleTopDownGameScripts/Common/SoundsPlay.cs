using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


[Serializable]
public class SoundsPlay
{
    public enum Selection
    {
        //随机播放片段
        Randomly,

        //随机播放片段，但排除最后一个播放过的
        RandomlyButExcludeLast,

        //顺序播放片段
        InSequence
    }

    [SerializeField]
    private AudioClip[] sounds;

    [SerializeField]
    private Vector2 volumeRange = new Vector2(0.5f, 0.75f);

    [SerializeField]
    private Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    private int lastSoundPlayed;


    public void Play(Selection selectionMethod, AudioSource audioSource, float volumeFactor = 1f)
    {
        if (!audioSource || sounds.Length == 0)
            return;

        int clipToPlay = CalculateNextClipToPlay(selectionMethod);

        var volume = Random.Range(volumeRange.x, volumeRange.y) * volumeFactor;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

        audioSource.PlayOneShot(sounds[clipToPlay], volume);

        lastSoundPlayed = clipToPlay;
    }

    public void PlayAtPosition(Selection selectionMethod, Vector3 position, float volumeFactor = 1f)
    {
        if (sounds.Length == 0)
            return;

        int clipToPlay = CalculateNextClipToPlay(selectionMethod);

        AudioSource.PlayClipAtPoint(sounds[clipToPlay], position, Random.Range(volumeRange.x, volumeRange.y) * volumeFactor);

        lastSoundPlayed = clipToPlay;
    }

    public void Play2D(Selection selectionMethod = Selection.RandomlyButExcludeLast)
    {
        if (sounds.Length == 0)
            return;

        int clipToPlay = CalculateNextClipToPlay(selectionMethod);
        GameController.Instance.Play2D(sounds[clipToPlay], Random.Range(volumeRange.x, volumeRange.y));
    }

    private int CalculateNextClipToPlay(Selection selectionMethod)
    {
        int clipToPlay = 0;

        if (selectionMethod == Selection.Randomly || sounds.Length == 1)
            clipToPlay = Random.Range(0, sounds.Length);
        else if (selectionMethod == Selection.RandomlyButExcludeLast)
        {
            //将上次播放过的声音放在数组第一个
            AudioClip firstClip = sounds[0];
            sounds[0] = sounds[lastSoundPlayed];
            sounds[lastSoundPlayed] = firstClip;

            //随机播放时排除数组中的第一个
            clipToPlay = Random.Range(1, sounds.Length);
        }
        else if (selectionMethod == Selection.InSequence)
            clipToPlay = (int)Mathf.Repeat(lastSoundPlayed + 1, sounds.Length);

        return clipToPlay;
    }
}

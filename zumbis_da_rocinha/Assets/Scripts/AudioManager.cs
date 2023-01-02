using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager{
    public AudioSource source;
    public AudioClip[] sons;

    public void PlayCue(string som){
        int pos = int.Parse(som);
        if(pos >= 0 && pos < sons.Length)
            source.PlayOneShot(sons[pos]);
    }
}

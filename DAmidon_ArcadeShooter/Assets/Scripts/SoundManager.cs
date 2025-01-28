using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Sound Manager for playing a variety of sounds
public class SoundManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    [SerializeField] GameObject source;
    [SerializeField] Slider soundSlider;

    public void PlaySound(string soundName)
    {
        foreach (var sound in sounds)
        {
            if(sound.name == soundName) //Search for matching sound name
            {
                //Create and setup GO
                GameObject source_ = Instantiate(source, transform.position, Quaternion.identity);
                source_.name = "Sound: " + sound.name;
                AudioSource audioSource = source_.GetComponent<AudioSource>();

                //Set clip, volume, random pitch, and play
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume * soundSlider.value;

                //Makes sounds slightly less annoying
                float pitch = Random.Range(sound.pitch.x, sound.pitch.y);
                audioSource.pitch = pitch; 

                audioSource.Play();

                //Cleanup
                Destroy(source_, sound.length);
            }
        }
    }
}

//Sound Class
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float volume;
    public float length;
    public Vector2 pitch;
}

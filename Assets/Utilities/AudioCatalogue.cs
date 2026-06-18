using SLS.ListUtilities;
using UnityEngine;

public class AudioCatalogue : MonoBehaviour
{
    public AudioSource source;
    public DictionaryS<string, AudioClip> sounds;

    public void PlaySound(string soundName) => source.PlayOneShot(sounds[soundName]);
}
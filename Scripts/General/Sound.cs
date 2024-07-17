using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    public void PlaySound(AudioClip clip, float volume, float pitch, float delay, bool looped)
    { 
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.loop = looped;

        if (delay != 0)
            _audioSource.Play();
        else
            _audioSource.PlayDelayed(delay);
    }
}

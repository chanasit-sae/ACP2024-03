using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]private SoundLibrary sfxLibrary;
    [SerializeField]private AudioSource sfx2DSource;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            //AudioSource.PlayClipAtPoint(clip, pos);
            StartCoroutine(AnimateMusicCrossfade(clip,0.5f));
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }
    public void PlaySound3D(string soundName)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName),new Vector3(0,0,0));
    }

    public void PlaySound2D(string soundName)
    {
        //sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
        StartCoroutine(AnimateMusicCrossfade(sfxLibrary.GetClipFromName(soundName)));
    }
    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        if (percent < 1)
        {
            //percent += Time.deltaTime * 1 / fadeDuration;
            sfx2DSource.volume = Mathf.Lerp(1f, 0, percent);
        }

        sfx2DSource.clip = nextTrack;
        sfx2DSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            sfx2DSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}

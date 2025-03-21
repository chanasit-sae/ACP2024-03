using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    //public static MainMenu Instance;

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    [SerializeField]public string nextScene;

    //private void Awake()
    //{
        
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    private void Start()
    {

        LoadVolume();
        MusicManager.Instance.PlayMusic("MainMenu");
    }
    public void Play() 
    {
        gameController.Instance.stage = 1;
        gameController.Instance.score = 0;
        SceneManager.LoadScene(nextScene); //add game scene here boi
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic("Game");
        }
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip background;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip completeSound;
    [SerializeField] private AudioClip catchFood;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    public Slider MusicSlider;
    public Slider SfxSlider;


    [Header("Volume Settings")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        if (MusicSlider != null)
        {
            MusicSlider.value = musicVolume;
            MusicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (SfxSlider != null)
        {
            SfxSlider.value = sfxVolume;
            SfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        backgroundMusicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        PlayBackgroundMusic(background);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
        backgroundMusicSource.clip = clip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayClickSound() => PlaySFX(clickSound);//
    
    
   
    public void PlayCompleteSound() => PlaySFX(completeSound);
    public void PlayWinSound() => PlaySFX(winSound);
    public void PlayLoseSound() => PlaySFX(loseSound);
    public void PlayCatchSound() => PlaySFX(catchFood);

    public void UpdateVolume()
    {
        backgroundMusicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        backgroundMusicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = sfxVolume;
    }
}

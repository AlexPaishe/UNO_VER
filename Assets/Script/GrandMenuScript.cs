using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GrandMenuScript : MonoBehaviour
{
    private Animator anima;
    public GameObject[] Menu;
    public Slider MusicSlider;
    public Slider SoundSlider;
    private float MusicVolume = 0;
    private float SoundVolume = 0;
    public AudioSource Music;
    public AudioSource SoundCard;
    public Material BarMat;
    
    void Start()
    {
        anima = GetComponent<Animator>();
        BarMat.DisableKeyword("_EMISSION");
        MusicSlider.value = MusicVolume;
        SoundSlider.value = SoundVolume;
    }

    private void Awake()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicSound");
        SoundVolume = PlayerPrefs.GetFloat("SoundSound");
    }

    // Update is called once per frame
    void Update()
    {
        SoundSetting();
    }

    public void OpenDoor()
    {
        anima.SetTrigger("OpenDoor");
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()//Выход из игры
    {
        Application.Quit();
    }

    public void Author()
    {
        Debug.Log("Author");
        Menu[0].SetActive(false);
        Menu[1].SetActive(true);
    }

    public void Setting()
    {
        Menu[0].SetActive(false);
        Menu[3].SetActive(true);
    }

    public void Rules()
    {
        Menu[0].SetActive(false);
        Menu[2].SetActive(true);
    }

    public void Collection()
    {
        Menu[0].SetActive(false);
        Menu[4].SetActive(true);
    }

    public void BackMenu()
    {
        for(int i = 1;i<Menu.Length;i++)
        {
            Menu[i].SetActive(false);
        }
        Menu[0].SetActive(true);
    }

    private void SoundSetting()//Просчитывание настроек
    {
        Music.volume = PlayerPrefs.GetFloat("MusicSound");
        SoundCard.volume = PlayerPrefs.GetFloat("SoundSound");
    }

    public void MusicAction(float val)//Настрока Громкости Музыки
    {
        PlayerPrefs.SetFloat("MusicSound", val);
    }

    public void SoundAction(float val)//Настройка громкости звуков
    {
        PlayerPrefs.SetFloat("SoundSound", val);
    }

    public void OnPoint()
    {
        BarMat.EnableKeyword("_EMISSION");
    }

    public void OffPoint()
    {
        BarMat.DisableKeyword("_EMISSION");
    }
}

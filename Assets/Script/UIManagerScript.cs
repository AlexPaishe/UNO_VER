using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public Image[] IndicatorHotCard;//Индикатор карты на столе
    public Image[] IndicatorSaveCard;//Индикатор пропуска хода
    public float TimerBegin;//Время таймеров
    private float Timer;//Таймер пропуска хода
    private float PlusTimer;//Таймер взятия карты
    private bool goSave = false;//начало таймера пропуска хода
    private GameManagerScript game;//Игровой менеджер
    public Image[] ArrowTurn;//Картинки стрелок круга
    public Sprite[] Arrow;//Виды спрайтов стрелок круга
    private bool rotateArrow = false;//Повернут ли круг
    private SoundScript Sound; //Звуки
    public Text EndText;//Текст победы или поражения
    public GameObject End;//Меню окончания игры
    public GameObject Pause;//Меню паузы
    private bool PauseMenuOpen = false;//Включено ли меню паузы
    public GameObject Setting;//Меню настроек
    public Slider MusicSound;//Настрока громкости музыки
    public Slider SoundSound;//Настройка громкости звуков
    private float MusicVolume = 0;//Громкость музыки
    private float SoundVolume = 0;//Громкость звуков
    private bool endmatch = false;
    void Start()
    {
        game = FindObjectOfType<GameManagerScript>();

        Sound = FindObjectOfType<SoundScript>();

        for(int i = 0;i<IndicatorHotCard.Length;i++)
        {
            IndicatorHotCard[i].enabled = false;
        }

        for(int i = 0; i< IndicatorSaveCard.Length;i++)
        {
            IndicatorSaveCard[i].enabled = false;
        }
        Timer = TimerBegin;
        PlusTimer = TimerBegin;

        for(int i = 0; i< game.PlusText.Length; i++)
        {
            game.PlusText[i].text = "";
        }

        MusicSound.value = MusicVolume;
        SoundSound.value = SoundVolume;
    }

    private void Awake()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicSound");
        SoundVolume = PlayerPrefs.GetFloat("SoundSound");
    }

    // Update is called once per frame
    void Update()
    {
        HotCardInd();

        SaveCardInd();

        EndMatch();

        PauseMenu();

        SoundSetting();
    }

    private void FixedUpdate()
    {
        PlusCardIndicator();

        TurnArrow();
    }

    private void TurnArrow() // Поворот стрелок для отоброжения поворота круга
    {
        if (game.forward == true && rotateArrow == false)
        {
            for (int i = 0; i < ArrowTurn.Length; i++)
            {
                for (int j = 0; j < Arrow.Length; j++)
                {
                    if (j == (i * 2) + 1)
                    {
                        ArrowTurn[i].sprite = Arrow[j];
                    }
                }
            }
            rotateArrow = true;
        }
        else if(game.forward == false && rotateArrow == true)
        {
            for (int i = 0; i < ArrowTurn.Length; i++)
            {
                for (int j = 0; j < Arrow.Length; j++)
                {
                    if (j == i * 2)
                    {
                        ArrowTurn[i].sprite = Arrow[j];
                    }
                }
            }
            rotateArrow = false;
        }
    }

    private void PlusCardIndicator()//Отображение получения карт у игроков
    {
        if (game.plusCard[0] > 0 || game.plusCard[1] > 0 || game.plusCard[2] > 0 || game.plusCard[3] > 0)
        {
            PlusTimer -= Time.fixedDeltaTime;
            if (PlusTimer <= 0)
            {
                for (int i = 0; i < game.PlusText.Length; i++)
                {
                    game.PlusText[i].text = "";
                    game.plusCard[i] = 0;
                }
                PlusTimer = TimerBegin;
                Sound.GiveCardSound = true;
            }
        }
    }

    private void HotCardInd()//Отображение последней карты
    {
        for (int i = 0; i < IndicatorHotCard.Length; i++)
        {
            if (i == game.HotCard)
            {
                IndicatorHotCard[i].enabled = true;
            }
            else
            {
                IndicatorHotCard[i].enabled = false;
            }
        }
    }

    private void SaveCardInd() //Отображение пропуска хода
    {
        for (int i = 0; i < IndicatorSaveCard.Length; i++)
        {
            if (goSave == false)
            {
                if (i == game.SaveCard)
                {
                    IndicatorSaveCard[i].enabled = true;
                    goSave = true;
                }
                else
                {
                    IndicatorSaveCard[i].enabled = false;
                }
            }
        }
        if (goSave == true)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                game.SaveCard = 4;
                goSave = false;
                Timer = TimerBegin;
            }
        }
    }

    private void EndMatch()//Меню окончания матча
    {
        if(game.CurrentGame.FirstEnemyHand.Count == 0 || game.CurrentGame.SecondEnemyHand.Count == 0 ||
            game.CurrentGame.ThirdEnemyHand.Count == 0|| game.CurrentGame.PlayerHand.Count == 0)
        {
            game.go = false;
            endmatch = true;
            End.SetActive(true);
            if(game.CurrentGame.PlayerHand.Count == 0)
            {
                EndText.text = "Победа!!!";
            }
            else
            {
                EndText.text = " Поражение!!!";
            }
        }
    }

    private void PauseMenu()//Реализация меню паузы
    {
        if (endmatch == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenuOpen = !PauseMenuOpen;
            }

            if (PauseMenuOpen == true)
            {
                game.go = false;
                Pause.SetActive(true);
            }
            else
            {
                game.go = true;
                Pause.SetActive(false);
                Setting.SetActive(false);
            }
        }
    }

    public void SettingMenu()//Включение меню настроек
    {
        Setting.SetActive(true);
    }

    public void MusicAction( float val)//Настрока Громкости Музыки
    {
        PlayerPrefs.SetFloat("MusicSound", val);
    }

    public void SoundAction( float val)//Настройка громкости звуков
    {
        PlayerPrefs.SetFloat("SoundSound", val);
    }

    private void SoundSetting()//Просчитывание настроек
    {
        Sound.Music.volume = PlayerPrefs.GetFloat("MusicSound");
        Sound.SoundCard.volume = PlayerPrefs.GetFloat("SoundSound");
    }

    public void GrandMenu()//Возвращение в главное меню
    {
        SceneManager.LoadScene(0);
    }
}

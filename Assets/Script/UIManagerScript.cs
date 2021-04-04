using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Image[] ArrowTurn;
    public Sprite[] Arrow;
    void Start()
    {
        game = FindObjectOfType<GameManagerScript>();

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
    }

    // Update is called once per frame
    void Update()
    {      
        for(int i = 0; i < IndicatorHotCard.Length ; i++)
        {
            if(i == game.HotCard)
            {
                IndicatorHotCard[i].enabled = true;
            }
            else
            {
                IndicatorHotCard[i].enabled = false;
            }
        }

        for(int i = 0;i < IndicatorSaveCard.Length ; i++)
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
            if(Timer<=0)
            {
                game.SaveCard = 4;
                goSave = false;
                Timer = TimerBegin;
            }
        }
    }

    private void FixedUpdate()
    {
        if (game.plusCard[0] > 0 || game.plusCard[1] > 0 || game.plusCard[2] > 0 || game.plusCard[3] > 0)
        {
            PlusTimer -= Time.fixedDeltaTime;
            if(PlusTimer <= 0)
            {
                for (int i = 0; i < game.PlusText.Length; i++)
                {
                    game.PlusText[i].text = "";
                    game.plusCard[i] = 0;
                }
                PlusTimer = TimerBegin;
            }
        }

        if (game.forward == true)
        {
            for (int i = 0; i < ArrowTurn.Length; i++)
            {
                for (int j = 0; j < Arrow.Length; j++)
                {
                    if(j == (i*2)+1)
                    {
                        ArrowTurn[i].sprite = Arrow[j];
                    }
                }
            }
        }
        else
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
        }     
    }
}

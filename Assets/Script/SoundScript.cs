using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public AudioSource Music;//Музыка
    public AudioSource SoundCard;//Произведение звуков
    public AudioClip[] AllSound;//Все звуки
    public float TimerBegin;
    private float Timer;
    public bool GiveCardSound = false;

    private void Start()
    {
        Timer = TimerBegin;
        GiveCardSound = false;
    }

    public void SearchCardSound()//Звук при переходе с карты на карту
    {
        int search = Random.Range(4, 5);
        SoundCard.clip = AllSound[search];
        SoundCard.Play();
    }

    public void ArcherSound()//Звук победы лучника
    {
        SoundCard.clip = AllSound[9];
        SoundCard.Play();
    }

    public void MagicianSound()//Звук победы мага
    {
        SoundCard.clip = AllSound[10];
        SoundCard.Play();
    }

    public void WarriorSound()//Звук победы воина
    {
        SoundCard.clip = AllSound[7];
        SoundCard.Play();
    }

    public void AttackSound()//Звук выставления карты на стол
    {
        int Attack = Random.Range(2, 3);
        SoundCard.clip = AllSound[Attack];
        SoundCard.Play();
    }

    public void NecramagSound()//Звук призыва Некромага
    {
        SoundCard.clip = AllSound[11];
        SoundCard.Play();
    }

    public void DemiurgeSound()//Звук Призыва Демиурга
    {
        SoundCard.clip = AllSound[8];
        SoundCard.Play();
    }

    public void AssassinSound()//Звук презыва Ассассина
    {
        SoundCard.clip = AllSound[6];
        SoundCard.Play();
    }

    private void Update()
    {
        GiveCardSounds();
    }

    private void GiveCardSounds()//Звук получение карты
    {
        if (GiveCardSound == true)
        {
            int GiveCard = Random.Range(0, 1);
            SoundCard.clip = AllSound[GiveCard];
            SoundCard.Play();
            GiveCardSound = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardScript : MonoBehaviour
{
    public int Race; //Раса карты сброса
    public int Specialization;// Специализация карты сброса (лучник, воин, маг, ассассин, демиург или некромант)
    public int ForceCard; //Сила карты
    public Image BattleImage;//Картинка карты
    public Card SelfCard;//Все данные карты

    private void ShowCardInfo(Card card)//Получение карты при старте игры
    {
        SelfCard = card;
        Race = card.Race;
        Specialization = card.Specialization;
        ForceCard = card.ForceCard;
        BattleImage.sprite = card.Logo;
    }

    private void Start()
    {
        BattleImage = GetComponent<Image>();
        CardManagerScript cardMan = FindObjectOfType<CardManagerScript>();
        ShowCardInfo(CardManager.AllCards[Random.Range(0, cardMan.CardVariation.Length - 20)]);
        Debug.Log($" Раса {Race} Сила {ForceCard} Специализация {Specialization} картинка {BattleImage.sprite.name}");
    }
}

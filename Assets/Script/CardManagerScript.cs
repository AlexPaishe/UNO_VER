using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Card
{
    public Sprite Logo;//Картинка карты
    public int Race;//Расса карты
    public int ForceCard;//Сила карты
    public int Specialization;//Специализация

    public Card(Sprite logo, int race, int forcecard, int specialization)//Конструктор карты
    {
        Logo = logo;
        Race = race;
        ForceCard = forcecard;
        Specialization = specialization;
    }
}

public static class CardManager
{
    public static List<Card> AllCards = new List<Card>();//Список карты
}

public class CardManagerScript : MonoBehaviour
{
    public Sprite[] CardVariation;//Все виды картинок карты

    private void Awake()
    {
        #region Реализация создания колоды всех карт
        int count = 1;
        int number = 1;
        int spec = 1;
        for (int i = 0; i < CardVariation.Length; i++)
        {
            if (i < spec * 18 && spec < 5 && i < CardVariation.Length - 2)
            {
                if (i < (count * 6) + ((spec - 1) * 18))
                {
                    if (count == 1)
                    {
                        CardManager.AllCards.Add(new Card(CardVariation[i], count, number, spec));
                        //Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
                        number++;
                    }
                    else if (count > 1)
                    {
                        number++;
                        CardManager.AllCards.Add(new Card(CardVariation[i], count, number, spec));
                        //Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
                    }
                }
                else if (i == (count * 6) + ((spec - 1) * 18))
                {
                    count++;
                    number = 1;
                    CardManager.AllCards.Add(new Card(CardVariation[i], count, number, spec));
                    //Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
                }
            }
            else if (i == spec * 18 && spec < 5 && i < CardVariation.Length - 2)
            {
                count = 1;
                number = 1;
                spec++;
                CardManager.AllCards.Add(new Card(CardVariation[i], count, number, spec));
                //Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
                number++;
            }
            else if (i == CardVariation.Length - 2)
            {
                count = 4;
                CardManager.AllCards.Add(new Card(CardVariation[i], count, 0, 0));
                Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
            }
            else if (i == CardVariation.Length - 1)
            {
                count = 5;
                CardManager.AllCards.Add(new Card(CardVariation[i], count, 0, 0));
                Debug.Log($" Номер {i} Картинка {CardVariation[i].name} Раса {count} Сила {number} сециализация {spec}");
            }
        }
        #endregion
    }
}

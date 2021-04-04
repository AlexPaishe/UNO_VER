using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public int Race;//Расса карты
    public int Specialization;//Специализация карты
    public int ForceCard;//Сила карты
    private BattleCardScript BattleCard;//Карты сброса
    private int click = 0;//Количество кликов на данный момент
    public float timerBegin;//Время между кликами
    private float timer;//таймер для реализация двойного клика
    private Image CardImage;//Картинка карты
    private bool go;//Может ли карта быть разыгранной в данный момент или нет
    public Image LightIndicator;//Подсветка карт, которая может быть разыграна
    private Button but;//Кнопка карты
    public Card SelfCard;//Все данные карты
    private GameManagerScript game;//Менеджер игры
    private Transform DefaultParent;//Трансформ родителя
    private HorizontalLayoutGroup hor;//переменная компонента который раставляет карты
    private Vector3 ScaleCard;
    private Vector3 transformCard;

    private void Start()
    {
        timer = timerBegin;
        but = GetComponent<Button>();
        CardImage = GetComponent<Image>();
        BattleCard = FindObjectOfType<BattleCardScript>();
        CardManagerScript cardMan = FindObjectOfType<CardManagerScript>();       
        game = FindObjectOfType<GameManagerScript>();
        DefaultParent = transform.parent;
        hor = FindObjectOfType<HorizontalLayoutGroup>();
        ScaleCard = transform.localScale;
        transformCard = transform.parent.position;

        #region Присваивание рандомного значения карты, в зависимости от хода и введение ее в список руки игрока
        //if (game.Road < game.assassinStart)
        //{
        //    ShowCardInfo(CardManager.AllCards[Random.Range(0, cardMan.CardVariation.Length - 20)]);
        //}
        //else
        //{
        //    ShowCardInfo(CardManager.AllCards[Random.Range(0, cardMan.CardVariation.Length)]);
        //}
        ShowCardInfo(CardManager.AllCards[Random.Range(0, cardMan.CardVariation.Length)]);
        game.CurrentGame.PlayerHand.Add(SelfCard);
        if (game.Road == 1)
        {
            game.CurrentGame.PlayerHand.RemoveAt(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (game.Turn == 0)
        {
            Battle();
            if (go == true)
            {
                but.interactable = true;
                LightIndicator.color = new Color(1, 1, 1, 0.2f);
                //if (click == 1)
                //{
                //    timer -= Time.fixedDeltaTime;
                //    if (timer < 0)
                //    {
                //        click = 0;
                //        timer = timerBegin;
                //    }
                //}
                if (click == 1)
                {
                    if (Specialization == 1 && BattleCard.Specialization == 3 ||
                        Specialization == 2 && BattleCard.Specialization == 1 ||
                        Specialization == 3 && BattleCard.Specialization == 2)
                    {
                        SpecCard();
                    }
                    else if(Specialization == 4 && Race < 4)
                    {
                        AssassinCard();
                    }
                    else if (Specialization < 4 && Race< 4)
                    {
                        game.HotCard = 0;
                    }
                    else if(Race == 4)
                    {
                        MegaCard(2);
                    }
                    else if(Race == 5)
                    {
                        MegaCard(4);
                    }
                    BattleCard.BattleImage.sprite = CardImage.sprite;
                    BattleCard.BattleImage.color = new Color(1, 1, 1, 1);
                    BattleCard.Race = Race;
                    BattleCard.Specialization = Specialization;
                    BattleCard.ForceCard = ForceCard;
                    if (Specialization != 4)
                    {
                        game.ChangeTurn();
                    }
                    //Debug.Log($" Раса {BattleCard.Race} Сила {BattleCard.ForceCard} картинка {BattleCard.BattleImage.sprite.name}");
                    for(int i = 0;i<game.CurrentGame.PlayerHand.Count;i++)
                    {
                        if(CardImage.sprite == game.CurrentGame.PlayerHand[i].Logo)
                        {
                            game.CurrentGame.PlayerHand.RemoveAt(i);
                            break;
                        }
                    }                   
                    Destroy(gameObject);
                }
            }
            else
            {
                LightIndicator.color = new Color(1, 1, 1, 0);
                but.interactable = false;
            }
        }
        else
        {
            go = false;
            LightIndicator.color = new Color(1, 1, 1, 0);
            but.interactable = false;
        }
    }

    public void ClickCard()//Реализация клика
    {
        click++;
    }

    private void Battle()//Реализация возможности разыгрывания карт
    {
        if(Race == 4||Race == 5 || BattleCard.Race == 4|| BattleCard.Race == 5)
        {
            go = true;
        }
        else if (Race == BattleCard.Race && BattleCard.ForceCard == 1 && ForceCard == 6)
        {
            go = false;
        }
        else if (Race == BattleCard.Race && BattleCard.ForceCard == 6 && ForceCard == 1)
        {
            go = true;
        }
        else if(Race == BattleCard.Race && ForceCard > BattleCard.ForceCard)
        {
            go = true;
        }
        else if(Race == BattleCard.Race&& ForceCard == BattleCard.ForceCard)
        {
            go = false;
        }
        else if(Race == 1 && BattleCard.Race == 3||Race == 2&& BattleCard.Race ==1||Race == 3 && BattleCard.Race == 2)
        {
            go = true;
        }
        else if(Race == 3 && BattleCard.Race == 1||Race == 1 && BattleCard.Race == 2|| Race == 2 && BattleCard.Race == 1)
        {
            go = false;
        }
    }

    public void ShowCardInfo(Card card)//Реализация присваивания наминала карты
    {
        SelfCard = card;
        Race = card.Race;
        Specialization = card.Specialization;
        ForceCard = card.ForceCard;
        CardImage.sprite = card.Logo;
    }

    private void SpecCard()//Реализация получение карт при пробитии специализации
    {
        if (game.HotCard == 0)
        {
            for (int i = 0; i < game.fine; i++)
            {
                game.GiveCardToHand();
                game.plusCard[0]++;
            }
            game.HotCard = 0;
            game.PlusText[0].text = $"+{game.plusCard[0]}";
        }
        else if (game.HotCard == 1)
        {
            for (int i = 0; i< game.fine; i++)
            {
                if (game.Road < game.assassinStart)
                {
                    game.CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    game.CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
                game.plusCard[1]++;
            }
            game.FirstText.text = $"{game.CurrentGame.FirstEnemyHand.Count}";
            game.PlusText[1].text = $"+{game.plusCard[1]}";
            game.HotCard = 0;
        }
        else if (game.HotCard == 2)
        {
            for (int i = 0; i < game.fine; i++)
            {
                if (game.Road < game.assassinStart)
                {
                    game.CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    game.CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
                game.plusCard[2]++;
            }
            game.SecondText.text = $"{game.CurrentGame.SecondEnemyHand.Count}";
            game.PlusText[2].text = $"+{game.plusCard[2]}";
            game.HotCard = 0;
        }
        else if (game.HotCard == 3)
        {
            for (int i = 0; i < game.fine; i++)
            {
                if (game.Road < game.assassinStart)
                {
                    game.CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    game.CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
                game.plusCard[3]++;
            }
            game.ThirdText.text = $"{game.CurrentGame.ThirdEnemyHand.Count}";
            game.PlusText[3].text = $"+{game.plusCard[3]}";
            game.HotCard = 0;
        }
        else if (game.HotCard == 4)
        {
            game.HotCard = 0;
        }
    }

    private void AssassinCard() //Реализация штрафа Ассассина
    {
        if(game.forward == true)
        {
            //for (int i = 0; i < game.fine; i++)
            //{
            //    game.CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
            //}
            game.Turn = 2;
            game.forward = false;
        }
        else
        {
            //for (int i = 0; i < game.fine; i++)
            //{
            //    game.CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
            //}
            game.Turn = 2;
            game.forward = true;
        }
        game.HotCard = 0;
    }

    private void MegaCard(int number) //Реализация штрафа легендарных карт
    {
        if (game.HotCard == 0)
        {
            for (int i = 0; i < (game.fine * number); i++)
            {
                game.GiveCardToHand();
                game.plusCard[0]++;
            }
            game.PlusText[0].text = $"+{game.plusCard[0]}";
        }
        else if (game.HotCard == 1)
        {
            for (int i = 0; i < (game.fine * number); i++)
            {
                game.CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                game.plusCard[1]++;
            }
            game.FirstText.text = $"{game.CurrentGame.FirstEnemyHand.Count}";
            game.PlusText[1].text = $"+{game.plusCard[1]}";
        }
        else if (game.HotCard == 2)
        {
            for (int i = 0; i < (game.fine * number); i++)
            {
                game.CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                game.plusCard[2]++;
            }
            game.SecondText.text = $"{game.CurrentGame.SecondEnemyHand.Count}";
            game.PlusText[2].text = $"+{game.plusCard[2]}";
        }
        else if (game.HotCard == 3)
        {
            for (int i = 0; i < (game.fine * number); i++)
            {
                game.CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                game.plusCard[3]++;
            }
            game.ThirdText.text = $"{game.CurrentGame.ThirdEnemyHand.Count}";
            game.PlusText[3].text = $"+{game.plusCard[3]}";
        }
        else if(game.HotCard == 4)
        {

        }
        game.HotCard = 0;
    }

    public void OnPoint()//Действия при наведении курсора на карту
    {
        if (game.Turn == 0)
        {
            Vector3 UP = transform.position;
            UP.y += 50;
            transform.position = UP;
            Vector3 ScaleUP = transform.localScale;
            ScaleUP.x *= 1.4f;
            ScaleUP.y *= 1.4f;
            transform.localScale = ScaleUP;
            hor.enabled = false;
            transform.SetParent(DefaultParent.parent);
        }
    }

    public void OffPoint()//Действия при выходи курсора из зоны карты
    {
        if (game.Turn == 0)
        {
            Vector3 UP = transform.position;
            UP.y = transformCard.y;
            transform.position = UP;
            transform.localScale = ScaleCard;
            transform.SetParent(DefaultParent);
        }
    }
}

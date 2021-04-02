using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game
{
    public List<Card> PlayerHand, FirstEnemyHand, SecondEnemyHand, ThirdEnemyHand; //Рука Игрока, и двух соперников

    public Game()
    {
        #region Реализация начальных карт всех соперников
        FirstEnemyHand = GiveDeckCard();
        SecondEnemyHand = GiveDeckCard();
        ThirdEnemyHand = GiveDeckCard();
        PlayerHand = GiveDeckCard();
        #endregion
    }

    List<Card>GiveDeckCard()//Реализация начальных карт в одной руке
    {
        List<Card> list = new List<Card>();
        for(int i = 0; i<7;i++)
        {
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count-20)]);
        }
        return list;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public Game CurrentGame; //Менеджер по рукам играков
    public Transform PlayerDeck; //Рука игрока
    public GameObject CardPref; //Префаб карты
    public bool forward = true; //Направление движения круга
    public int Turn = 0; // Чья очередь играть
    private BattleCardScript battle; // Карты сброса
    public float timerBegin; // Время на обдумывание врагов
    private float timer; // таймер на обдумывание врагов

    public Text FirstText; // Количество карт первого врага
    public Text SecondText; // Количесво карт второго врага
    public Text ThirdText; // Количество карт третьего врага 
    public int Road = 1; // Количество кругов
    public int fine = 1; // Штраф за пробитие специализации
    public int assassinStart; //Круг с которого появляются Ассассины

    void Start()
    {
        CurrentGame = new Game();

        GiveHandCards(CurrentGame.PlayerHand, PlayerDeck);

        battle = FindObjectOfType<BattleCardScript>();

        timer = timerBegin;
    }

    private void GiveHandCards(List<Card> deck, Transform hand) // РЕализация появление начальных карт в руке Игрока
    {
        for(int i = 0;i<7;i++)
        {
            GiveCardToHand(hand);
        }
    }

    private void GiveCardToHand (Transform hand)// Реализация появления карты в руке Игрока
    {
        GameObject cardGo = Instantiate(CardPref, hand, false);
    }

    public void ChangeTurn()//РЕализация движение круга
    {
        if (forward == true)
        {
            Turn++;
            if (Turn > 3)
            {
                Turn = 0;
            }
        }
        else
        {
            Turn--;
            if (Turn < 0)
            {
                Turn = 3;
            }
        }
    }

    private void Update()
    {
        #region Реализация времени на обдумывания игроков и получение карт при отсутствия возможности играть
        if (Turn == 0)
        {
            for(int i = 0;i<CurrentGame.PlayerHand.Count;i++)
            {
                //Debug.Log(CurrentGame.PlayerHand[i].Logo);
            }
            PlayerCard();
        }
        if(Turn == 1)
        {
            timer -= Time.deltaTime;
            if(timer <0)
            {
                FirstEnemy();
                timer = timerBegin;
            }
        }
        if(Turn == 2)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                SecondEnemy();
                timer = timerBegin;
            }
        }
        if(Turn == 3)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                ThirdEnemy();
                timer = timerBegin;
                Road++;
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if(CurrentGame.FirstEnemyHand.Count == 0||CurrentGame.PlayerHand.Count == 0|| CurrentGame.SecondEnemyHand.Count == 0||CurrentGame.ThirdEnemyHand.Count == 0)
        {
            Restart();
        }
    }

    #region Реализация ИИ противников
    private void SecondEnemy() // Реализация ИИ Второго игрока
    {
        int count = 0;
        int Assassin = 0;
        for (int i = 0; i < CurrentGame.SecondEnemyHand.Count; i++)
        {
            if (CurrentGame.SecondEnemyHand[i].Race == battle.Race && CurrentGame.SecondEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
                CurrentGame.SecondEnemyHand[i].Race == battle.Race && CurrentGame.SecondEnemyHand[i].ForceCard ==6 && battle.ForceCard != 1 &&
                CurrentGame.SecondEnemyHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.SecondEnemyHand[i].Race == battle.Race && CurrentGame.SecondEnemyHand[i].ForceCard !=6 && CurrentGame.SecondEnemyHand[i].ForceCard > battle.ForceCard||
                CurrentGame.SecondEnemyHand[i].Race == 1 && battle.Race == 3 || CurrentGame.SecondEnemyHand[i].Race == 2 && battle.Race == 1 ||
                CurrentGame.SecondEnemyHand[i].Race == 3 && battle.Race == 2)
            {
                if (CurrentGame.SecondEnemyHand[i].Specialization == 1 && battle.Specialization == 3 ||
                    CurrentGame.SecondEnemyHand[i].Specialization == 2 && battle.Specialization == 1 ||
                    CurrentGame.SecondEnemyHand[i].Specialization == 3 && battle.Specialization == 2)
                {
                    SpecCard(2);
                }
                else if(CurrentGame.SecondEnemyHand[i].Specialization == 4)
                {
                    AssassinCard(2);
                    Assassin++;
                }
                battle.Race = CurrentGame.SecondEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.SecondEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.SecondEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.SecondEnemyHand[i].Specialization;

                count++;
                //Debug.Log($" Раса {battle.Race} Сила {battle.ForceCard} картинка {battle.BattleImage.sprite.name}");
                CurrentGame.SecondEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count==0)
        {
            for (int i = 0; i < fine; i++)
            {
                if (Road < assassinStart)
                {
                    CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
            }
        }
        if (Assassin == 0)
        {
            ChangeTurn();
        }
        SecondText.text = $"{CurrentGame.SecondEnemyHand.Count}";
    }

    private void FirstEnemy() //Реализация ИИ Первого Игрока
    {
        int count = 0;
        int Assassin = 0;
        for (int i = 0; i < CurrentGame.FirstEnemyHand.Count; i++)
        {
            if (CurrentGame.FirstEnemyHand[i].Race == battle.Race && CurrentGame.FirstEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
                CurrentGame.FirstEnemyHand[i].Race == battle.Race && CurrentGame.FirstEnemyHand[i].ForceCard == 6 && battle.ForceCard != 1 &&
                CurrentGame.FirstEnemyHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.FirstEnemyHand[i].Race == battle.Race && CurrentGame.FirstEnemyHand[i].ForceCard !=6 && CurrentGame.FirstEnemyHand[i].ForceCard > battle.ForceCard||
                CurrentGame.FirstEnemyHand[i].Race == 1 && battle.Race == 3 || CurrentGame.FirstEnemyHand[i].Race == 2 && battle.Race == 1 ||
                CurrentGame.FirstEnemyHand[i].Race == 3 && battle.Race == 2)
            {
                if (CurrentGame.FirstEnemyHand[i].Specialization == 1 && battle.Specialization == 3 ||
                    CurrentGame.FirstEnemyHand[i].Specialization == 2 && battle.Specialization == 1 ||
                    CurrentGame.FirstEnemyHand[i].Specialization == 3 && battle.Specialization == 2)
                {
                    SpecCard(1);
                }
                else if(CurrentGame.FirstEnemyHand[i].Specialization == 4)
                {
                    AssassinCard(1);
                    Assassin++;
                }
                battle.Race = CurrentGame.FirstEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.FirstEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.FirstEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.FirstEnemyHand[i].Specialization;
                count++;
                //Debug.Log($" Раса {battle.Race} Сила {battle.ForceCard} картинка {battle.BattleImage.sprite.name}");
                CurrentGame.FirstEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                if (Road < assassinStart)
                {
                    CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
            }
        }
        if (Assassin == 0)
        {
            ChangeTurn();
        }
        FirstText.text = $"{CurrentGame.FirstEnemyHand.Count}";
    }

    private void ThirdEnemy() // Реализаця ИИ Третьего Игрока
    {
        int count = 0;
        int Assassin = 0;
        for (int i = 0; i < CurrentGame.ThirdEnemyHand.Count; i++)
        {
            if (CurrentGame.ThirdEnemyHand[i].Race == battle.Race && CurrentGame.ThirdEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
                CurrentGame.ThirdEnemyHand[i].Race == battle.Race && CurrentGame.ThirdEnemyHand[i].ForceCard == 6 && battle.ForceCard != 1 &&
                CurrentGame.ThirdEnemyHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.ThirdEnemyHand[i].Race == battle.Race && CurrentGame.ThirdEnemyHand[i].ForceCard !=6 && CurrentGame.ThirdEnemyHand[i].ForceCard > battle.ForceCard||
                CurrentGame.ThirdEnemyHand[i].Race == 1 && battle.Race == 3 || CurrentGame.ThirdEnemyHand[i].Race == 2 && battle.Race == 1 ||
                CurrentGame.ThirdEnemyHand[i].Race == 3 && battle.Race == 2)
            {
                if(CurrentGame.ThirdEnemyHand[i].Specialization == 1 && battle.Specialization ==3||
                    CurrentGame.ThirdEnemyHand[i].Specialization == 2 && battle.Specialization == 1 ||
                    CurrentGame.ThirdEnemyHand[i].Specialization == 3 && battle.Specialization == 2)
                {
                    SpecCard(3);
                }
                else if(CurrentGame.ThirdEnemyHand[i].Specialization == 4)
                {
                    AssassinCard(3);
                    Assassin++;
                }
                battle.Race = CurrentGame.ThirdEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.ThirdEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.ThirdEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.ThirdEnemyHand[i].Specialization;
                count++;
                //Debug.Log($" Раса {battle.Race} Сила {battle.ForceCard} картинка {battle.BattleImage.sprite.name}");
                CurrentGame.ThirdEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                if (Road < assassinStart)
                {
                    CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                }
                else
                {
                    CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                }
            }
        }
        if (Assassin == 0)
        {
            ChangeTurn();
        }
        ThirdText.text = $"{CurrentGame.ThirdEnemyHand.Count}";
    }

    #endregion

    private void PlayerCard() //Реализация получения карт при отсутвии возможности ходить для Игрока
    {
        int count = 0;
        for (int i = 0; i < CurrentGame.PlayerHand.Count; i++)
        {
            if (CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
                CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard == 6 && battle.ForceCard != 1 &&
                CurrentGame.PlayerHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard != 6 && CurrentGame.PlayerHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.PlayerHand[i].Race == 1 && battle.Race == 3 || CurrentGame.PlayerHand[i].Race == 2 && battle.Race == 1 ||
                CurrentGame.PlayerHand[i].Race == 3 && battle.Race == 2)
            {
                count++;
            }
        }
        Debug.Log($"Круг {Road} число карт {count}");
        if (count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                GiveCardToHand(PlayerDeck);
            }
            ChangeTurn();
        }
    }

    public void Restart() // Реализация Переигрывания партии
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SpecCard(int number) // Реализация получения карт при пробитии специализации
    {
        if(number == 1)
        {
            if(forward == true)
            {
                for (int i = 0; i < fine; i++)
                {
                    GiveCardToHand(PlayerDeck);
                }
            }
            else
            {
                for (int i = 0; i < fine; i++)
                {
                    if (Road < assassinStart)
                    {
                        CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                    }
                    else
                    {
                        CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                    }
                }
                SecondText.text = $"{CurrentGame.SecondEnemyHand.Count}";
            }
        }
        else if (number == 2)
        {
            if (forward == true)
            {
                for (int i = 0; i < fine; i++)
                {
                    if (Road < assassinStart)
                    {
                        CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                    }
                    else
                    {
                        CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                    }
                }
                FirstText.text = $"{CurrentGame.FirstEnemyHand.Count}";
            }
            else
            {
                for (int i = 0; i < fine; i++)
                {
                    if (Road < assassinStart)
                    {
                        CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                    }
                    else
                    {
                        CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                    }
                }
                ThirdText.text = $"{CurrentGame.ThirdEnemyHand.Count}";
            }
        }
        else if(number == 3)
        {
            if(forward == true)
            {
                for (int i = 0; i < fine; i++)
                {
                    if (Road < assassinStart)
                    {
                        CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count - 20)]);
                    }
                    else
                    {
                        CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                    }
                }
                SecondText.text = $"{CurrentGame.SecondEnemyHand.Count}";
            }
            else
            {
                for (int i = 0; i < fine; i++)
                {
                    GiveCardToHand(PlayerDeck);
                }
            }
        }
    }

    private void AssassinCard(int number)
    {
        if(number == 1)
        {
            if(forward == true)
            {
                Turn = 3;
                forward = false;
            }
            else
            {
                Turn = 3;
                forward = true;
            }
        }
        else if (number == 2)
        {
            if (forward == true)
            {
                Turn = 0;
                forward = false;
            }
            else
            {
                Turn = 0;
                forward = true;
            }
        }
        else if(number == 3)
        {
            if (forward == true)
            {
                Turn = 1;
                forward = false;
            }
            else
            {
                Turn = 1;
                forward = true;
            }
        }
    } //Реализация штрафа Ассасина

    public void Quit()
    {
        Application.Quit();
    }
}

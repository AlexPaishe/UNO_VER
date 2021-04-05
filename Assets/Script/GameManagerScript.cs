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
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
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
    public int HotCard = 4; // Чья карта на столе
    public int SaveCard = 4; // Кто последний взял карту
    private BattleCardScript battle; // Карты сброса
    public float timerBegin; // Время на обдумывание врагов
    private float timer; // таймер на обдумывание врагов

    public Text FirstText; // Количество карт первого врага
    public Text SecondText; // Количесво карт второго врага
    public Text ThirdText; // Количество карт третьего врага 
    public Text[] PlusText;//Количество прибавляемых карт
    public int [] plusCard;//Количество получаемых карт
    public int Road = 1; // Количество кругов
    public int fine = 1; // Штраф за пробитие специализации
    private SoundScript Sound;//Звуки
    public bool go = true;//Могут ли враги играть

    void Start()
    {
        CurrentGame = new Game();

        GiveHandCards();

        battle = FindObjectOfType<BattleCardScript>();

        timer = timerBegin;

        Sound = FindObjectOfType<SoundScript>();

        for(int i = 0; i< plusCard.Length;i++)
        {
            plusCard[i] = 0;
        }
    }

    private void GiveHandCards() // РЕализация появление начальных карт в руке Игрока
    {
        for(int i = 0;i<7;i++)
        {
            GiveCardToHand();
        }
    }

    public void GiveCardToHand()// Реализация появления карты в руке Игрока
    {
        GameObject cardGo = Instantiate(CardPref, PlayerDeck, false);
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
        if (go == true)
        {
            if (Turn == 0)
            {
                for (int i = 0; i < CurrentGame.PlayerHand.Count; i++)
                {
                    //Debug.Log(CurrentGame.PlayerHand[i].Logo);
                }
                PlayerCard();
            }
            if (Turn == 1)
            {
                FirstText.color = Color.red;
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    FirstEnemy();
                    timer = timerBegin;
                    FirstText.color = Color.white;
                }
            }
            if (Turn == 2)
            {
                SecondText.color = Color.red;
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    SecondEnemy();
                    timer = timerBegin;
                    SecondText.color = Color.white;
                }
            }
            if (Turn == 3)
            {
                ThirdText.color = Color.red;
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    ThirdEnemy();
                    timer = timerBegin;
                    ThirdText.color = Color.white;
                    Road++;
                }
            }
        }
        #endregion
    }

    #region Реализация ИИ противников

    private void SecondEnemy() // Реализация ИИ Второго игрока
    {
        int count = 0;
        int Assassin = 0;
        for (int i = 0; i < CurrentGame.SecondEnemyHand.Count; i++)
        {
            if (CurrentGame.SecondEnemyHand[i].Race == 4 || CurrentGame.SecondEnemyHand[i].Race == 5 || battle.Race == 4 || battle.Race == 5||
                CurrentGame.SecondEnemyHand[i].Race == battle.Race && CurrentGame.SecondEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
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
                    SpecCardSound(2, i);
                }
                else if(CurrentGame.SecondEnemyHand[i].Specialization == 4 && CurrentGame.SecondEnemyHand[i].Race < 4)
                {
                    AssassinCard(2);
                    Assassin++;
                }
                else if(CurrentGame.SecondEnemyHand[i].Race == 4)
                {
                    MegaCard(2);
                    HotCard = 2;
                    LegendaryCardSound(2, i);
                }
                else if(CurrentGame.SecondEnemyHand[i].Race == 5)
                {
                    MegaCard(4);
                    HotCard = 2;
                    LegendaryCardSound(2, i);
                }
                else if (CurrentGame.SecondEnemyHand[i].Specialization < 4 && CurrentGame.SecondEnemyHand[i].Race < 4)
                {
                    HotCard = 2;
                    Sound.AttackSound();
                }

                battle.Race = CurrentGame.SecondEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.SecondEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.SecondEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.SecondEnemyHand[i].Specialization;

                count++;
                CurrentGame.SecondEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count==0)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[2]++;
            }
            PlusText[2].text = $"+{plusCard[2]}";
            SaveCard = 2;
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
            if (CurrentGame.FirstEnemyHand[i].Race == 4 || CurrentGame.FirstEnemyHand[i].Race == 5 || battle.Race == 4 || battle.Race == 5||
                CurrentGame.FirstEnemyHand[i].Race == battle.Race && CurrentGame.FirstEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
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
                    SpecCardSound(1, i);
                }
                else if(CurrentGame.FirstEnemyHand[i].Specialization == 4 && CurrentGame.FirstEnemyHand[i].Race < 4)
                {
                    AssassinCard(1);
                    Assassin++;
                }
                else if(CurrentGame.FirstEnemyHand[i].Race == 4)
                {
                    MegaCard(2);
                    HotCard = 1;
                    LegendaryCardSound(1, i);
                }
                else if(CurrentGame.FirstEnemyHand[i].Race == 5)
                {
                    MegaCard(4);
                    HotCard = 1;
                    LegendaryCardSound(1, i);
                }
                else if (CurrentGame.FirstEnemyHand[i].Specialization < 4 && CurrentGame.FirstEnemyHand[i].Race < 4)
                {
                    HotCard = 1;
                    Sound.AttackSound();
                }

                battle.Race = CurrentGame.FirstEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.FirstEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.FirstEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.FirstEnemyHand[i].Specialization;
                count++;
                CurrentGame.FirstEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[1]++;
            }
            SaveCard = 1;
            PlusText[1].text = $"+{plusCard[1]}";
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
            if (CurrentGame.ThirdEnemyHand[i].Race == 4 || CurrentGame.ThirdEnemyHand[i].Race == 5|| battle.Race == 4 || battle.Race == 5 ||
                CurrentGame.ThirdEnemyHand[i].Race == battle.Race && CurrentGame.ThirdEnemyHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
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
                    SpecCardSound(3, i);
                }
                else if(CurrentGame.ThirdEnemyHand[i].Specialization == 4 && CurrentGame.ThirdEnemyHand[i].Race < 4)
                {
                    AssassinCard(3);
                    Assassin++;
                }
                else if(CurrentGame.ThirdEnemyHand[i].Race == 4)
                {
                    MegaCard(2);
                    HotCard = 3;
                    LegendaryCardSound(3, i);
                }
                else if(CurrentGame.ThirdEnemyHand[i].Race == 5)
                {
                    MegaCard(4);
                    HotCard = 3;
                    LegendaryCardSound(3, i);
                }
                else if (CurrentGame.ThirdEnemyHand[i].Specialization < 4 && CurrentGame.ThirdEnemyHand[i].Race < 4)
                {
                    HotCard = 3;
                    Sound.AttackSound();
                }

                battle.Race = CurrentGame.ThirdEnemyHand[i].Race;
                battle.BattleImage.sprite = CurrentGame.ThirdEnemyHand[i].Logo;
                battle.ForceCard = CurrentGame.ThirdEnemyHand[i].ForceCard;
                battle.Specialization = CurrentGame.ThirdEnemyHand[i].Specialization;
                count++;
                CurrentGame.ThirdEnemyHand.RemoveAt(i);
                break;
            }
        }
        if(count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[3]++;
            }
            SaveCard = 3;
            PlusText[3].text = $"+{plusCard[3]}";
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
            if (CurrentGame.PlayerHand[i].Race == 4 || CurrentGame.PlayerHand[i].Race == 5 || battle.Race == 4 || battle.Race == 5 ||
                CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard == 1 && battle.ForceCard == 6 ||
                CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard == 6 && battle.ForceCard != 1 &&
                CurrentGame.PlayerHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.PlayerHand[i].Race == battle.Race && CurrentGame.PlayerHand[i].ForceCard != 6 && CurrentGame.PlayerHand[i].ForceCard > battle.ForceCard ||
                CurrentGame.PlayerHand[i].Race == 1 && battle.Race == 3 || CurrentGame.PlayerHand[i].Race == 2 && battle.Race == 1 ||
                CurrentGame.PlayerHand[i].Race == 3 && battle.Race == 2)
            {
                count++;
            }
        }
        if (count == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                GiveCardToHand();
                plusCard[0]++;
                PlusText[0].text = $"+{plusCard[0]}";
                SaveCard = 0;
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
        if(HotCard == 0)
        {
            for (int i = 0; i < fine; i++)
            {
                GiveCardToHand();
                plusCard[0]++;
            }
            PlusText[0].text = $"+{plusCard[0]}";
        }
        else if (HotCard == 1)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[1]++;
            }
            FirstText.text = $"{CurrentGame.FirstEnemyHand.Count}";
            PlusText[1].text = $"+{plusCard[1]}";
        }
        else if(HotCard == 2)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[2]++;
            }
            SecondText.text = $"{CurrentGame.SecondEnemyHand.Count}";
            PlusText[2].text = $"+{plusCard[2]}";
        }
        else if (HotCard == 3)
        {
            for (int i = 0; i < fine; i++)
            {
                CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[3]++;
            }
            ThirdText.text = $"{CurrentGame.ThirdEnemyHand.Count}";
            PlusText[3].text = $"+{plusCard[3]}";
        }
        else if (HotCard == 4)
        {

        }
        HotCard = number;
    }

    private void AssassinCard(int number)//Реализация штрафа Ассасина
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
            HotCard = number;
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
            HotCard = number;
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
            HotCard = number;
        }
        Sound.AssassinSound();
    } 

    private void MegaCard(int war)//Реализация штрафа легендарных карт
    {
        if (HotCard == 0)
        {
            for (int i = 0; i < fine * war; i++)
            {
                GiveCardToHand();
                plusCard[0]++;
            }
            PlusText[0].text = $"+{plusCard[0]}";
        }
        else if (HotCard == 1)
        {
            for (int i = 0; i < fine * war; i++)
            {
                CurrentGame.FirstEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[1]++;
            }
            FirstText.text = $"{CurrentGame.FirstEnemyHand.Count}";
            PlusText[1].text = $"+{plusCard[1]}";
        }
        else if (HotCard == 2)
        {
            for (int i = 0; i < fine * war; i++)
            {
                CurrentGame.SecondEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[2]++;
            }
            SecondText.text = $"{CurrentGame.SecondEnemyHand.Count}";
            PlusText[2].text = $"+{plusCard[2]}";
        }
        else if (HotCard == 3)
        {
            for (int i = 0; i < fine * war; i++)
            {
                CurrentGame.ThirdEnemyHand.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
                plusCard[3]++;
            }
            ThirdText.text = $"{CurrentGame.ThirdEnemyHand.Count}";
            PlusText[3].text = $"+{plusCard[3]}";
        }
        else if (HotCard == 4)
        {

        }
    }

    private void LegendaryCardSound(int numberplayer, int numbercard)//Реализация звуков мега карт
    {
        if(numberplayer == 1)
        {
            if(CurrentGame.FirstEnemyHand[numbercard].Race == 4)
            {
                Sound.NecramagSound();
            }
            else if(CurrentGame.FirstEnemyHand[numbercard].Race == 5)
            {
                Sound.DemiurgeSound();
            }
        }
        else if(numberplayer == 2)
        {
            if (CurrentGame.SecondEnemyHand[numbercard].Race == 4)
            {
                Sound.NecramagSound();
            }
            else if (CurrentGame.SecondEnemyHand[numbercard].Race == 5)
            {
                Sound.DemiurgeSound();
            }
        }
        else if(numberplayer == 3)
        {
            if (CurrentGame.ThirdEnemyHand[numbercard].Race == 4)
            {
                Sound.NecramagSound();
            }
            else if (CurrentGame.ThirdEnemyHand[numbercard].Race == 5)
            {
                Sound.DemiurgeSound();
            }
        }
    }

    private void SpecCardSound(int numberplayer, int numbercard)//Реализация звуков прибития специализации
    {
        if(numberplayer == 1)
        {
            if(CurrentGame.FirstEnemyHand[numbercard].Specialization == 1 && battle.Specialization == 3)
            {
                Sound.ArcherSound();
            }
            else if (CurrentGame.FirstEnemyHand[numbercard].Specialization == 2 && battle.Specialization == 1)
            {
                Sound.MagicianSound();
            }
            else if (CurrentGame.FirstEnemyHand[numbercard].Specialization == 3 && battle.Specialization == 2)
            {
                Sound.WarriorSound();
            }
        }
        else if (numberplayer == 2)
        {
            if (CurrentGame.SecondEnemyHand[numbercard].Specialization == 1 && battle.Specialization == 3)
            {
                Sound.ArcherSound();
            }
            else if (CurrentGame.SecondEnemyHand[numbercard].Specialization == 2 && battle.Specialization == 1)
            {
                Sound.MagicianSound();
            }
            else if (CurrentGame.SecondEnemyHand[numbercard].Specialization == 3 && battle.Specialization == 2)
            {
                Sound.WarriorSound();
            }
        }
        else if (numberplayer == 3)
        {
            if (CurrentGame.ThirdEnemyHand[numbercard].Specialization == 1 && battle.Specialization == 3)
            {
                Sound.ArcherSound();
            }
            else if (CurrentGame.ThirdEnemyHand[numbercard].Specialization == 2 && battle.Specialization == 1)
            {
                Sound.MagicianSound();
            }
            else if (CurrentGame.ThirdEnemyHand[numbercard].Specialization == 3 && battle.Specialization == 2)
            {
                Sound.WarriorSound();
            }
        }
    }
}

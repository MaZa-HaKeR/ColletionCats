using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game
{
    public Player Player, Enemy;
    public List<Card> EnemyDeck, PlayerDeck;

    public Game()
    {
        EnemyDeck = GiveEnemyDeckCard();
        PlayerDeck = GiveDeckCard();

        Player = new Player();
        Enemy = new Player();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 20; i++)
        {
            var card = CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)];

            if (card.IsSpell)
            {
                list.Add(((SpellCard)card).GetCopy());
            }
            else
                list.Add(card.GetCopy());
        }
        return list;
    }

    List<Card> GiveEnemyDeckCard()
    {
        List<Card> list = new List<Card>();
        while (list.Count < 20)
        {
            var card = CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)];

            if (card.IsSpell && ((SpellCard)card).SpellTarget != SpellCard.TargetType.ENEMY_CARD_TARGET && ((SpellCard)card).SpellTarget != SpellCard.TargetType.ALLY_CARD_TARGET)
                list.Add(((SpellCard)card).GetCopy());
            else if (!card.IsSpell)
                list.Add(card.GetCopy());
        }
        return list;
    }
}

public class GameManagerScr : MonoBehaviour
{
    public static GameManagerScr Instance;
    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand, EnemyField, PlayerField;
    
    public GameObject CardPref;
    int Turn, TurnTime = 30;
    public AttackedHero EnemyHero, PlayerHero;
    public AI EnemyAI;

    public List<CardController> PlayerHandCards = new List<CardController>(),
                             PlayerFieldCards = new List<CardController>(),
                             EnemyHandCards = new List<CardController>(),
                             EnemyFieldCards = new List<CardController>();


    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;
        }
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void EndGame()
    {
        StopAllCoroutines();

        foreach (var card in PlayerFieldCards)
            Destroy(card.gameObject);
        foreach (var card in PlayerHandCards)
            Destroy(card.gameObject);
        foreach (var card in EnemyFieldCards)
            Destroy(card.gameObject);
        foreach (var card in EnemyHandCards)
            Destroy(card.gameObject);
        
        PlayerFieldCards.Clear();
        PlayerHandCards.Clear();
        EnemyFieldCards.Clear();
        EnemyHandCards.Clear();

    }

    public void StartGame()
    {
        Turn = 0;
        CurrentGame = new Game();

        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);
        
        UIController.Instance.StartGame();

        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        int i = 0;
        while (i++ < 5)
        {
            GiveCardToHand(deck, hand);
        }
    }

    void GiveCardToHand(List<Card> deck, Transform hand)
    {
        if (deck.Count == 0)
            return;

        Card card = deck[0];

        CardCreatePref(card, hand);

        deck.RemoveAt(0);
    }

    void CardCreatePref(Card card, Transform hand){
        GameObject cardGO = Instantiate(CardPref, hand, false);
        CardController cardC = cardGO.GetComponent<CardController>();

        cardC.Init(card, hand == PlayerHand);

        if(cardC.IsPlayerCard)
            PlayerHandCards.Add(cardC);
        else
            EnemyHandCards.Add(cardC);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        UIController.Instance.UpdateTurnTime(TurnTime);

        foreach (var card in PlayerFieldCards)
            card.Info.HighlightCard(false);

        CheckCardsForManaAvailability();

        if (IsPlayerTurn)
        {
            foreach (var card in PlayerFieldCards)
            {
                card.Card.CanAttack = true;
                card.Info.HighlightCard(true);
                card.Ability.OnNewTurn();
            }

            while(TurnTime-- > 0)
            {
                UIController.Instance.UpdateTurnTime(TurnTime);
                yield return new WaitForSeconds(1);
            }
            ChangeTurn();

        }
        else
        {
            foreach (var card in EnemyFieldCards)
            {
                card.Card.CanAttack = true;
                card.Ability.OnNewTurn();
            }

            EnemyAI.MakeTurn();

            while(TurnTime-- > 0)
            {
                UIController.Instance.UpdateTurnTime(TurnTime);
                yield return new WaitForSeconds(1);
            }

            ChangeTurn();
        }

    }

    public void ChangeTurn()
    {
        StopAllCoroutines();

        Turn++;

        UIController.Instance.DisableTurnBtn();

        if (IsPlayerTurn){
            GiveNewCards();

            CurrentGame.Player.IncreaseManaPool();
            CurrentGame.Player.RestoreRoundMana();

        }
        else
        {
            CurrentGame.Enemy.IncreaseManaPool();
            CurrentGame.Enemy.RestoreRoundMana();
        }
        UIController.Instance.UpdateHPAndMana();
        StartCoroutine(TurnFunc());
    }

    void GiveNewCards()
    {
        if (PlayerHandCards.Count < 7)
            GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);
        if (EnemyHandCards.Count < 7)
            GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);

    }

    public void CardsFight(CardController attacker, CardController defender)
    {
        GameObject.Find("Sounds").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Fight"));
        defender.Card.GetDamage(attacker.Card.Attack);

        attacker.OnDamageDeal();
        defender.OnDamageTake(attacker);

        attacker.Card.GetDamage(defender.Card.Attack);
        attacker.OnDamageTake();

        attacker.CheckForAlive();
        defender.CheckForAlive();
    }

    public void ReduceMana(bool playerMana, int manacost){
        if(playerMana)
            CurrentGame.Player.Mana -= manacost;
        else
            CurrentGame.Enemy.Mana -= manacost;

        UIController.Instance.UpdateHPAndMana();
        
    }

    public void DamageHero(CardController card, bool isEnemyAttacked){
        if(isEnemyAttacked)
            CurrentGame.Enemy.GetDamage(card.Card.Attack);
        else
            CurrentGame.Player.GetDamage(card.Card.Attack);
        
        UIController.Instance.UpdateHPAndMana();
        card.OnDamageDeal();
        CheckForResult();
    }
    
    public void CheckForResult(){
        if(CurrentGame.Enemy.HP == 0 || CurrentGame.Player.HP == 0){
            EndGame();
            UIController.Instance.ShowResult();
        }
    }

    public void CheckCardsForManaAvailability(){
        foreach (var card in PlayerHandCards){
            card.Info.HighlightManaAvaliability(CurrentGame.Player.Mana);
        }
    }

    public void HighlightTargets(CardController attacker, bool highlight)
    {
        List<CardController> targets = new List<CardController>();

        if(attacker.Card.IsSpell)
        {
            var spellCard = (SpellCard)attacker.Card;

            if (spellCard.SpellTarget == SpellCard.TargetType.NO_TARGET)
                targets = new List<CardController>();
            else if (spellCard.SpellTarget == SpellCard.TargetType.ALLY_CARD_TARGET)
                targets = PlayerFieldCards;
            else
                targets = EnemyFieldCards;
        }
        else
        {
            if(EnemyFieldCards.Exists(x => x.Card.IsProvocation))
                targets = EnemyFieldCards.FindAll(x => x.Card.IsProvocation);
            else
            {
                targets = EnemyFieldCards;
                EnemyHero.HighlightAsTarget(highlight);
            }
        }
        foreach (var card in targets)
        {
            if (attacker.Card.IsSpell)
                card.Info.HighlightAsSpellTarget(highlight);
            else
                card.Info.HighlightAsTarget(highlight);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public Card Card;

    public bool IsPlayerCard;

    public CardInfoScr Info;
    public CardMovementScr Movement;
    public CardAbility Ability;

    GameManagerScr gameManager;

    public void Init(Card card, bool isPlayerCard)
    {
        Card = card;
        gameManager = GameManagerScr.Instance;
        IsPlayerCard = isPlayerCard;

        if(IsPlayerCard)
        {
            Info.ShowCardInfo();
            GetComponent<AttackedCard>().enabled = false;
        }
        else
            Info.HideCardInfo();
    }

    public void OnCast()
    {
        if(Card.IsSpell && ((SpellCard)Card).SpellTarget != SpellCard.TargetType.NO_TARGET)
        {   
            UIController.Instance.UpdateHPAndMana();
            return;
        }

        if(IsPlayerCard)
        {
            gameManager.PlayerHandCards.Remove(this);
            gameManager.PlayerFieldCards.Add(this);
            gameManager.ReduceMana(true, Card.Manacost);
            gameManager.CheckCardsForManaAvailability();
        }
        else
        {
            gameManager.EnemyHandCards.Remove(this);
            gameManager.EnemyFieldCards.Add(this);
            gameManager.ReduceMana(false, Card.Manacost);
            Info.ShowCardInfo();
        }
        Card.IsPlaced = true;
        GameObject.Find("Sounds").GetComponent<AudioSource>().PlayOneShot(Card.Sound);

        if (Card.HasAbility)
            Ability.OnCast();

        if(Card.IsSpell)
            UseSpell(null);
    
        UIController.Instance.UpdateHPAndMana();
    }

    public void OnDamageTake(CardController attacker = null)
    {
        CheckForAlive();
        Ability.OnDamageTake(attacker);
    }

    public void OnDamageDeal()
    {
        Card.TimesDealedDamage++;
        Card.CanAttack = false;
        Info.HighlightCard(false);

        if(Card.HasAbility)
            Ability.OnDamageDeal();
    }

    public void UseSpell(CardController target)
    {

        var spellCard = (SpellCard)Card;

        GameObject.Find("Sounds").GetComponent<AudioSource>().PlayOneShot(Card.Sound);
        switch (spellCard.Spell)
        {
            case SpellCard.SpellType.HEAL_ALLY_FIELD_CARDS:

                var allyCards = IsPlayerCard ? gameManager.PlayerFieldCards : gameManager.EnemyFieldCards;

                foreach (var card in allyCards)
                {
                    card.Card.Defense += spellCard.SpellValue;
                    card.Info.RefreshData();
                } 

                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS:

                var enemyCards = IsPlayerCard ? 
                                new List<CardController>(gameManager.EnemyFieldCards) : 
                                new List<CardController>(gameManager.PlayerFieldCards);

                foreach (var card in enemyCards)
                    GiveDamageTo(card, spellCard.SpellValue);

                break;

            case SpellCard.SpellType.HEAL_ALLY_HERO:

                if (IsPlayerCard)
                    gameManager.CurrentGame.Player.HP += spellCard.SpellValue;
                else
                    gameManager.CurrentGame.Enemy.HP += spellCard.SpellValue;

                UIController.Instance.UpdateHPAndMana();

                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_HERO:

                if (IsPlayerCard)
                    gameManager.CurrentGame.Enemy.HP -= spellCard.SpellValue;
                else
                    gameManager.CurrentGame.Player.HP -= spellCard.SpellValue;

                UIController.Instance.UpdateHPAndMana();
                gameManager.CheckForResult();
                break;

            case SpellCard.SpellType.HEAL_ALLY_CARD:
                target.Card.Defense += spellCard.SpellValue;
                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_CARD:
                GiveDamageTo(target, spellCard.SpellValue);
                break;

            case SpellCard.SpellType.SHIELD_ON_ALLY_CARD:
                if(!target.Card.Abilities.Exists(x => x == Card.AbilityType.SHIELD))
                    target.Card.Abilities.Add(Card.AbilityType.SHIELD);
                break;

            case SpellCard.SpellType.PROVOCATION_ON_ALLY_CARD:
                if(!target.Card.Abilities.Exists(x => x == Card.AbilityType.PROVOCATION))
                    target.Card.Abilities.Add(Card.AbilityType.PROVOCATION);
                break;

            case SpellCard.SpellType.BUFF_CARD_DAMAGE:
                if (IsPlayerCard)
                    target.Card.Attack += spellCard.SpellValue;
                break;

            case SpellCard.SpellType.DEBUFF_CARD_DAMAGE:
                target.Card.Attack = Mathf.Clamp(target.Card.Attack - spellCard.SpellValue, 0, int.MaxValue);
                break;
        }

        if (target != null)
        {
            target.Ability.OnCast();
            target.CheckForAlive();
        }

        DestroyCard();
    }

    void GiveDamageTo(CardController card, int damage)
    {
        card.Card.GetDamage(damage);
        card.CheckForAlive();
        card.OnDamageTake();
    }

    public void CheckForAlive()
    {
        if (Card.IsAlive)
            Info.RefreshData();
        else
        {
            GameObject.Find("Sounds").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Death"));
            DestroyCard();
        }
    }


    public void DestroyCard()
    {
        Movement.OnEndDrag(null);
        RemoveCardFromList(gameManager.PlayerHandCards);
        RemoveCardFromList(gameManager.PlayerFieldCards);
        RemoveCardFromList(gameManager.EnemyHandCards);
        RemoveCardFromList(gameManager.EnemyFieldCards);      

        Destroy(gameObject);
    }

    void RemoveCardFromList(List<CardController> list)
    {
        if(list.Exists(x=>x==this))
            list.Remove(this);
    }
}

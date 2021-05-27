using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card
{
    public enum AbilityType
    {
        NO_ABILITY,
        INSTANT_ACTIVE,
        DOUBLE_ATTACK,
        SHIELD,
        PROVOCATION,
        REGENERATION_EACH_TURN,
        COUNTER_ATTACK
    }
    public string Name;
    public Sprite Logo;
    public int Attack, Defense, Manacost;
    public bool CanAttack;
    public bool IsPlaced = false;

    public List<AbilityType> Abilities;


    public AudioClip Sound;

    public bool IsSpell;
    public bool IsAlive
    {
        get
        {
            return Defense > 0;
        }
    }
    public bool HasAbility
    {
        get
        {
            return Abilities.Count > 0;
        }
    }
    public bool IsProvocation
    {
        get
        {
            return Abilities.Exists(x => x == AbilityType.PROVOCATION);
        }
    }
    public int TimesDealedDamage;
    public Card(string name, string logoPath, string soundPath, int attack, int defense, int manacost, AbilityType abilityType = 0)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Sound = Resources.Load<AudioClip>(soundPath);
        Attack = attack;
        Defense = defense;
        Manacost = manacost;
        CanAttack = false;
        IsPlaced = false;
        Abilities = new List<AbilityType>();

        if (abilityType != 0)
            Abilities.Add(abilityType);

        TimesDealedDamage = 0;
    }

    public Card(Card card)
    {
        Name = card.Name;
        Logo = card.Logo;
        Sound = card.Sound;
        Attack = card.Attack;
        Defense = card.Defense;
        Manacost = card.Manacost;
        CanAttack = false;
        IsPlaced = false;

        Abilities = new List<AbilityType>(card.Abilities);

        TimesDealedDamage = 0;
    }
    public void GetDamage(int dmg)
    {
        if(dmg > 0)
        {
            if(Abilities.Exists(x => x == AbilityType.SHIELD))
                Abilities.Remove(AbilityType.SHIELD);
            else
                Defense -= dmg;

        }
    }

    public Card GetCopy()
    {
        return new Card(this);
    }
}

public class SpellCard : Card
{
    public enum SpellType
    {
        NO_SPELL,
        HEAL_ALLY_FIELD_CARDS,
        DAMAGE_ENEMY_FIELD_CARDS,
        HEAL_ALLY_HERO,
        DAMAGE_ENEMY_HERO,
        HEAL_ALLY_CARD,
        DAMAGE_ENEMY_CARD,
        SHIELD_ON_ALLY_CARD,
        PROVOCATION_ON_ALLY_CARD,
        BUFF_CARD_DAMAGE,
        DEBUFF_CARD_DAMAGE
    }
    public enum TargetType
    {
        NO_TARGET,
        ALLY_CARD_TARGET,
        ENEMY_CARD_TARGET
    }
    public TargetType SpellTarget;
    public SpellType Spell;
    public int SpellValue;

    public SpellCard(string name, string logoPath, string soundPath, int manacost, SpellType spellType = 0, 
                    int spellValue = 0, TargetType targetType = 0) : base(name, logoPath, soundPath, 0, 0, manacost)
    {
        IsSpell = true;

        Spell = spellType;
        SpellTarget = targetType;
        SpellValue = spellValue;
    }

    public SpellCard(SpellCard card) : base(card)
    {
        IsSpell = true;
        Spell = card.Spell;
        SpellTarget = card.SpellTarget;
        SpellValue = card.SpellValue;
    }

    public new SpellCard GetCopy()
    {
        return new SpellCard(this);
    }
}

public static class CardManager
{
    public static List<Card> AllCards = new List<Card>();
}

public class CardManagerScr : MonoBehaviour
{

    public void Awake()
    {
        CardManager.AllCards.Add(new Card("Slowpoke", "Sprites/Cards/SFK_Slowpoke", "Sounds/HeroSounds/Slowpoke", 3, 2, 2));
        CardManager.AllCards.Add(new Card("Atom", "Sprites/Cards/SK2_Atom", "Sounds/HeroSounds/Atom", 2, 1, 1));
        CardManager.AllCards.Add(new Card("Babe", "Sprites/Cards/SK2_Babe", "Sounds/HeroSounds/Babe", 1, 2, 1));
        CardManager.AllCards.Add(new Card("Bat | Provocation", "Sprites/Cards/SK2_Bat", "Sounds/HeroSounds/Bat", 2, 5, 2, Card.AbilityType.PROVOCATION));
        CardManager.AllCards.Add(new Card("Beggar | Regeneration", "Sprites/Cards/SK2_Beggar", "Sounds/HeroSounds/Beggar", 1, 10, 3, Card.AbilityType.REGENERATION_EACH_TURN));
        CardManager.AllCards.Add(new Card("BendingUnit", "Sprites/Cards/SK2_BendingUnit", "Sounds/HeroSounds/BendingUnit", 3, 10, 5));
        CardManager.AllCards.Add(new Card("Blondechan", "Sprites/Cards/SK2_Blondechan", "Sounds/HeroSounds/Blondechan", 1, 9, 3));
        CardManager.AllCards.Add(new Card("Captain", "Sprites/Cards/SK2_Captain", "Sounds/HeroSounds/Captain", 3, 4, 3));
        CardManager.AllCards.Add(new Card("ChebuRash", "Sprites/Cards/SK2_ChebuRash", "Sounds/HeroSounds/ChebuRash", 2, 7, 4));
        CardManager.AllCards.Add(new Card("Darthfather", "Sprites/Cards/SK2_Darthfather", "Sounds/HeroSounds/Darthfather", 2, 5, 3));
        CardManager.AllCards.Add(new Card("Depotboy | Provocation", "Sprites/Cards/SK2_Depotboy", "Sounds/HeroSounds/Depotboy", 1, 4, 2, Card.AbilityType.PROVOCATION));
        CardManager.AllCards.Add(new Card("Electrorat", "Sprites/Cards/SK2_Electrorat", "Sounds/HeroSounds/Electrorat", 1, 2, 1));
        CardManager.AllCards.Add(new Card("Flyboy", "Sprites/Cards/SK2_Flyboy", "Sounds/HeroSounds/Flyboy", 1, 1, 1));
        CardManager.AllCards.Add(new Card("Dracula", "Sprites/Cards/SK2_Dracula", "Sounds/HeroSounds/Dracula", 3, 3, 2));
        CardManager.AllCards.Add(new Card("Fox", "Sprites/Cards/SK2_Fox", "Sounds/HeroSounds/Fox", 2, 1, 1));
        CardManager.AllCards.Add(new Card("Frederick", "Sprites/Cards/SK2_Frederick", "Sounds/HeroSounds/Frederick", 8, 4, 5));
        CardManager.AllCards.Add(new Card("Ghostface", "Sprites/Cards/SK2_Ghostface", "Sounds/HeroSounds/Ghostface", 5, 1, 2));
        CardManager.AllCards.Add(new Card("Hedgehog", "Sprites/Cards/SK2_Hedgehog", "Sounds/HeroSounds/Hedgehog", 2, 7, 4));
        CardManager.AllCards.Add(new Card("HGPoter", "Sprites/Cards/SK2_HGPoter", "Sounds/HeroSounds/HGPoter", 1, 6, 2));
        CardManager.AllCards.Add(new Card("Human | Counter attack", "Sprites/Cards/SK2_Human", "Sounds/HeroSounds/Human", 3, 5, 3, Card.AbilityType.COUNTER_ATTACK));
        CardManager.AllCards.Add(new Card("Ipkiss | Double attack", "Sprites/Cards/SK2_Ipkiss", "Sounds/HeroSounds/Ipkiss", 6, 5, 5, Card.AbilityType.DOUBLE_ATTACK));
        CardManager.AllCards.Add(new Card("Kakaroto", "Sprites/Cards/SK2_Kakaroto", "Sounds/HeroSounds/Kakaroto", 12, 5, 7));
        CardManager.AllCards.Add(new Card("Lowenbru | Instant active", "Sprites/Cards/SK2_Lowenbru", "Sounds/HeroSounds/Lowenbru", 10, 2, 5, Card.AbilityType.INSTANT_ACTIVE));
        CardManager.AllCards.Add(new Card("Metalman", "Sprites/Cards/SK2_Metalman", "Sounds/HeroSounds/Metalman", 8, 3, 5));
        CardManager.AllCards.Add(new Card("Mouseman", "Sprites/Cards/SK2_Mouseman", "Sounds/HeroSounds/Mickey", 3, 1, 2));
        CardManager.AllCards.Add(new Card("Neighbor", "Sprites/Cards/SK2_Neighbor", "Sounds/HeroSounds/Neighbor", 2, 2, 1));
        CardManager.AllCards.Add(new Card("NyaNya", "Sprites/Cards/SK2_NyaNya", "Sounds/HeroSounds/NyaNya", 8, 6, 6));
        CardManager.AllCards.Add(new Card("Pickup-master", "Sprites/Cards/SK2_Pickup-master", "Sounds/HeroSounds/PickupMaster", 2, 4, 3));
        CardManager.AllCards.Add(new Card("Pit", "Sprites/Cards/SK2_Pit", "Sounds/HeroSounds/Pit", 6, 8, 6));
        CardManager.AllCards.Add(new Card("Pitman", "Sprites/Cards/SK2_Pitman", "Sounds/HeroSounds/Pitman", 6, 10, 7));
        CardManager.AllCards.Add(new Card("Plant | Provocation", "Sprites/Cards/SK2_Plant", "Sounds/HeroSounds/Plant", 3, 12, 6, Card.AbilityType.PROVOCATION));
        CardManager.AllCards.Add(new Card("Plumber | Provocation", "Sprites/Cards/SK2_Plumber", "Sounds/HeroSounds/Plumber", 2, 9, 5, Card.AbilityType.PROVOCATION));
        CardManager.AllCards.Add(new Card("Redhead | Counter Attack", "Sprites/Cards/SK2_Redhead", "Sounds/HeroSounds/RedHead", 2, 5, 3, Card.AbilityType.COUNTER_ATTACK));
        CardManager.AllCards.Add(new Card("Ren", "Sprites/Cards/SK2_Ren", "Sounds/HeroSounds/Ren", 7, 7, 7));
        CardManager.AllCards.Add(new Card("Sponge", "Sprites/Cards/SK2_Sponge", "Sounds/HeroSounds/Sponge", 10, 10, 10));
        CardManager.AllCards.Add(new Card("Sweettooth", "Sprites/Cards/SK2_Sweettooth", "Sounds/HeroSounds/SweetTooth", 8, 9, 8));
        CardManager.AllCards.Add(new Card("Thief", "Sprites/Cards/SK2_Thief", "Sounds/HeroSounds/Thief", 7, 5, 6));
        CardManager.AllCards.Add(new Card("Wakawaka", "Sprites/Cards/SK2_Wakawaka", "Sounds/HeroSounds/Wakawaka", 19, 2, 9));
        CardManager.AllCards.Add(new Card("William", "Sprites/Cards/SK2_William", "Sounds/HeroSounds/William", 9, 9, 8));


        CardManager.AllCards.Add(new SpellCard("Resurection", "Sprites/Spells/Resurection", "Sounds/SpellsSounds/MassHeal", 2,
            SpellCard.SpellType.HEAL_ALLY_FIELD_CARDS, 2, SpellCard.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new SpellCard("Fire wall", "Sprites/Spells/FireWall", "Sounds/SpellsSounds/MassDamage", 2,
            SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS, 2, SpellCard.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new SpellCard("Heal Bottle", "Sprites/Spells/HealPotion", "Sounds/SpellsSounds/HeroHeal", 2,
            SpellCard.SpellType.HEAL_ALLY_HERO, 2, SpellCard.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new SpellCard("Knife", "Sprites/Spells/Knife", "Sounds/SpellsSounds/HeroDamage", 2,
            SpellCard.SpellType.DAMAGE_ENEMY_HERO, 2, SpellCard.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new SpellCard("Prayer to god", "Sprites/Spells/PrayerToGod", "Sounds/SpellsSounds/CardHeal", 2, 
            SpellCard.SpellType.HEAL_ALLY_CARD, 2, SpellCard.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new SpellCard("Fireball", "Sprites/Spells/Fireball", "Sounds/SpellsSounds/CardDamage", 2,
            SpellCard.SpellType.DAMAGE_ENEMY_CARD, 2, SpellCard.TargetType.ENEMY_CARD_TARGET));
        CardManager.AllCards.Add(new SpellCard("Shield", "Sprites/Spells/Shield", "Sounds/SpellsSounds/Shield", 2,
            SpellCard.SpellType.SHIELD_ON_ALLY_CARD, 2, SpellCard.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new SpellCard("Provocation", "Sprites/Spells/Provocation", "Sounds/SpellsSounds/Provocation", 2,
            SpellCard.SpellType.PROVOCATION_ON_ALLY_CARD, 2, SpellCard.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new SpellCard("Potion of Strength", "Sprites/Spells/StrengthPotion", "Sounds/SpellsSounds/CardBuff", 2,
            SpellCard.SpellType.BUFF_CARD_DAMAGE, 2, SpellCard.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new SpellCard("Weakness Potion", "Sprites/Spells/WeaknessPotion", "Sounds/SpellsSounds/CardDebuff", 2,
            SpellCard.SpellType.DEBUFF_CARD_DAMAGE, 2, SpellCard.TargetType.ENEMY_CARD_TARGET));
    
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FighterState { OK, CONFUSE }

public class Fighter
{
    public FighterBase Base { get; set; }

    public int Level { get; set; }

    public int HP { get; set; }

    public int Defense { get; set; }

    public List<Movement> Movements { get; set; }

    public FighterState fighterState { get; set; }

    public Fighter(FighterBase fighterBase)
    {
        this.Base = fighterBase;
        this.Level = fighterBase.Level;
        this.HP = MaxHp;
        this.Defense = MaxDefense;

        Movements = new List<Movement>();
        fighterBase.Movements.ForEach(m => Movements.Add(new Movement(m.MovementScriptable)));
    }

    public int Attack { get { return Mathf.FloorToInt(Base.Attack * Level / 100f) + 5; } }

    public int MaxDefense { get { return Mathf.FloorToInt(Base.Defense * Level / 100f) + 5; } }

    public int MaxHp { get { return Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10; } }

    public int Speed { get { return Mathf.FloorToInt(Base.Speed * Level / 100f) + 5; } }

    public int SpAttack { get { return Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5; } }

    public int SpDefense { get { return Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5; } }

    public void TakeDamage(Movement movement, Fighter enemy)
    {
        float mod = Random.Range(0.85f, 1f);
        float a = (2 * enemy.Level + 10) / 250f;
        float d = a * movement.Base.Power * ((float)enemy.Attack / MaxDefense) + 2;
        int damage = Mathf.FloorToInt(d * mod);

        HP -= damage;
    }

    public void Health(Movement movement)
    {
        float mod = Random.Range(0.85f, 1f);
        float healingFactor = (2 * Level + 10) / 250f;
        int healingAmount = Mathf.FloorToInt(healingFactor * movement.Base.Power * mod);

        this.HP = Mathf.Min(HP + healingAmount, MaxHp);
    }

    public void BoostDefense(Movement movement)
    {
        float mod = Random.Range(0.85f, 1f);
        float boostFactor = (2 * Level + 10) / 250f;
        int defenseBoost = Mathf.FloorToInt(boostFactor * movement.Base.Power * mod);
        
        this.Defense += defenseBoost;
    }

    public void DecreaseDefense(Movement movement)
    {
        float mod = Random.Range(0.85f, 1f);
        float boostFactor = (2 * Level + 10) / 250f;
        int defenseBoost = Mathf.FloorToInt(boostFactor * movement.Base.Power * mod);

        this.Defense -= defenseBoost;
    }

    public Movement GetRandomMovement() => Movements[Random.Range(0, Movements.Count)];

}

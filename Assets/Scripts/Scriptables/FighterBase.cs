using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Characters", menuName = "Characters/Create new character")]
public class FighterBase : ScriptableObject
{
    [SerializeField] private string characterName, description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int maxHp, attack, defense, speed, spAttack, spDefense, level;
    [SerializeField] List<CharacterMovements> movements;

    public string Name { get { return characterName; } }

    public string Description { get { return description; } }

    public Sprite Sprite { get { return sprite; } }

    public int MaxHp { get { return maxHp; } }

    public int Attack { get { return attack; } }

    public int Defense { get { return defense; } }

    public int Speed { get { return speed; } }

    public int SpAttack { get { return spAttack; } }

    public int SpDefense { get { return spDefense; } }

    public int Level { get { return level; } }

    public List<CharacterMovements> Movements { get {  return movements; } }
}

[System.Serializable]
public class CharacterMovements
{
    [SerializeField] MovementsBase movementScriptable;
    [SerializeField] int level;

    public MovementsBase MovementScriptable { get { return movementScriptable; } }

    public int Level { get { return level; } }
}

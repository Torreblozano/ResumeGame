using UnityEngine;
using static UnityEngine.UI.ScrollRect;

[CreateAssetMenu(fileName = "Movements", menuName = "Characters/Create new movement")]

public class MovementsBase : ScriptableObject
{
    [SerializeField] private string movementName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private int power, accuracy, pp;

    [SerializeField] private MovementType movementType;

    public string MovementName { get { return movementName; } }

    public string Description { get { return description; } }

    public int Power { get { return power; } set { power = value; } }

    public int Accuracy { get { return accuracy; } }

    public int PP { get { return pp; } }

    public MovementType Type => movementType;

}

[System.Serializable] 
public enum MovementType
{
    NORMAL,
    PSY,
    DEF,
    HP
}

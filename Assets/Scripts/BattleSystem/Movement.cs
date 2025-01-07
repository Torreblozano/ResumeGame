public class Movement{
    public MovementsBase Base { get; set; }
    public int PP { get; set; }
    public Movement(MovementsBase movement)
    {
        Base = movement;
        PP = movement.PP;
    }
}

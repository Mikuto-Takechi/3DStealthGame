namespace MonstersDomain
{
    public interface IPickable
    {
        bool IsPickedUp { get; set; }
        void PickUp();
    }
}

using Safari.Player;

public class FirstAid : Item
{
    public override void Use(PlayerController playerController)
    {
        playerController.CurrentHealth++;
        Destroy(gameObject);
    }
}

using Safari.Player;

namespace Safari.Items
{
    public class FirstAid : Item
    {
        public override void Use(PlayerController playerController)
        {
            playerController.CurrentHealth++;
            TextBoxController.instance.AddNewMessage(new Message("Recovered Health by 1"));
            Destroy(gameObject);
        }
    }
}
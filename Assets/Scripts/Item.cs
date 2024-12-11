using Safari.Player;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
        if (collision.TryGetComponent<PlayerController>(out var pc))
        {
            Use(pc);
        }
    }

    public abstract void Use(PlayerController playerController);
}

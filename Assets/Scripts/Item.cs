using Safari.Animals;
using Safari.Player;
using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var position = Vector2Int.FloorToInt(this.transform.position);
        if (EntityController.positionMap.TryGetValue(position, out var entity) && entity is PlayerController pc)
        {
            Use(pc);
        }
    }

    public abstract void Use(PlayerController playerController);
}

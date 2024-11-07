using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public LayerMask collisionLayer;

    public PlayerController controller;

    public Vector3 DetermineMoveLocation()
    {
        Vector3 finalMoveLocation = controller.movePoint.position;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            finalMoveLocation += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        }
        else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
        {
            finalMoveLocation += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
        }

        return finalMoveLocation;
    }

    public void HandlePlayerMove(Vector3 finalMoveLocation)
    {
        if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
        {
            foreach (EnemyController enemy in controller.enemyManager.enemies)
            {
                if (Vector2.Distance(finalMoveLocation, enemy.enemyTransform.position) <= 0.5f)
                {
                    // take damage and don't move
                    playerHealth.currentHealth--;
                    controller.textBox.AddNewMessage($"You walked into the {enemy.name} and it attacked you!"); // The player taking damage is technically the enemy's action, so the enemy doesn't get to move again.
                    controller.waitForPlayerToReleaseDirection = true; // the player should only take damage once
                    break;
                }
            }

            if (!controller.waitForPlayerToReleaseDirection)
            {
                controller.movePoint.position = finalMoveLocation;
                controller.gameManager.state = GameState.ENEMYTURN;
            }
        }
    }
}

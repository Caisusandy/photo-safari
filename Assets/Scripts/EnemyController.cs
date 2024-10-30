using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    public Transform playerMovePoint;
    public PlayerHealth playerHealth;

    public LayerMask collisionLayer;

    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        // calculate and start next move
        if (Vector3.Distance(transform.position, movePoint.position) == 0 && gameManager.state is GameState.ENEMYTURN)
        {
            // player made their move, so the enemy moves now
            int moveBy = Random.Range(-1, 2);
            while (moveBy == 0)
            {
                moveBy = Random.Range(-1, 2);
            }

            Vector3 finalMoveLocation = movePoint.position;
            if (Random.Range(0, 2) == 0)
            {
                finalMoveLocation += new Vector3(moveBy, 0f, 0f);
            }
            else
            {
                finalMoveLocation += new Vector3(0f, moveBy, 0f);
            }

            // check for walls
            if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            {
                // check for player pos
                if (Vector3.Distance(playerMovePoint.position, finalMoveLocation) == 0)
                {
                    // deal damage to player and don't move
                    playerHealth.currentHealth--;
                    Debug.Log("Player moved where enemy was heading. Current Health: " + playerHealth.currentHealth);
                }
                else
                {
                    movePoint.position = finalMoveLocation;
                }
            }

            // for now, just turn the state back to the player's turn. This will need to change for multiple enemies
            gameManager.state = GameState.PLAYERTURN;
        }
    }
}

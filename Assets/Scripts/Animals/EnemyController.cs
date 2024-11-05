using UnityEngine;

public enum EnemyTrait { RANDOM, PATROL }

public class EnemyController : MonoBehaviour
{
    [Tooltip("The point that the enemy moves towards")]
    public Transform movePoint;
    public float moveSpeed = 5f;
    public Transform playerMovePoint;
    public PlayerHealth playerHealth;
    public LayerMask collisionLayer;
    public GameManager gameManager;

    internal bool finishedTurn = false;
    internal Transform enemyTransform; // this field is so that the transform can be shared with the player

    [SerializeField]
    EnemyTrait enemyTrait;

    [Tooltip("If the animal is set to patrol, how many tiles it will move before turning 90 degrees")]
    [SerializeField]
    int patrolLength = 1;

    private int patrolCount = 0;
    private Vector2 currentDir = new Vector2(1f, 0f);



    void Start()
    {
        movePoint.parent = null;
        enemyTransform = transform;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, movePoint.position) <= 0.5f && gameManager.state is GameState.ENEMYTURN)
        {
            switch (enemyTrait)
            {
                case EnemyTrait.RANDOM:
                    MoveRandom();
                    break;
                case EnemyTrait.PATROL:
                    MovePatrol();
                    break;
                default:
                    break;
            }
        }
    }

    private void MovePatrol()
    {
        Vector2 finalMoveLocation = movePoint.position;
        if (patrolCount >= patrolLength)
        {
            currentDir = new Vector2(-currentDir.y, currentDir.x);
            patrolCount = 0;
        }

        finalMoveLocation += currentDir;
        patrolCount++;
        HandleEnemyMove(finalMoveLocation);
    }


    private void MoveRandom()
    {
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

        HandleEnemyMove(finalMoveLocation);
    }

    private void HandleEnemyMove(Vector2 finalMoveLocation)
    {
        // check for walls
        if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
        {
            // check for player pos
            if (Vector2.Distance(playerMovePoint.position, finalMoveLocation) == 0)
            {
                // deal damage to player and don't move
                playerHealth.currentHealth--;
                Debug.Log("Player moved where enemy was heading. Current Health: " + playerHealth.currentHealth);
                patrolCount--;
            }
            else
            {
                movePoint.position = finalMoveLocation;
            }
        }

        finishedTurn = true;
    }
}

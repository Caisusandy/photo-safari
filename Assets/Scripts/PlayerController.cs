using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public PlayerHealth playerHealth;
    public Transform enemyTranform;
    public LayerMask collisionLayer;
    public GameManager gameManager;
    public CameraFlash cameraFlash;

    private bool waitForPlayerToReleaseDirection = false;

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        bool noDirectionInput = Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0;
        if (waitForPlayerToReleaseDirection && noDirectionInput)
        {
            waitForPlayerToReleaseDirection = false;
        }

        bool acceptingPlayerInput = Vector3.Distance(transform.position, movePoint.position) <= .05f && gameManager.state is GameState.PLAYERTURN && !waitForPlayerToReleaseDirection;
        if (acceptingPlayerInput)
        {
            Vector3 finalMoveLocation = DetermineMoveLocation();
            if (Vector3.Distance(finalMoveLocation, movePoint.position) != 0)
            {
                HandlePlayerMove(finalMoveLocation);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                cameraFlash.flashEnabled = true;
                gameManager.state = GameState.ENEMYTURN;
            }
        }
    }

    #region Player Move

    private Vector3 DetermineMoveLocation()
    {
        Vector3 finalMoveLocation = movePoint.position;
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

    private void HandlePlayerMove(Vector3 finalMoveLocation)
    {
        if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
        {
            // check for enemy pos
            if (Vector3.Distance(finalMoveLocation, enemyTranform.position) == 0)
            {
                // take damage and don't move
                playerHealth.currentHealth--;
                Debug.Log("Player moved onto enemy. Current Health: " + playerHealth.currentHealth);
                waitForPlayerToReleaseDirection = true; // The player taking damage is technically the enemy's action, so the enemy doesn't get to move again.
            }
            else
            {
                movePoint.position = finalMoveLocation;
                gameManager.state = GameState.ENEMYTURN;
            }
        }
    }

    #endregion
}

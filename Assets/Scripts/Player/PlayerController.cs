using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // player-specific fields
    public float moveSpeed = 5f;
    public Transform movePoint;

    // enemy info
    public EnemyManager enemyManager;
    internal List<string> enemiesWithPictures = new List<string>();

    // scripts
    public PlayerMovement movementScript;
    public PlayerCamera cameraScript;

    // other fields
    public GameManager gameManager;
    public Transform stairs;
    public bool waitForPlayerToReleaseDirection = false;
    public TextBoxController textBox;

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, stairs.position) <= 0.5f)
        {
            HandlePlayerOnStairs();
        }

        bool noDirectionInput = Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0;
        if (waitForPlayerToReleaseDirection && noDirectionInput)
        {
            waitForPlayerToReleaseDirection = false;
        }

        bool acceptingPlayerInput = Vector2.Distance(transform.position, movePoint.position) <= .05f && gameManager.state is GameState.PLAYERTURN && !waitForPlayerToReleaseDirection;
        if (acceptingPlayerInput)
        {
            Vector3 finalMoveLocation = movementScript.DetermineMoveLocation();
            if (Vector2.Distance(finalMoveLocation, movePoint.position) != 0)
            {
                movementScript.HandlePlayerMove(finalMoveLocation);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                cameraScript.TakePicture();
            }
        }
    }

    private void HandlePlayerOnStairs()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int numEnemiesWithPictures = enemiesWithPictures.Count;
        if (numEnemiesWithPictures >= enemyCount)
        {
            gameManager.state = GameState.WON;
        }
        else
        {
            string winConMessage = $"You still need to take a picture of {enemyCount - numEnemiesWithPictures} animal";
            if (enemyCount - numEnemiesWithPictures > 1)
            {
                winConMessage += "s";
            }

            SetUpMessage(winConMessage);
        }
    }

    public void SetUpMessage(string messageToDisplay)
    {
        textBox.messageToDisplay = messageToDisplay;

        if (textBox.isDisplayingMessage)
        {
            textBox.resetMessage = true;
        }
        else
        {
            textBox.isDisplayingMessage = true;
        }
    }

}

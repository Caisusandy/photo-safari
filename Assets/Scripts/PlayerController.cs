using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // player-specific fields
    public float moveSpeed = 5f;
    public Transform movePoint;
    public PlayerHealth playerHealth;

    // camera fields
    public CameraFlash cameraFlash;
    public TextBoxController textBox;

    // other fields
    public List<Transform> enemies;
    public LayerMask collisionLayer;
    public GameManager gameManager;
    public Transform stairs;

    private bool waitForPlayerToReleaseDirection = false;
    private List<string> enemiesWithPictures = new List<string>();

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, stairs.position) == 0)
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
            Vector3 finalMoveLocation = DetermineMoveLocation();
            if (Vector2.Distance(finalMoveLocation, movePoint.position) != 0)
            {
                HandlePlayerMove(finalMoveLocation);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                TakePicture();
            }
        }
    }

    private void HandlePlayerOnStairs()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int numEnemiesWithPictures = enemiesWithPictures.Count;
        if (numEnemiesWithPictures == enemyCount)
        {
            gameManager.state = GameState.WON;
        }
        else
        {
            string winConMessage = $"You still need to take a picture of {enemyCount - numEnemiesWithPictures} ";
            if (enemyCount - numEnemiesWithPictures > 1)
            {
                winConMessage += "enemies";
            }
            else
            {
                winConMessage += "enemy";
            }

            SetUpMessage(winConMessage);
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
            foreach (Transform enemy in enemies)
            {
                if (Vector2.Distance(finalMoveLocation, enemy.position) == 0)
                {
                    // take damage and don't move
                    playerHealth.currentHealth--;
                    Debug.Log("Player moved onto enemy. Current Health: " + playerHealth.currentHealth);
                    waitForPlayerToReleaseDirection = true; // The player taking damage is technically the enemy's action, so the enemy doesn't get to move again.
                    break;
                }
            }

            if (!waitForPlayerToReleaseDirection)
            {
                movePoint.position = finalMoveLocation;
                gameManager.state = GameState.ENEMYTURN;
            }
        }
    }

    #endregion

    #region Take Picture
    private void TakePicture()
    {
        cameraFlash.flashEnabled = true;

        string photoSubject = DetectPhotoSubject();

        if (!enemiesWithPictures.Contains(photoSubject))
        {
            enemiesWithPictures.Add(photoSubject);
        }

        string message = GetPictureMessage(photoSubject);
        SetUpMessage(message);
        gameManager.state = GameState.ENEMYTURN;
    }

    private string GetPictureMessage(string photoSubject)
    {
        string messageToDisplay;
        if (string.IsNullOrEmpty(photoSubject))
        {
            messageToDisplay = "Didn't get a picture of anything";
        }
        else
        {
            messageToDisplay = "Took a picture of a " + photoSubject;
        }

        return messageToDisplay;
    }

    private string DetectPhotoSubject()
    {
        List<Vector3> adjacentSpacesToPlayer = new List<Vector3>()
        {
            new Vector3(transform.position.x + 1, transform.position.y, 0),
            new Vector3(transform.position.x - 1, transform.position.y, 0),
            new Vector3(transform.position.x, transform.position.y + 1, 0),
            new Vector3(transform.position.x, transform.position.y - 1, 0),
        };

        string photoSubject = null;
        bool enemyFound = false;
        foreach (Vector3 space in adjacentSpacesToPlayer)
        {
            foreach (Transform enemy in enemies)
            {
                if (Vector2.Distance(space, enemy.position) == 0)
                {
                    photoSubject = enemy.name;
                    enemyFound = true;
                    break;
                }
            }

            if (enemyFound)
            {
                break;
            }
        }

        return photoSubject;
    }

    #endregion

    private void SetUpMessage(string messageToDisplay)
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

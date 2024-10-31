using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveBy = 1f;
    public Transform movePoint;
    public PlayerHealth playerHealth;
    public Transform enemy; // when more enemies are added this will need to be changed into a list
    public LayerMask collisionLayer;
    public GameManager gameManager;
    public CameraFlash cameraFlash;
    public TextBoxController textBox;

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
                TakePicture();
            }
        }
    }

    #region Player Move

    private Vector3 DetermineMoveLocation()
    {
        Vector3 finalMoveLocation = movePoint.position;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            finalMoveLocation += new Vector3(Input.GetAxisRaw("Horizontal") * moveBy, 0f, 0f);
        }
        else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
        {
            finalMoveLocation += new Vector3(0f, Input.GetAxisRaw("Vertical") * moveBy, 0f);
        }

        return finalMoveLocation;
    }

    private void HandlePlayerMove(Vector3 finalMoveLocation)
    {
        if (!Physics2D.OverlapCircle(finalMoveLocation, .2f * moveBy, collisionLayer))
        {
            // check for enemy pos
            if (Vector3.Distance(finalMoveLocation, enemy.position) == 0)
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

    #region Take Picture
    private void TakePicture()
    {
        cameraFlash.flashEnabled = true;
        SetPictureMessage();

        if (textBox.isDisplayingMessage)
        {
            textBox.resetMessage = true;
        }
        else
        {
            textBox.isDisplayingMessage = true;
        }

        gameManager.state = GameState.ENEMYTURN;
    }

    private void SetPictureMessage()
    {
        string photoSubject = DetectPhotoSubject();
        string messageToDisplay = "Took a picture of ";
        if (photoSubject is null)
        {
            messageToDisplay = "Didn't get a picture of anything";
        }
        else
        {
            messageToDisplay += "a " + photoSubject;
        }

        textBox.messageToDisplay = messageToDisplay;
    }

    private string DetectPhotoSubject()
    {
        List<Vector3> adjacentSpacesToPlayer = new List<Vector3>()
        {
            new Vector3(transform.position.x + moveBy, transform.position.y, 0),
            new Vector3(transform.position.x - moveBy, transform.position.y, 0),
            new Vector3(transform.position.x, transform.position.y + moveBy, 0),
            new Vector3(transform.position.x, transform.position.y - moveBy, 0),
        };

        string photoSubject = null;
        foreach (Vector3 space in adjacentSpacesToPlayer)
        {
            if (Vector3.Distance(space, enemy.position) == 0)
            {
                photoSubject = enemy.name;
                break;
            }
        }

        return photoSubject;
    }

    #endregion
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public CameraFlash cameraFlash;
    public PlayerController controller;

    public void TakePicture()
    {
        cameraFlash.flashEnabled = true;

        string photoSubject = DetectPhotoSubject();

        if (!controller.enemiesWithPictures.Contains(photoSubject) && !string.IsNullOrEmpty(photoSubject))
        {
            controller.enemiesWithPictures.Add(photoSubject);
        }

        string message = GetPictureMessage(photoSubject);
        controller.SetUpMessage(message);
        controller.gameManager.state = GameState.ENEMYTURN;
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
        List<Vector2> adjacentSpacesToPlayer = new List<Vector2>()
        {
            new Vector2(transform.position.x + 1, transform.position.y),
            new Vector2(transform.position.x - 1, transform.position.y),
            new Vector2(transform.position.x, transform.position.y + 1),
            new Vector2(transform.position.x, transform.position.y - 1),
        };

        string photoSubject = null;
        bool enemyFound = false;
        foreach (Vector2 space in adjacentSpacesToPlayer)
        {
            foreach (Transform enemy in controller.enemies)
            {
                if (Vector2.Distance(space, enemy.position) <= 0.5f)
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
}

using Safari;
using Safari.Animals;
using Safari.Player;
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
        TextBoxController.instance.AddNewMessage(new Message(message));
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
        foreach (Vector2 space in adjacentSpacesToPlayer)
        {
            var index = Vector2Int.FloorToInt(space);
            if (EntityController.positionMap.TryGetValue(index, out var enemy))
            {
                photoSubject = enemy.name;
                break;
            }
        }

        return photoSubject;
    }
}

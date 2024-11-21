using Safari;
using Safari.Animals;
using Safari.Player;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public PlayerController controller;

    public void TakePicture()
    {
        GameManager.instance.cameraFlash.flashEnabled = true;

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
        string photoSubject = null;
        Vector2 spaceInFrontOfPlayer = controller.currentDirection + (Vector2)transform.position;
        var index = Vector2Int.FloorToInt(spaceInFrontOfPlayer);
        if (EntityController.positionMap.TryGetValue(index, out var enemy))
        {
            photoSubject = enemy.name.Replace("(Clone)", "");
        }

        return photoSubject;
    }
}

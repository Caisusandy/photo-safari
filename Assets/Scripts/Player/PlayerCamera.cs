using Safari;
using Safari.Animals;
using Safari.Player;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public PlayerController controller;

    public void TakePicture()
    {
        GameManager.instance.cameraFlash.flashEnabled = true;

        EnemyController animal = DetectPhotoSubject();

        if (!GameManager.instance.enemiesWithPictures.Contains(animal) && animal != null)
        {
            GameManager.instance.enemiesWithPictures.Add(animal);
            UpdateStatusCounter(animal.name);
        }

        string photoSubject = animal.name.Remove(animal.name.IndexOf("("));
        string message = GetPictureMessage(photoSubject);
        TextBoxController.instance.AddNewMessage(new Message(message));
    }

    private void UpdateStatusCounter(string photoSubject)
    {
        if (photoSubject.Contains("frog", System.StringComparison.CurrentCultureIgnoreCase))
        {
            GameManager.instance.numFrogsRequired--;
        }
        else if (photoSubject.Contains("capybara", System.StringComparison.CurrentCultureIgnoreCase))
        {
            GameManager.instance.numCapybarasRequired--;
        }
        else if (photoSubject.Contains("jaguar", System.StringComparison.CurrentCultureIgnoreCase))
        {
            GameManager.instance.numJaguarsRequired--;
        }
        else if (photoSubject.Contains("butterfly", System.StringComparison.CurrentCultureIgnoreCase))
        {
            GameManager.instance.numButterfliesRequired--;
        }
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

    private EnemyController DetectPhotoSubject()
    {
        Vector2 spaceInFrontOfPlayer = controller.currentDirection.ToVector2() + (Vector2)transform.position;
        var index = Vector2Int.FloorToInt(spaceInFrontOfPlayer);
        if (EntityController.positionMap.TryGetValue(index, out var enemy))
        {
            return (EnemyController)enemy;
        }

        return null;
    }
}

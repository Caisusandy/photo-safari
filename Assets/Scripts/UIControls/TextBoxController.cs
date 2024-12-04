using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public struct Message
{
    public Message(float totalTimeToDisplay, string message)
    {
        this.totalTimeToDisplay = totalTimeToDisplay;
        messageToDisplay = message;
        startTime = 0f;
    }

    public Message(string message)
    {
        totalTimeToDisplay = 2.5f;
        messageToDisplay = message;
        startTime = 0f;
    }

    public override bool Equals(object obj)
    {
        if (obj is Message)
        {
            return ((Message)obj).messageToDisplay == messageToDisplay;
        }

        if (obj is string)
        {
            return (string)obj == messageToDisplay;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return messageToDisplay.GetHashCode();
    }

    public float totalTimeToDisplay;
    public string messageToDisplay;
    public float startTime;
}

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance;

    public TextMeshProUGUI textMesh;
    public GameObject textBackground;

    internal string textBoxMessage = null;

    private Queue<Message> messagesDisplaying = new Queue<Message>();
    private Queue<Message> messageBacklog = new Queue<Message>();
    private bool waitForSpace = false;
    private float elapsedTime = 0f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        textMesh.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (messagesDisplaying.Count > 0)
        {
            elapsedTime += Time.deltaTime;

            List<Message> messagesDisplayingList = messagesDisplaying.ToList();
            for (int i = 0; i < messagesDisplayingList.Count; i++)
            {
                ActivateTextBox();
                Message message = messagesDisplayingList[i];
                if (elapsedTime - message.startTime >= message.totalTimeToDisplay)
                {
                    messagesDisplaying.Dequeue();
                    waitForSpace = false;

                    if (messageBacklog.Count > 0)
                    {
                        Message backlogMessage = messageBacklog.Dequeue();
                        backlogMessage.startTime = elapsedTime;
                        messagesDisplaying.Enqueue(backlogMessage);
                    }

                    messagesDisplayingList = messagesDisplaying.ToList();
                    textBoxMessage = string.Empty;
                    for (int j = 0; j < messagesDisplayingList.Count; j++)
                    {
                        if (j > 0)
                        {
                            textBoxMessage += "\n";
                        }

                        textBoxMessage += messagesDisplayingList[i].messageToDisplay;
                    }
                }
            }
        }
        else
        {
            DeactivateTextBox();
        }
    }

    private void ActivateTextBox()
    {
        textMesh.text = textBoxMessage;
        textMesh.gameObject.SetActive(true);
        textBackground.gameObject.SetActive(true);
    }

    private void DeactivateTextBox()
    {
        textMesh.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);
        textBoxMessage = string.Empty;
        textMesh.pageToDisplay = 1;
    }

    public void AddNewMessage(Message newMessage)
    {
        if (!messagesDisplaying.Contains(newMessage))
        {
            if (!waitForSpace)
            {
                newMessage.startTime = elapsedTime;
                if (!string.IsNullOrEmpty(textBoxMessage))
                {
                    textBoxMessage += "\n";
                }

                textBoxMessage += newMessage.messageToDisplay;
                messagesDisplaying.Enqueue(newMessage);

                if (textMesh.isTextOverflowing)
                {
                    waitForSpace = true;
                }
            }
            else
            {
                messageBacklog.Enqueue(newMessage);
            }
        }
    }
}

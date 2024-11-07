using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class TextBoxController : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public GameObject textBackground;

    internal string textBoxMessage = null;

    private Queue<string> messagesDisplaying = new Queue<string>();
    private Queue<string> messageBacklog = new Queue<string>();
    private bool waitForSpace = false;
    private float elapsedTime = 0f;

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
            ActivateTextBox();
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 2.5f)
            {
                elapsedTime = 0f;
                messagesDisplaying.Dequeue();
                waitForSpace = false;

                if (messageBacklog.Count > 0)
                {
                    messagesDisplaying.Enqueue(messageBacklog.Dequeue());
                }

                List<string> messagesDisplayingList = messagesDisplaying.ToList();
                textBoxMessage = string.Empty;
                for (int i = 0; i < messagesDisplayingList.Count; i++)
                {
                    if (i > 0 )
                    {
                        textBoxMessage += "\n";
                    }

                    textBoxMessage += messagesDisplayingList[i];
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

    public void AddNewMessage(string newMessage)
    {
        if (!waitForSpace)
        {
            if (!string.IsNullOrEmpty(textBoxMessage))
            {
                textBoxMessage += "\n";
            }

            textBoxMessage += newMessage;
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

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextBoxController : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Image textBackground;
    public float timeToDisplay = 3f;
    public bool isDisplayingMessage = false;
    public string messageToDisplay = null;
    public bool resetMessage = false;

    private float elapsedTime = 0f;

    void Start()
    {
        messageText.gameObject.SetActive(false);
        textBackground.color = new Color(textBackground.color.r, textBackground.color.g, textBackground.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayingMessage)
        {
            if (elapsedTime == 0f || resetMessage)
            {
                elapsedTime = 0f;
                InitializeTextBox();
                resetMessage = false;
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTime > timeToDisplay)
            {
                messageToDisplay = null;
                messageText.gameObject.SetActive(false);
                textBackground.color = new Color(textBackground.color.r, textBackground.color.g, textBackground.color.b, 0);
                isDisplayingMessage = false;
                elapsedTime = 0f;
            }
        }
    }

    private void InitializeTextBox()
    {
        if (!string.IsNullOrEmpty(messageText.text))
        {
            messageText.text = messageToDisplay;
        }
        else
        {
            messageText.text = "Default Message";
        }

        messageText.gameObject.SetActive(true);
        textBackground.color = new Color(textBackground.color.r, textBackground.color.g, textBackground.color.b, 100);
    }
}

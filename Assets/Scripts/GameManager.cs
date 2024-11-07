using UnityEngine;

public enum GameState { PLAYERTURN, ENEMYTURN, WON, GAMEOVER}

public class GameManager : MonoBehaviour
{
    public GameState state;
    public GameObject gameOverText;
    public GameObject winText;
    public TextBoxController textBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.PLAYERTURN;
        gameOverText.SetActive(false);
        winText.SetActive(false);
        textBox.AddNewMessage("Use the arrow keys or WASD to move. Press SPACE to take a picture of the animals. Once you've finished exploring use the stairs to advance.");
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.WON:
                winText.SetActive(true);
                break;
            case GameState.GAMEOVER:
                gameOverText.SetActive(true);
                break;
            default:
                break;
        }
    }
}

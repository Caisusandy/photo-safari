using UnityEngine;

public enum GameState { PLAYERTURN, ENEMYTURN, WON, GAMEOVER}

public class GameManager : MonoBehaviour
{
    public GameState state;
    public GameObject gameOverText;
    public GameObject winText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.PLAYERTURN;
        gameOverText.SetActive(false);
        winText.SetActive(false);
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

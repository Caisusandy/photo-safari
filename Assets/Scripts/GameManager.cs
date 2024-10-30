using UnityEngine;

public enum GameState { PLAYERTURN, ENEMYTURN, WON, GAMEOVER}

public class GameManager : MonoBehaviour
{
    public GameState state;
    public GameObject gameOverText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.PLAYERTURN;
        gameOverText.SetActive(false);
    }

    private void Update()
    {
        if (state is GameState.GAMEOVER)
        {
            gameOverText.SetActive(true);
        }
    }
}

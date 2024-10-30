using UnityEngine;

public enum GameState { PLAYERTURN, ENEMYTURN, WON, LOST}

public class GameManager : MonoBehaviour
{
    public GameState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.PLAYERTURN;
    }

    private void Update()
    {
    }
}

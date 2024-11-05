using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyController> enemies;
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool enemiesFinishedTurn = true;
        foreach (EnemyController enemy in enemies)
        {
            enemiesFinishedTurn = enemiesFinishedTurn && enemy.finishedTurn;
        }

        if (enemiesFinishedTurn && gameManager.state is GameState.ENEMYTURN)
        {
            gameManager.state = GameState.PLAYERTURN;
        }
    }
}

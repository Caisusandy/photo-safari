using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyController> enemies;
    public GameManager gameManager;

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

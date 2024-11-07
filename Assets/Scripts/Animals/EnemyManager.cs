using Safari.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Safari.Animals
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager instance;

        public List<EnemyController> enemies;
        public GameManager gameManager;

        private void Awake()
        {
            instance = this;
        }

        void Update()
        {
            bool enemiesFinishedTurn = enemies.All(e => e == null || e.finishedTurn);
            if (enemiesFinishedTurn && gameManager.State is GameState.ENEMYTURN)
            {
                gameManager.State = GameState.PLAYERTURN;
            }
        }

        public EnemyController CouldHitEnemy(Vector3 finalMoveLocation)
        {
            foreach (EnemyController enemy in enemies)
            {
                if (Vector2.Distance(finalMoveLocation, enemy.transform.position) <= 0.5f)
                {
                    return enemy;
                }
            }
            return null;
        }

    }
}
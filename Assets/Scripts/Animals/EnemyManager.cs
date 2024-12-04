using Safari.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Safari.Animals
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager instance;
        public GameManager gameManager;
        public List<EnemyController> enemies;
        internal int butterflyTotal = 0;
        internal int capybaraTotal = 0;
        internal int jaguarTotal = 0;
        internal int frogTotal = 0;

        private void Awake()
        {
            instance = this;
        }

        internal void DetermineAnimalTotals()
        {
            butterflyTotal = enemies.Count(animal => animal.name.Contains("butterfly", System.StringComparison.CurrentCultureIgnoreCase));
            capybaraTotal = enemies.Count(animal => animal.name.Contains("capybara", System.StringComparison.CurrentCultureIgnoreCase));
            frogTotal = enemies.Count(animal => animal.name.Contains("frog", System.StringComparison.CurrentCultureIgnoreCase));
            jaguarTotal = enemies.Count(animal => animal.name.Contains("jaguar", System.StringComparison.CurrentCultureIgnoreCase));

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
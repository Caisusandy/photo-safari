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
        public SpawnController spawner;
        public List<EnemyController> enemies;
        public List<EnemyController> toBeDestroyed = new List<EnemyController>();
        public GameObject butterflyPrefab;

        internal int butterflyTotal = 0;
        internal int capybaraTotal = 0;
        internal int jaguarTotal = 0;
        internal int frogTotal = 0;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (enemies.Count(e => e != null && !e.TookedPicIcon.activeSelf) < gameManager.numButterfliesRequired)
            {
                spawner.TrySpawnAnimal(butterflyPrefab);
                butterflyTotal++;
            }

            bool enemiesFinishedTurn = enemies.All(e => e == null || e.finishedTurn || e.isDestroyed || toBeDestroyed.Contains(e));
            if (enemiesFinishedTurn && gameManager.State == GameState.ENEMYTURN)
            {
                foreach (var enemy in toBeDestroyed)
                {
                    enemies.Remove(enemy);
                }

                toBeDestroyed.Clear();
                gameManager.State = GameState.PLAYERTURN;
            }
        }

        internal void DetermineAnimalTotals()
        {
            butterflyTotal = enemies.Count(animal => animal.name.Contains("butterfly", System.StringComparison.CurrentCultureIgnoreCase));
            capybaraTotal = enemies.Count(animal => animal.name.Contains("capybara", System.StringComparison.CurrentCultureIgnoreCase));
            frogTotal = enemies.Count(animal => animal.name.Contains("frog", System.StringComparison.CurrentCultureIgnoreCase));
            jaguarTotal = enemies.Count(animal => animal.name.Contains("jaguar", System.StringComparison.CurrentCultureIgnoreCase));
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
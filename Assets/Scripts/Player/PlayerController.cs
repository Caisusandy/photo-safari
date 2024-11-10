using Minerva.Module.Tasks;
using Safari.Animals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Safari.Player
{
    public class PlayerController : EntityController
    {
        public static PlayerController instance;

        [Header("Health")]
        public int maxHealth = 3;
        [SerializeField]
        private int currentHealth;

        [Header("References")]
        public PlayerMovement movementScript;
        public PlayerCamera cameraScript;
        public Transform stairs;

        [SerializeField]
        private bool waitForPlayerToReleaseDirection;


        public List<string> enemiesWithPictures = new List<string>();

        public int CurrentHealth
        {
            get => currentHealth; set
            {
                // decrease
                if (value < currentHealth)
                    BlockNextAction();
                currentHealth = value;
            }
        }


        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            TargetPosition = transform.position;
            currentHealth = maxHealth;
        }

        void Update()
        {
            if (CurrentHealth <= 0)
            {
                GameManager.instance.State = GameState.GAMEOVER;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, stairs.position) <= 0.5f)
            {
                HandlePlayerOnStairs();
            }

            bool acceptingPlayerInput = Vector2.Distance(transform.position, TargetPosition) <= .05f
                && GameManager.instance.State is GameState.PLAYERTURN
                && !waitForPlayerToReleaseDirection;

            if (!acceptingPlayerInput)
                return;

            Vector3 finalMoveLocation = movementScript.DetermineMoveLocation();
            if (Vector2.Distance(finalMoveLocation, TargetPosition) != 0)
            {
                var hasEnemy = positionMap.TryGetValue(Vector2Int.FloorToInt(finalMoveLocation), out var enemy);// = EnemyManager.instance.CouldHitEnemy(finalMoveLocation);
                if (hasEnemy)
                {
                    // take damage and don't move
                    CurrentHealth--;
                    TextBoxController.instance.AddNewMessage(new Message($"You walked into the {enemy.name} and it attacked you!")); // The player taking damage is technically the enemy's action, so the enemy doesn't get to move again.
                    return;
                }

                if (movementScript.HandlePlayerMove(finalMoveLocation))
                {
                    GameManager.instance.State = GameState.ENEMYTURN;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                cameraScript.TakePicture();
                GameManager.instance.State = GameState.ENEMYTURN;
            }
        }

        public async void BlockNextAction(float time = 1)
        {
            waitForPlayerToReleaseDirection = true;
            await UnityTask.WaitForSeconds(time);
            waitForPlayerToReleaseDirection = false;
        }

        private void OnDestroy()
        {
            instance = null;
        }



        private void HandlePlayerOnStairs()
        {
            int enemyCount = EnemyManager.instance.enemies.Count;
            int numEnemiesWithPictures = enemiesWithPictures.Count;
            if (numEnemiesWithPictures < enemyCount)
            {
                string winConMessage = $"You still need to take a picture of {enemyCount - numEnemiesWithPictures} animal";
                if (enemyCount - numEnemiesWithPictures > 1)
                {
                    winConMessage += "s";
                }

                if (!TextBoxController.instance.textBoxMessage.Contains(winConMessage))
                {
                    TextBoxController.instance.AddNewMessage(new Message(winConMessage));
                }
            }
            else
            {
                GameManager.instance.State = GameState.WON;
            }
        }
    }
}
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

        [SerializeField]
        private bool waitForPlayerToReleaseDirection;

        internal Direction currentDirection = Direction.Down; // the direction that the player is currently facing
        internal Direction inputDirection = Direction.None;

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

            if (Vector2.Distance(transform.position, GameManager.instance.stairs.position) <= 0.5f)
            {
                HandlePlayerOnStairs();
            }

            bool acceptingPlayerInput = Vector2.Distance(transform.position, TargetPosition) <= .05f
                && GameManager.instance.State is GameState.PLAYERTURN
                && !waitForPlayerToReleaseDirection;

            if (!acceptingPlayerInput)
                return;


            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (GameManager.instance.targetTile.activeSelf)
                {
                    GameManager.instance.targetTile.SetActive(false);
                }

                cameraScript.TakePicture();
                GameManager.instance.State = GameState.ENEMYTURN;
                return;
            }

            inputDirection = GetInputDirection();
            if (inputDirection != Direction.None)
            {
                currentDirection = inputDirection;
            }

            UpdateSprite(currentDirection);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                GameManager.instance.targetTile.transform.position = transform.position + currentDirection.ToVector3();

                if (!GameManager.instance.targetTile.activeSelf)
                {
                    GameManager.instance.targetTile.SetActive(true);
                }

                return;
            }

            if (GameManager.instance.targetTile.activeSelf)
            {
                GameManager.instance.targetTile.SetActive(false);
            }

            Vector3 finalMoveLocation = movementScript.DetermineMoveLocation();
            if (Vector2.Distance(finalMoveLocation, TargetPosition) != 0)
            {
                var hasEnemy = positionMap.TryGetValue(Vector2Int.FloorToInt(finalMoveLocation), out var enemy);
                if (hasEnemy)
                {
                    if (((EnemyController)enemy).isFragile)
                    {
                        enemy.Destroy();
                    }
                    else
                    {
                        // take damage and don't move
                        CurrentHealth--;
                        TextBoxController.instance.AddNewMessage(new Message($"You walked into the {enemy.name.Replace("(Clone)", "")} and it attacked you!")); // The player taking damage is technically the enemy's action, so the enemy doesn't get to move again.
                    }

                    return;
                }

                if (movementScript.HandlePlayerMove(finalMoveLocation))
                {
                    GameManager.instance.State = GameState.ENEMYTURN;
                }

                return;
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
            GameManager gameManager = GameManager.instance;
            int totalRemainingEnemies = gameManager.numButterfliesRequired + gameManager.numCapybarasRequired + gameManager.numFrogsRequired + gameManager.numJaguarsRequired;
            if (totalRemainingEnemies == 0)
            {
                GameManager.instance.State = GameState.WON;
            }
            else
            {
                string winConMessage = $"You still need to take a picture of {totalRemainingEnemies} animal";
                if (totalRemainingEnemies > 1)
                {
                    winConMessage += "s";
                }

                if (!TextBoxController.instance.textBoxMessage.Contains(winConMessage))
                {
                    TextBoxController.instance.AddNewMessage(new Message(winConMessage));
                }
            }
        }

        internal Direction GetInputDirection()
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                return Direction.Left;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                return Direction.Right;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                return Direction.Up;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                return Direction.Down;
            }
            else
            {
                return Direction.None;
            }
        }
    }
}
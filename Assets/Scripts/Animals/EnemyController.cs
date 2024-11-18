using Safari.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Safari.Animals
{
    public enum EnemyTrait { RANDOM, PATROL, TRACEPLAYER }

    /// <summary>
    /// Base class of all enemy, create subclass for complicate data class
    /// </summary>
    public class EnemyController : EntityController
    {
        public LayerMask collisionLayer;
        public bool isFragile;
        public SpriteRenderer spriteRenderer;

        /// <summary>
        /// Trait of enemy
        /// </summary>
        [Header("Traits")]
        [SerializeField]
        protected EnemyTrait enemyTrait;
        [SerializeField]
        protected bool isInSpecialActivity;
        [SerializeField]
        protected EnemyTrait specialTrait;

        [Header("Counter")]
        [SerializeField]
        protected int baseMovementTurn;
        [SerializeField]
        protected int baseMovementTurnCounter;
        [SerializeField]
        protected int specialActivityTurn;
        [SerializeField]
        protected int specialActivityTurnCounter;

        [Header("Patrol")]
        [Tooltip("If the animal is set to patrol, how many tiles it will move before turning 90 degrees")]
        [SerializeField]
        int patrolLength = 1;

        private int patrolCount = 0;

        protected Vector2 currentDir = new Vector2(1f, 0f);
        internal bool finishedTurn = false;

        protected EnemyTrait EnemyTrait => isInSpecialActivity ? specialTrait : enemyTrait;

        protected virtual void Start()
        {
            TargetPosition = transform.position;
            GameManager.OnGameStateChange += GameManager_OnGameStateChange;
        }

        protected virtual void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, moveSpeed * Time.deltaTime);
        }

        protected virtual void OnDestroy()
        { }

        private void GameManager_OnGameStateChange(GameState obj)
        {
            if (obj != GameState.ENEMYTURN)
            {
                return;
            }

            if (isInSpecialActivity)
            {
                specialActivityTurnCounter--;
                if (specialActivityTurnCounter >= 0) return;
                specialActivityTurnCounter = specialActivityTurn;
            }
            else
            {
                baseMovementTurnCounter--;
                if (baseMovementTurnCounter >= 0) return;
                baseMovementTurnCounter = baseMovementTurn;
            }

            OnEnemyTurn();
        }

        public virtual void OnEnemyTurn()
        {
            switch (EnemyTrait)
            {
                case EnemyTrait.RANDOM:
                    MoveRandom();
                    break;
                case EnemyTrait.PATROL:
                    MovePatrol();
                    break;
                default:
                    break;
            }
        }

        public virtual void MovePatrol()
        {
            Vector2 finalMoveLocation = TargetPosition;
            if (patrolCount >= patrolLength)
            {
                currentDir = new Vector2(-currentDir.y, currentDir.x);
                patrolCount = 0;
            }

            finalMoveLocation += currentDir;
            patrolCount++;
            HandleEnemyMove(finalMoveLocation);
        }


        public virtual void MoveRandom()
        {
            int moveBy = UnityEngine.Random.Range(-1, 2);
            while (moveBy == 0)
            {
                moveBy = UnityEngine.Random.Range(-1, 2);
            }

            Vector3 finalMoveLocation = TargetPosition;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                finalMoveLocation += new Vector3(moveBy, 0f, 0f);
            }
            else
            {
                finalMoveLocation += new Vector3(0f, moveBy, 0f);
            }

            HandleEnemyMove(finalMoveLocation);
        }

        public virtual void HandleEnemyMove(Vector2 finalMoveLocation)
        {
            // check for walls
            if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            {
                // check for player pos
                // use < 0.1 to avoid float calculation
                var rounded = Vector2Int.FloorToInt(finalMoveLocation);
                if (positionMap.TryGetValue(rounded, out var entity) && entity is PlayerController player)
                {
                    HandlePlayerCollision(player);
                }
                if (CanMove(finalMoveLocation))
                    TargetPosition = finalMoveLocation;
            }

            finishedTurn = true;
        }

        private void HandlePlayerCollision(PlayerController player)
        {
            if (isFragile)
            {
                // killed the entity, need to be better
                Destroy(gameObject);
            }
            else
            {
                // deal damage to player and don't move
                player.CurrentHealth--;
                player.TargetPosition = player.transform.position;

                TextBoxController.instance.AddNewMessage(new Message($"You were in the {name.Replace("(Clone)", "")}'s way so it attacked you!"));
                Debug.Log("Player moved where enemy was heading. Current Health: " + PlayerController.instance.CurrentHealth);
                patrolCount--;
            }
        }
    }
}
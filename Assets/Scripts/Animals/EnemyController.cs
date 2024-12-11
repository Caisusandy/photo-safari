using Safari.Player;
using UnityEngine;

namespace Safari.Animals
{
    public enum EnemyTrait { RANDOM, PATROL, TRACEPLAYER, FLEEING }

    /// <summary>
    /// Base class of all enemy, create subclass for complicate data class
    /// </summary>
    public class EnemyController : EntityController
    {
        public LayerMask collisionLayer;
        public bool isFragile;

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
        {
            GameManager.OnGameStateChange -= GameManager_OnGameStateChange;

            // update animal count
            if (name.Contains("butterfly", System.StringComparison.CurrentCultureIgnoreCase))
            {
                EnemyManager.instance.butterflyTotal--;
            }

            EnemyManager.instance.enemies.Remove(this);
            if (positionMap.TryGetValue(Index, out var e) && e == this)
                positionMap.Remove(Index);
            Debug.Log($"Destroyed {name}");
        }

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
            if (isDestroyed) return;

            switch (EnemyTrait)
            {
                case EnemyTrait.RANDOM:
                    MoveRandom();
                    break;
                default:
                    break;
            }
        }

        protected Vector3 PickRandomMoveLocation()
        {
            int moveBy = Random.Range(-1, 2);
            while (moveBy == 0)
            {
                moveBy = Random.Range(-1, 2);
            }

            Vector3 finalMoveLocation = TargetPosition;
            if (Random.Range(0, 2) == 0)
            {
                finalMoveLocation += new Vector3(moveBy, 0f, 0f);
            }
            else
            {
                finalMoveLocation += new Vector3(0f, moveBy, 0f);
            }

            return finalMoveLocation;
        }


        public virtual void MoveRandom()
        {
            Vector3 finalMoveLocation = PickRandomMoveLocation();
            StartEnemyTurn(finalMoveLocation);
        }

        public virtual void StartEnemyTurn(Vector2 finalMoveLocation)
        {
            HandleEnemyMove(finalMoveLocation);
            finishedTurn = true;
        }

        public virtual void HandleEnemyMove(Vector2 finalMoveLocation)
        {
            // check for walls
            if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            {
                // check for player pos
                // use < 0.1 to avoid float calculation
                var rounded = Vector2Int.FloorToInt(finalMoveLocation);
                UpdateSprite(finalMoveLocation);
                if (positionMap.TryGetValue(rounded, out var entity) && entity is PlayerController player)
                {
                    HandlePlayerCollision(player);
                }

                if (CanMove(finalMoveLocation))
                {
                    TargetPosition = finalMoveLocation;
                }
            }
        }

        public virtual void HandlePlayerCollision(PlayerController player)
        {
            if (isFragile)
            {
                // killed the entity, need to be better
                Destroy();
            }
            else
            {
                // deal damage to player and don't move
                player.CurrentHealth--;
                player.TargetPosition = player.transform.position;

                TextBoxController.instance.AddNewMessage(new Message($"You were in the {name.Replace("(Clone)", "")}'s way so it attacked you!"));
                Debug.Log("Player moved where enemy was heading. Current Health: " + PlayerController.instance.CurrentHealth);
            }
        }
    }
}
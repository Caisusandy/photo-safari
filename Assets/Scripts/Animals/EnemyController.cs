using NUnit.Framework;
using Safari.MapComponents;
using Safari.Player;
using System.Collections.Generic;
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
        [SerializeField]
        protected GameObject tookedPicIcon;

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

        public GameObject TookedPicIcon { get => tookedPicIcon; set => tookedPicIcon = value; }

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

            for (int i = 0; i < 100; i++)
            {
                Vector3 finalMoveLocation = TargetPosition;
                if (Random.Range(0, 2) == 0)
                {
                    finalMoveLocation += new Vector3(moveBy, 0f, 0f);
                }
                else
                {
                    finalMoveLocation += new Vector3(0f, moveBy, 0f);
                }

                Vector2Int point = Chunk.ToChunk(Vector3Int.FloorToInt(finalMoveLocation));
                if (!Map.instance.data.IsOutOfBound(point) && !Map.instance.data.IsNoRoom(point))
                    return finalMoveLocation;
            }
            return TargetPosition;
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

                if (CanMove(finalMoveLocation) && !PosBlockingHallway(finalMoveLocation))
                {
                    TargetPosition = finalMoveLocation;
                }
            }
        }

        public virtual bool PosInHallway(Vector2 finalMoveLocation)
        {
            bool posInVerticalHallway = Physics2D.OverlapCircle(finalMoveLocation + Direction.Right.ToVector2(), .2f, collisionLayer) &&
                Physics2D.OverlapCircle(finalMoveLocation + Direction.Left.ToVector2(), .2f, collisionLayer);
            bool posInHorzHallway = Physics2D.OverlapCircle(finalMoveLocation + Direction.Up.ToVector2(), .2f, collisionLayer) &&
                Physics2D.OverlapCircle(finalMoveLocation + Direction.Down.ToVector2(), .2f, collisionLayer);
            return posInVerticalHallway || posInHorzHallway;
        }

        public virtual bool PosBlockingHallway(Vector2 finalMoveLocation)
        {
            List<Vector2> adjacentPositions = new List<Vector2>()
            {
                Direction.Up.ToVector2(),
                Direction.Down.ToVector2(),
                Direction.Left.ToVector2(),
                Direction.Right.ToVector2(),
            };

            foreach (Vector2 position in adjacentPositions)
            {
                if (PosInHallway(position + finalMoveLocation))
                {
                    return true;
                }
            }

            return false;
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
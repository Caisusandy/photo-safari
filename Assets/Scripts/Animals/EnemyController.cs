using Safari.Player;
using UnityEngine;

namespace Safari.Animals
{
    public enum EnemyTrait { RANDOM, PATROL }

    /// <summary>
    /// Base class of all enemy, create subclass for complicate data class
    /// </summary>
    public class EnemyController : EntityController
    {
        public LayerMask collisionLayer;
        public GameManager gameManager;
        public TextBoxController textBox;

        internal bool finishedTurn = false;

        [SerializeField]
        EnemyTrait enemyTrait;

        [Tooltip("If the animal is set to patrol, how many tiles it will move before turning 90 degrees")]
        [SerializeField]
        int patrolLength = 1;

        private int patrolCount = 0;
        private Vector2 currentDir = new Vector2(1f, 0f);



        void Start()
        {
            TargetPosition = transform.position;
            GameManager.OnGameStateChange += GameManager_OnGameStateChange;
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, moveSpeed * Time.deltaTime);
        }

        private void GameManager_OnGameStateChange(GameState obj)
        {
            if (obj != GameState.ENEMYTURN)
            {
                return;
            }
            OnEnemyTurn();
        }

        public virtual void OnEnemyTurn()
        {
            switch (enemyTrait)
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

            HandleEnemyMove(finalMoveLocation);
        }

        public virtual void HandleEnemyMove(Vector2 finalMoveLocation)
        {
            // check for walls
            if (!Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            {
                // check for player pos
                // use < 0.1 to avoid float calculation
                //if (Vector2.Distance(PlayerController.instance.TargetPosition, finalMoveLocation) < 0.1f)
                var rounded = Vector2Int.FloorToInt(finalMoveLocation);
                if (positionMap.TryGetValue(rounded, out var entity) && entity is PlayerController player)
                {
                    // deal damage to player and don't move
                    player.CurrentHealth--;
                    player.TargetPosition = player.transform.position;

                    textBox.AddNewMessage(new Message($"You were in the {name}'s way so it attacked you!"));
                    Debug.Log("Player moved where enemy was heading. Current Health: " + PlayerController.instance.CurrentHealth);
                    patrolCount--;
                }
                if (CanMove(finalMoveLocation))
                    TargetPosition = finalMoveLocation;
            }

            finishedTurn = true;
        }
    }
}
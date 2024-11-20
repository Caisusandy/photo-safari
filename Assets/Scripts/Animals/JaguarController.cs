using Safari.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Safari.Animals
{
    public class JaguarController : EnemyController
    {
        private int distanceToSeePlayer = 5;
        private int moveAmount = 1;

        private Vector2? GetPlayerInView()
        {
            positionMap.Keys.ToList();
            foreach (var pos in positionMap.Keys)
            {
                bool playerInView = Vector2.Distance(pos, transform.position) <= distanceToSeePlayer &&
                    positionMap.TryGetValue(pos, out var entity) &&
                    entity is PlayerController player;
                if (playerInView)
                {
                    return pos;
                }
            }

            return null;
        }

        public override void OnEnemyTurn()
        {
            Vector2? detectedPlayerPos = GetPlayerInView();
            bool shouldSwitchToTracing = detectedPlayerPos != null && !isInSpecialActivity;
            if (shouldSwitchToTracing)
            {
                isInSpecialActivity = true;
                moveAmount = 4;
            }

            switch (EnemyTrait)
            {
                case EnemyTrait.RANDOM:
                    MoveRandom();
                    break;
                case EnemyTrait.TRACEPLAYER:
                    if (detectedPlayerPos != null)
                    {
                        MoveTracePlayer(detectedPlayerPos.Value);
                        moveAmount = 2;
                        Debug.Log($"{name} is tracing player");
                    }
                    else
                    {
                        isInSpecialActivity = false;
                        Debug.Log($"{name} lost interest in tracing player");
                    }
                    break;
                default:
                    break;
            }
        }

        public virtual void MoveTracePlayer(Vector2 detectedPlayerPos)
        {
            Vector3 finalMoveLocation = TargetPosition;

            // iterate through possible directions
            float bestDistance = Vector2.Distance(finalMoveLocation, detectedPlayerPos);
            Vector2 bestDirection = Vector2.up;
            List<Vector2> possibleDirs = new List<Vector2>()
            {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right,
            };
            foreach (var direction in possibleDirs)
            {
                finalMoveLocation = TargetPosition + direction;
                var rounded = Vector2Int.FloorToInt(finalMoveLocation);
                float currentDistance = Vector2.Distance(finalMoveLocation, detectedPlayerPos);

                bool dirIsValid = currentDistance < bestDistance &&
                    !Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer) &&
                    !positionMap.TryGetValue(rounded, out var entity);
                if (dirIsValid)
                {
                    bestDistance = currentDistance;
                    bestDirection = direction;
                }
            }

            finalMoveLocation = TargetPosition + bestDirection * moveAmount;
            HandleEnemyMove(finalMoveLocation);
        }
    }
}

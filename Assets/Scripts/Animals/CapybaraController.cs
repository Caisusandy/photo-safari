using Safari.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Safari.Animals
{
    public class CapybaraController : EnemyController
    {
        [Header("Patrol")]
        [Tooltip("If the animal is set to patrol, how many tiles it will move before turning 90 degrees")]
        [SerializeField]
        int patrolLength = 1;

        private int patrolCount = 0;


        private int distanceToSeeJaguar = 5;

        private Vector2? GetJaguarInView()
        {
            positionMap.Keys.ToList();
            foreach (var pos in positionMap.Keys)
            {
                bool jaguarInView = Vector2.Distance(pos, transform.position) <= distanceToSeeJaguar &&
                    positionMap.TryGetValue(pos, out var entity) &&
                    entity is JaguarController jaguar;
                if (jaguarInView)
                {
                    return pos;
                }
            }

            return null;
        }

        public override void OnEnemyTurn()
        {
            Vector2? detectedJaguarPos = GetJaguarInView();
            bool shouldSwitchToFleeing = detectedJaguarPos != null && !isInSpecialActivity;
            if (shouldSwitchToFleeing)
            {
                isInSpecialActivity = true;
            }

            switch (EnemyTrait)
            {
                case EnemyTrait.PATROL:
                    MovePatrol();
                    break;
                case EnemyTrait.FLEEING:
                    if (detectedJaguarPos != null)
                    {
                        MoveFleeing(detectedJaguarPos.Value);
                        Debug.Log($"{name} is fleeing from jaguar");
                    }
                    else
                    {
                        isInSpecialActivity = false;
                        Debug.Log($"{name} is no longer fleeing from jaguar");
                    }
                    break;
                default:
                    break;
            }
        }

        private void MovePatrol()
        {
            Vector2 finalMoveLocation = TargetPosition;
            if (patrolCount >= patrolLength)
            {
                currentDir = new Vector2(-currentDir.y, currentDir.x);
                patrolCount = 0;
            }

            finalMoveLocation += currentDir;
            patrolCount++;
            StartEnemyTurn(finalMoveLocation);
        }

        private void MoveFleeing(Vector2 detectedJaguarPos)
        {
            Vector3 finalMoveLocation = TargetPosition;

            // iterate through possible directions
            float bestDistance = Vector2.Distance(finalMoveLocation, detectedJaguarPos);
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
                float currentDistance = Vector2.Distance(finalMoveLocation, detectedJaguarPos);

                bool dirIsValid = currentDistance > bestDistance &&
                    !Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer) &&
                    !positionMap.TryGetValue(rounded, out var entity);
                if (dirIsValid)
                {
                    bestDistance = currentDistance;
                    bestDirection = direction;
                }
            }

            finalMoveLocation = TargetPosition + bestDirection;
            StartEnemyTurn(finalMoveLocation);
        }


        public override void HandlePlayerCollision(PlayerController player)
        {
            base.HandlePlayerCollision(player);
            if (!isFragile)
            {
                patrolCount--;
            }
        }
    }
}

using Safari.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Safari.Animals
{
    public class JaguarController : EnemyController
    {
        private int distanceToSeePlayer = 5;
        private int moveAmount = 1;

        private List<Vector3> PointsInView
        {
            get
            {
                List<Vector3> pointsInView = new List<Vector3>();

                for (int i = 1; i <= distanceToSeePlayer; i++)
                {
                    if (i > 1)
                    {
                        pointsInView.Add(new Vector3(transform.position.x + i - 1, transform.position.y + i - 1));
                        pointsInView.Add(new Vector3(transform.position.x + i - 1, transform.position.y - i - 1));
                        pointsInView.Add(new Vector3(transform.position.x - i - 1, transform.position.y + i - 1));
                        pointsInView.Add(new Vector3(transform.position.x - i - 1, transform.position.y - i - 1));
                    }

                    pointsInView.Add(new Vector3(transform.position.x + i, transform.position.y));
                    pointsInView.Add(new Vector3(transform.position.x - i, transform.position.y));
                    pointsInView.Add(new Vector3(transform.position.x, transform.position.y + i));
                    pointsInView.Add(new Vector3(transform.position.x, transform.position.y - i));
                }

                return pointsInView;
            }
        }

        private Vector2? GetPlayerInView()
        {
            foreach (var pos in PointsInView)
            {
                var rounded = Vector2Int.FloorToInt(pos);
                if (positionMap.TryGetValue(rounded, out var entity) && entity is PlayerController player)
                {
                    Debug.Log($"{name} can see player");
                    return rounded;
                }
            }

            return null;
        }

        public override void OnEnemyTurn()
        {
            Vector2? detectedPlayerPos = GetPlayerInView();
            bool shouldSwitchToTracing = detectedPlayerPos != null && specialTrait is EnemyTrait.TRACEPLAYER && !isInSpecialActivity;
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
                        MoveTracePlayer(new Vector2(detectedPlayerPos.Value.x, detectedPlayerPos.Value.y));
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
            Vector2 finalMoveLocation = TargetPosition;
            if (Mathf.Abs(detectedPlayerPos.x - transform.position.x) > Mathf.Abs(detectedPlayerPos.y - transform.position.y))
            {
                currentDir = new Vector2(Mathf.Sign(detectedPlayerPos.x - transform.position.x) * moveAmount, 0f);
            }
            else
            {
                currentDir = new Vector2(0f, Mathf.Sign(detectedPlayerPos.y - transform.position.y) * moveAmount);
            }

            finalMoveLocation += currentDir;
            HandleEnemyMove(finalMoveLocation);

            if (moveAmount != 2)
            {
                moveAmount = 2;
            }
        }

    }
}

using Safari;
using Safari.Animals;
using Safari.MapComponents;
using TMPro;
using UnityEngine;

namespace Safari.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public LayerMask collisionLayer;

        public PlayerController controller;

        public Vector3 DetermineMoveLocation()
        {
            Vector3 finalMoveLocation = controller.TargetPosition;

            switch (controller.inputDirection)
            {
                case Direction.Right:
                    finalMoveLocation += new Vector3(1f, 0f, 0f);
                    break;
                case Direction.Up:
                    finalMoveLocation += new Vector3(0, 1f, 0f);
                    break;
                case Direction.Left:
                    finalMoveLocation += new Vector3(-1, 0f, 0f);
                    break;
                case Direction.Down:
                    finalMoveLocation += new Vector3(0f, -1f, 0f);
                    break;
                default:
                    break;
            }

            return finalMoveLocation;
        }

        /// <summary>
        /// Checks collision with walls and check if player is still moving before updating
        /// target position
        /// </summary>
        /// <param name="finalMoveLocation">The calculated position the player will move to</param>
        /// <returns></returns>
        public bool HandlePlayerMove(Vector3 finalMoveLocation)
        {
            // hit wall
            //if (Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
            if (Map.instance.geometry.HasTile(Vector3Int.FloorToInt(finalMoveLocation)))
            {
                Debug.Log("Has tile");
                return false;
            }
            // if player is still moving then don't move
            if (Vector2.Distance(controller.TargetPosition, controller.transform.position) > 0.01f)
            {
                Debug.Log("is already moving");
                return false;
            }

            controller.TargetPosition = finalMoveLocation;
            return true;
        }
    }
}
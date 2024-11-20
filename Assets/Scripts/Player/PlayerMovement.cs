using Safari;
using Safari.Animals;
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
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                finalMoveLocation += new Vector3(-1, 0f, 0f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                finalMoveLocation += new Vector3(1f, 0f, 0f);
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                finalMoveLocation += new Vector3(0, 1f, 0f);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                finalMoveLocation += new Vector3(0f, -1f, 0f);
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
            if (Physics2D.OverlapCircle(finalMoveLocation, .2f, collisionLayer))
                return false;
            // if player is still moving then don't move
            if (Vector2.Distance(controller.TargetPosition, controller.transform.position) > 0.01f)
                return false;

            controller.TargetPosition = finalMoveLocation;
            return true;
        }
    }
}
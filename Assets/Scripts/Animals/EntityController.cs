using System.Collections.Generic;
using UnityEngine;

namespace Safari.Animals
{
    public class EntityController : MonoBehaviour
    {
        public static Dictionary<Vector2Int, EntityController> positionMap = new();

        public float moveSpeed = 5f;
        [Tooltip("The point that the enemy moves towards")]
        [SerializeField]
        private Vector2 targetPosition;
        private Vector2Int index;

        /// <summary>
        /// The targeting position of the entity, if entity is not moving then it is the current position
        /// </summary>
        public Vector2 TargetPosition
        {
            get => targetPosition;
            set
            {
                targetPosition = value;
                UpdateSelfPosition();
            }
        }

        /// <summary>
        /// Update self position on the position map
        /// </summary>
        public void UpdateSelfPosition()
        {
            positionMap.Remove(index);
            index = Vector2Int.FloorToInt(targetPosition);
            positionMap.Add(index, this);
        }

        /// <summary>
        /// Check destination is valid to move to
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CanMove(Vector3 position)
        {
            var rounded = Vector2Int.FloorToInt(position);
            // not having key or value (entity) is invalid (destroyed)
            return !positionMap.TryGetValue(rounded, out var controller) || !controller;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Sprites")]
        public SpriteRenderer spriteRenderer;
        public Sprite upSprite;
        public Sprite downSprite;
        public Sprite leftSprite;
        public Sprite rightSprite;
        public Animator animator;
        public bool isDestroyed;

        protected Vector2Int Index => index;

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

        private void Awake()
        {
            positionMap = new();
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

        [ContextMenu(nameof(PrintMap))]
        public void PrintMap()
        {
            Debug.Log(string.Join('\n', positionMap.Select(p => $"{p.Key}: {p.Value}")));
        }

        private void ChangeSprite(Vector2 direction)
        {
            if (direction.y < 0 && downSprite != null)
            {
                spriteRenderer.sprite = downSprite;
                return;
            }

            if (direction.y > 0 && upSprite != null)
            {
                spriteRenderer.sprite = upSprite;
                return;
            }

            if (direction.x < 0)
            {
                if (leftSprite != null)
                {
                    spriteRenderer.sprite = leftSprite;
                    spriteRenderer.flipX = false;
                }
                else if (rightSprite != null)
                {
                    spriteRenderer.sprite = rightSprite;
                    spriteRenderer.flipX = true;
                }

                return;
            }

            if (direction.x > 0)
            {
                if (rightSprite != null)
                {
                    spriteRenderer.sprite = rightSprite;
                    spriteRenderer.flipX = false;
                }
                else if (leftSprite != null)
                {
                    spriteRenderer.sprite = leftSprite;
                    spriteRenderer.flipX = true;
                }

                return;
            }
        }

        protected void UpdateSprite(Direction direction)
        {
            ChangeSprite(DirectionExtensions.ToVector2(direction));
        }

        protected void UpdateSprite(Vector3 finalMoveLocation)
        {
            Vector2 direction = new Vector2(finalMoveLocation.x, finalMoveLocation.y) - TargetPosition;

            // this is just for the butterfly right now so the only directions will be left and right
            if (animator != null)
            {
                spriteRenderer.flipX = direction.x < 0;
                return;
            }

            ChangeSprite(direction);
        }

        [ContextMenu(nameof(PrintPositionDebug))]
        public void PrintPositionDebug()
        {
            Debug.Log($"{index} {positionMap.TryGetValue(index, out var value)} {value == this}");
        }

        internal void Destroy()
        {
            if (isDestroyed) return;

            // update animal count
            if (name.Contains("butterfly", StringComparison.CurrentCultureIgnoreCase))
            {
                EnemyManager.instance.butterflyTotal--;
            }

            if (positionMap.TryGetValue(Index, out var e) && e == this)
            {
                positionMap.Remove(Index);
            }

            EnemyManager.instance.toBeDestroyed.Add((EnemyController)this);
            Debug.Log($"Destroyed {name}");

            isDestroyed = true;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            positionMap = null;
        }
    }
}
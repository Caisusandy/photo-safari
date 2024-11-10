using Safari.Player;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Safari.Animals
{
    public class FrogController : EnemyController
    {
        static readonly Vector2Int[] neighbors = new Vector2Int[]  {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };

        protected override void Start()
        {
            base.Start();
            GameManager.OnGameStateChange += TryHide;
            DetectEnemy();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.OnGameStateChange -= TryHide;
        }

        private void TryHide(GameState obj)
        {
            if (obj == GameState.PLAYERTURN)
            {
                DetectEnemy();
            }
            if (obj == GameState.ENEMYTURN)
            {
                var index = Vector2Int.FloorToInt(TargetPosition);
                // temporary remove the frog from the map so player can't take photo
                positionMap[index] = this;
            }
        }

        private void DetectEnemy()
        {
            var scared = neighbors.Select(p =>
            {
                var position = Vector2Int.FloorToInt(TargetPosition + p);
                if (positionMap.TryGetValue(position, out var controller)) return controller;
                return null;
            }).Any(e => e && e is not PlayerController and not FrogController);

            var index = Index;
            if (scared)
            {
                // temporary remove the frog from the map so player can't take photo
                positionMap.Remove(index);
                Debug.Log("scared");
                ChangeAlphaHide();
            }
            else
            {
                Debug.Log("normal");
                UpdateSelfPosition();
                ChangeAlphaShow();
            }
        }

        private async void ChangeAlphaHide()
        {
            var t = 0f;
            if (spriteRenderer.color.a <= 0.5001f) return;
            while (t < 1f && this)
            {
                var color = spriteRenderer.color;
                color.a = 1 - t / 2;
                spriteRenderer.color = color;
                t += Time.deltaTime;
                await Task.Yield();
            }
        }

        private async void ChangeAlphaShow()
        {
            if (spriteRenderer.color.a > 0.999f) return;
            var t = 0f;
            while (t < 1f && this)
            {
                var color = spriteRenderer.color;
                color.a = 0.5f + t / 2;
                spriteRenderer.color = color;
                t += Time.deltaTime;
                await Task.Yield();
            }
        }
    }
}
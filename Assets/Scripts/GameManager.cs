using Minerva.Module;
using Safari.MapComponents;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari
{
    public enum GameState { PLAYERTURN, ENEMYTURN, WON, GAMEOVER }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField]
        private GameState state;
        public GameObject gameOverText;
        public GameObject winText;
        public TextBoxController textBox;

        public static event Action<GameState> OnGameStateChange;

        public GameState State
        {
            get => state;
            set
            {
                state = value;
                if (OnGameStateChange == null) return;
                foreach (var item in OnGameStateChange?.GetInvocationList())
                {
                    try
                    {
                        item.DynamicInvoke(state);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    if (state != value)
                    {
                        Debug.LogError($"State changed: from {value} to {state}");
                    }
                }
            }
        }

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            State = GameState.PLAYERTURN;
            gameOverText.SetActive(false);
            winText.SetActive(false);
            textBox.AddNewMessage(new Message(3f, "Use the arrow keys or WASD to move. Press SPACE to take a picture of the animals. Once you've finished exploring use the stairs to advance."));
        }

        private void Update()
        {
            switch (State)
            {
                case GameState.WON:
                    winText.SetActive(true);
                    break;
                case GameState.GAMEOVER:
                    gameOverText.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
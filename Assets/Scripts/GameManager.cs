using Safari.Animals;
using Safari.MapComponents.Generators;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Safari
{
    public enum GameState { PLAYERTURN, ENEMYTURN, WON, GAMEOVER }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField]
        private GameState state;

        [Header("UI Elements")]
        public GameObject gameOverText;
        public GameObject winText;
        public TextBoxController textBox;

        [Header("Scripts")]
        public SpawnController spawnController;
        public MapGeneratorRunner mapGenerator;

        [Header("Player Variables")]
        public Transform stairs;
        public CameraFlash cameraFlash;
        public GameObject targetTile;

        public static event Action<GameState> OnGameStateChange;

        [Header("Win Requirement Variables")]
        public int minPhotosRequired;
        public int numButterfliesRequired = 0;
        public int numCapybarasRequired = 0;
        public int numFrogsRequired = 0;
        public int numJaguarsRequired = 0;

        public List<EnemyController> enemiesWithPictures = new List<EnemyController>();

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
                }
            }
        }

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            mapGenerator.ResetMapComponent();
            mapGenerator.RunAndInstantiate();

            spawnController.SpawnObjects();

            DetermineAnimalTargets();
            InitializeUi();

            State = GameState.PLAYERTURN;
        }

        private void DetermineAnimalTargets()
        {
            EnemyManager.instance.DetermineAnimalTotals();
            int numPhotosRequired = -1;
            while (numPhotosRequired < minPhotosRequired)
            {
                numButterfliesRequired = UnityEngine.Random.Range(0, EnemyManager.instance.butterflyTotal + 1);
                numCapybarasRequired = UnityEngine.Random.Range(0, EnemyManager.instance.capybaraTotal + 1);
                numFrogsRequired = UnityEngine.Random.Range(0, EnemyManager.instance.frogTotal + 1);
                numJaguarsRequired = UnityEngine.Random.Range(0, EnemyManager.instance.jaguarTotal + 1);
                numPhotosRequired = numButterfliesRequired + numCapybarasRequired + numFrogsRequired + numJaguarsRequired;
            }
        }

        private void InitializeUi()
        {
            // disable elements that shouldn't be visible
            gameOverText.SetActive(false);
            winText.SetActive(false);
            targetTile.SetActive(false);

            textBox.AddNewMessage(new Message(3f, "Use the arrow keys or WASD to move. Press SPACE to take a picture of the animals and shift to adjust your angle. Once you've finished exploring use the stairs to advance."));
        }

        private void Update()
        {
            switch (State)
            {
                case GameState.WON:
                    winText.SetActive(true);
                    if (Input.GetKey(KeyCode.Space))
                    {
                        SceneManager.LoadScene(0);
                    }
                    break;
                    break;
                case GameState.GAMEOVER:
                    gameOverText.SetActive(true);
                    if (Input.GetKey(KeyCode.Space))
                    {
                        SceneManager.LoadScene(0);
                    }
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
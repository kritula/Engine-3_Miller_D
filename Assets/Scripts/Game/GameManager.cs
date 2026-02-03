using UnityEngine;

namespace OmniumLessons
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterFactory _characterFactory;
        [SerializeField] private GameData _gameData;
        [SerializeField] private WindowsService _windowsService;

        private bool _isGameActive = false;
        private  float _gameTimeSec = 0;
        
        public static GameManager Instance { get; private set; }
        
        public ScoreManager ScoreManager { get; private set; }
        public WindowsService WindowsService => _windowsService;
        public CharacterFactory CharacterFactory => _characterFactory;
        public GameData GameData => _gameData;
        public float GameTime => _gameTimeSec;

        private CharacterSpawnController _spawnController;

        // Добавим концептуально еще понятие "игра на паузе", в отличие от нашего урока. В целом, ничего не поменяется, но сам контроль над игрой станет более гибким.
        public bool IsGamePaused
        {
            get;
            set;
        } = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                Initialize();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Initialize()
        {
            ScoreManager = new ScoreManager();
            _windowsService.Initialize();
        }

        public void StartGame()
        {
            if (_isGameActive)
            {
                Debug.Log("Game is already active");
                return;
            }
            
            var player = CharacterFactory.CreateCharacter(CharacterType.DefaultPlayer);
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            RegisterCharacter(player);

            _gameTimeSec = 0f;
            ScoreManager.StartGame();

            // 4) Запускаем спавн-контроллер
            _spawnController = new CharacterSpawnController();
            _spawnController.StartSpawn();

            _isGameActive = true;
            IsGamePaused = false;
        }

        private void Update()
        {
            if (!_isGameActive || IsGamePaused)
                return;
            
            _gameTimeSec += Time.deltaTime;

            // Спавн теперь живёт здесь
            _spawnController.OnUpdate(Time.deltaTime);

            if (_gameTimeSec >= _gameData.GameTimeSecondsMax)
            {
                GameVictory();
            }
        }

        // ВАЖНО: сюда будет обращаться SpawnController после создания врага
        public void RegisterCharacter(Character character)
        {
            if (character == null || character.LiveComponent == null)
                return;

            character.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;
        }

        private void OnCharacterDeathHandler(Character deathCharacter)
        {
            Debug.Log("character " + deathCharacter.gameObject.name + " is dead");
            switch (deathCharacter.CharacterType)
            {
                case CharacterType.DefaultPlayer:
                    GameOver();
                    break;
                case CharacterType.DefaultEnemy:
                    ScoreManager.AddScore(deathCharacter.CharacterData.ScoreCost);
                    Debug.Log("Score = " + ScoreManager.GameScore);
                    break;
            }
        
            CharacterFactory.ReturnCharacterToPool(deathCharacter);
            deathCharacter.gameObject.SetActive(false);

            // отписка
            deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
        }
        
        private void GameOver()
        {
            Debug.Log("GameOver!");
            Debug.Log("Score = " + ScoreManager.GameScore);
            Debug.Log("ScoreMax = " + ScoreManager.ScoreMax);
            ScoreManager.CompleteMatch();
            _isGameActive = false;
            IsGamePaused = true;

            // останавливаем спавн
            _spawnController.StopSpawn();
            WindowsService.ShowWindow<DefeatWindow>(false);
        }

        private void GameVictory()
        {
            Debug.Log("Game Over! Time's up!");
            ScoreManager.CompleteMatch();
            _isGameActive = false;
            IsGamePaused = true;

            // останавливаем спавн
            _spawnController.StopSpawn();
            WindowsService.ShowWindow<VictoryWindow>(false);
        }
    }
}
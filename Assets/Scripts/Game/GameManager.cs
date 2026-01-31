using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CharacterFactory _characterFactory;
        [SerializeField] private GameData _gameData;
        
        private bool _isGameActive = false;
        private  float _gameTimeSec = 0;
        
        public static GameManager Instance { get; private set; }
        
        public ScoreManager ScoreManager { get; private set; }
        
        public CharacterFactory CharacterFactory => _characterFactory;
        public GameData GameData => _gameData;

        private CharacterSpawnController _spawnController;

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
        }

        private void Update()
        {
            if (!_isGameActive)
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

            // останавливаем спавн
            _spawnController.StopSpawn();
        }

        private void GameVictory()
        {
            Debug.Log("Game Over! Time's up!");
            ScoreManager.CompleteMatch();
            _isGameActive = false;

            // останавливаем спавн
            _spawnController.StopSpawn();
        }
    }
}
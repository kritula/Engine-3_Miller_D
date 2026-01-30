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
        private  float _timeBetweenEnemySpawn = 0;
        
        public static GameManager Instance { get; private set; }
        
        
        public ScoreManager ScoreManager { get; private set; }
        
        public CharacterFactory CharacterFactory => _characterFactory;
        
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
            player.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;

            _gameTimeSec = 0f;
            _timeBetweenEnemySpawn = _gameData.TimeBetweenEnemySpawn;
            ScoreManager.StartGame();
        
            _isGameActive = true;
        }

        private void Update()
        {
            if (!_isGameActive)
                return;
            
            _gameTimeSec += Time.deltaTime;
            _timeBetweenEnemySpawn += Time.deltaTime;
        
            if (_timeBetweenEnemySpawn >= _gameData.TimeBetweenEnemySpawn)
            {
                SpawnEnemy();
                _timeBetweenEnemySpawn = 0;
            }
        
            if (_gameTimeSec >= _gameData.GameTimeSecondsMax)
            {
                GameVictory();
            }
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
        
            deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
        }
        
        private void SpawnEnemy()
        {
            var character = CharacterFactory.CreateCharacter(CharacterType.DefaultEnemy);

            float posX = CharacterFactory.Player.transform.position.x + GetOffset();
            float posZ = CharacterFactory.Player.transform.position.z + GetOffset();
            Vector3 spawnPoint = new Vector3(posX, 0, posZ);
            character.transform.position = spawnPoint;
            
            character.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;
            character.gameObject.SetActive(true);

        
            
            float GetOffset()
            {
                bool isPlus = Random.Range(0, 1) > 0;
                float randomOffset = Random.Range(_gameData.MinEnemySpawnOffset, _gameData.MaxEnemySpawnOffset);
                
                return isPlus
                    ? randomOffset
                    : -randomOffset;
            }
        }

        private void GameOver()
        {
            Debug.Log("GameOver!");
            Debug.Log("Score = " + ScoreManager.GameScore);
            Debug.Log("ScoreMax = " + ScoreManager.ScoreMax);
            ScoreManager.CompleteMatch();
            _isGameActive = false;
        }

        private void GameVictory()
        {
            Debug.Log("Game Over! Time's up!");
            ScoreManager.CompleteMatch();
            _isGameActive = false;
        }
    }
}
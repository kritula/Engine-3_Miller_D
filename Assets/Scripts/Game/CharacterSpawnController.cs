using UnityEngine;

namespace OmniumLessons
{
    public class CharacterSpawnController
    {
        private CharacterFactory CharacterFactory => GameManager.Instance.CharacterFactory;
        private GameData GameData => GameManager.Instance.GameData;

        //внутринние счетчики
        private float _gameTimeSeconds;
        private float _spawnTimerEnemy;

        //параметры роста спавна
        private int _baseMaxEnemies = 2;
        private int _enemiesAddedPerStep = 1;

        private float _enemyGrowthIntervalSeconds = 10f; // <-- рост лимита каждые 10 сек

        private bool _isActiveSpawn;

        public void StartSpawn()
        {
            _isActiveSpawn = true;
            _gameTimeSeconds = 0f;
            _spawnTimerEnemy = 0f;

            _baseMaxEnemies = 2;
        }

        public void StopSpawn()
        {
            _isActiveSpawn = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isActiveSpawn)
                return;

            _gameTimeSeconds += deltaTime;
            _spawnTimerEnemy += deltaTime;
            // считаем, сколько раз вырос лимит
            int growthSteps = (int)(_gameTimeSeconds / _enemyGrowthIntervalSeconds);

            // считаем текущий максимум врагов
            int maxEnemiesNow = _baseMaxEnemies + growthSteps * _enemiesAddedPerStep;

            // Считаем, сколько врагов УЖЕ есть
            int currentEnemies = CountActiveEnemies();

            // Если врагов уже достаточно — не спавним
            if (currentEnemies >= maxEnemiesNow)
                return;

            // Проверяем, прошло ли достаточно времени для нового врага
            if (_spawnTimerEnemy >= GameData.TimeBetweenEnemySpawn)
            {
                SpawnEnemy();
                _spawnTimerEnemy = 0;  // сбрасываем таймер
            }
        }

        private int CountActiveEnemies()
        {
            int count = 0;
            var active = CharacterFactory.ActiveCharacters;

            for (int i = 0; i < active.Count; i++)
            {
                if (active[i].CharacterType == CharacterType.DefaultEnemy && active[i].LiveComponent.IsAlive)
                    count++;
            }

            return count;
        }

        private void SpawnEnemy()
        {
            Character enemy = CharacterFactory.CreateCharacter(CharacterType.DefaultEnemy);

            float posX = CharacterFactory.Player.transform.position.x + GetOffset();
            float posZ = CharacterFactory.Player.transform.position.z + GetOffset();
            Vector3 spawnPoint = new Vector3(posX, 0, posZ);
            enemy.transform.position = spawnPoint;

            GameManager.Instance.RegisterCharacter(enemy);
            enemy.gameObject.SetActive(true);


            float GetOffset()
            {
                bool isPlus = Random.value > 0.5f;
                float randomOffset = Random.Range(GameData.MinEnemySpawnOffset, GameData.MaxEnemySpawnOffset);

                return isPlus
                    ? randomOffset
                    : -randomOffset;
            }
        }
    }
}

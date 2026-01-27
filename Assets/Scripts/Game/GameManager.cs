using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class GameManager : MonoBehaviour
    {
        // все ссылки на наши данные и внешние классы
        [SerializeField] private CharacterFactory _characterFactory;
        [SerializeField] private GameData _gameData;
        
        private bool _isGameActive = false;
        private  float _gameTimeSec = 0;
        private  float _timeBetweenEnemySpawn = 0;
        
        // static означает, что элемент принадлежит самому классу, а не его конкретным экземплярам
        // Класс (не экземпляр класса) GameManager на всю игру всего один,
        // поэтому и наше static свойство Instance будет одно на всю игру.
        // А зная, что он один, исключительный, мы можем получить его по имени класса, что вы увидите позднее по коду.
        public static GameManager Instance { get; private set; }
        
        
        public ScoreManager ScoreManager { get; private set; }
        
        public CharacterFactory CharacterFactory => _characterFactory;
        
        // Метод Awake() аналогичен методу Start(), но вызывается раньше в этом же кадре.
        // То есть, сначала будут вызваны все методы Awake() новых объектов на сцене, а затем все методы Start()
        private void Awake()
        {
            // Инстанс в игре может быть всего один, но по чьей-либо ошибке экземпляров класса GameManager на сцене может быть много.
            // Нам нужно проредить это множество, оставив один единственный экземпляр и сохранив себе его в статическое свойство.
            
            // Делается это следующим образом: каждый экземпляр класса GameManager обращается к единственному существующему static полю Instance.
            // Так как Instance один на все экземпляры класса, его наполненность данными скажет всем остальным экземплярам класса, что нужно уничтожиться.
            if (Instance == null)
            {
                Instance = this;
                // Особый метод движка, который сообщает следующее:
                // ни при каких условиях не уничтожать этот объект, пока я сам об этом не попрошу!
                // GameObject с таким компонентом всегда будет существовать в игре, независимо от того, что в ней происходит.
                DontDestroyOnLoad(this.gameObject);
                Initialize();
            }
            else
            {
                // Если мы уже нашли себе экземпляр класса GameManager, все прочие экземпляры следует уничтожить.
                Destroy(this.gameObject);
            }
        }

        private void Initialize()
        {
            // ScoreManager - еще один пример класса, не унаследованного от MonoBehaviour. Его не существует в сцене игры, поэтому нужно создать его напрямую.
            // Буквально: сделайте мне НОВЫЙ ScoreManager. Заметьте, у класса есть полноценный конструктор. Об этом мы говорили на уроке #2.
            ScoreManager = new ScoreManager();
        }

        public void StartGame()
        {
            // Если игра уже идет, мы не можем начать новую. Сначала надо завершить старую.
            if (_isGameActive)
            {
                Debug.Log("Game is already active");
                return;
            }
            
            // Создаем или респавним, как получится, нашего персонажа
            var player = CharacterFactory.CreateCharacter(CharacterType.DefaultPlayer);
            // ... ставим его в нулевые координаты
            player.transform.position = Vector3.zero;
            // ... включаем его игровой объект
            player.gameObject.SetActive(true);
            // ... и подписываемся на его смерть, чтобы корректно завершить игру.
            player.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;

            // Сбрасываем все счетчики класса GameManager
            _gameTimeSec = 0f;
            _timeBetweenEnemySpawn = _gameData.TimeBetweenEnemySpawn;
            // Перезапускаем счетчик очков в ScoreManager
            ScoreManager.StartGame();
        
            // После всех проделанных манипуляций, мы переводим наш статус игры в запущенный.
            _isGameActive = true;
        }

        private void Update()
        {
            // Если после if идет внутри фигурных скобок вызов только одной команды, то скобки можно опустить
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

        // Метод OnCharacterDeathHandler мы используем для подписки на событие смерти персонажа.
        // Как видите, в этот метод приходит погибший персонаж. 
        // Это обязательное требование к данному методу, т.к. event, на который мы подписались, выглядит так:
        // public event Action<Character> OnCharacterDeath;
        private void OnCharacterDeathHandler(Character deathCharacter)
        {
            Debug.LogError("character " + deathCharacter.gameObject.name + " is dead");
            // Методом перебора определяем, что надо сделать, в зависимости от того, какой персонаж погиб.
            switch (deathCharacter.CharacterType)
            {
                // Если погиб игрок, то незамедлительно заканчиваем игру
                case CharacterType.DefaultPlayer:
                    GameOver();
                    break;
                // Если погиб враг - плюс к очкам.
                case CharacterType.DefaultEnemy:
                    ScoreManager.AddScore(deathCharacter.CharacterData.ScoreCost);
                    Debug.LogError("Score = " + ScoreManager.GameScore);
                    break;
            }
        
            // возвращаем персонажа в пул объектов
            CharacterFactory.ReturnCharacterToPool(deathCharacter);
            // Отключаем его
            deathCharacter.gameObject.SetActive(false);
        
            // ОЧЕНЬ ВАЖНО. Не забываем отписаться от события смерти. Попробую объяснить на примере по шагам:
            // 1) Вы создали персонажа в первый раз, подписались на событие его смерти. В ивенте есть об этом пометка.
            // 2) Персонаж умирает, вы возвращаете его в пул, подсчитав очки за его гибель, но не отписавшись от события смерти тут.
            // 3) Вы берете этого же персонажа повторно в процессе игры. Подписываетесь на его смерть еще раз. У вас уже две пометки в event.
            // 4) Персонаж умирает, и т.к. в event две пометки о подписке, этот метод будет вызван ДВАЖДЫ!
            // Он вам дважды подсчитает очки, дважды его попробует вернуть в пул итд.
            // За такими процессами очень важно следить
            deathCharacter.LiveComponent.OnCharacterDeath -= OnCharacterDeathHandler;
        }
        
        private void SpawnEnemy()
        {
            // Создаем\переиспользуем экземпляр противника
            var character = CharacterFactory.CreateCharacter(CharacterType.DefaultEnemy);

            // Говорим ему расположиться в пределах нашего игрока. Это выглядит как "возьми позицию игрока со сдвигом"
            float posX = CharacterFactory.Player.transform.position.x + GetOffset();
            float posZ = CharacterFactory.Player.transform.position.z + GetOffset();
            Vector3 spawnPoint = new Vector3(posX, 0, posZ);
            character.transform.position = spawnPoint;
            
            // Подписываемся на его смерть, чтобы считать очки за его убийство и возвращать его в пул объектов
            character.LiveComponent.OnCharacterDeath += OnCharacterDeathHandler;
            // "Включаем" нашего противника. Теперь он в игре
            character.gameObject.SetActive(true);

        
            
            // Еще одна новая конструкция - метод внутри метода.
            // Более корректно это называется "локальный метод другого метода".
            // Метод GetOffset() можно вызвать только внутри метода SpawnEnemy(), и нигде больше!
            // Заметьте, ключевых слов, говорящих о доступе тут нет, т.к. такой метод всегда private.
            // Плюсы: можно разделить сложный для чтения и понимания метод на логические шаги. 
            // Минусы: при работе с любым переборм (for, switch) выделяет на свою работу дополнительную память.
            // Используйте такой прием разумно.
            float GetOffset()
            {
                // Класс Random предоставляет методы для получения псевдо-случайных чисел для создания случайностей в вашей игре
                // в нашем случае, мы будем его использовать для получения случайного сдвига координаты спавна
                // Для этого мы используем метод Range(), который возвращает псевдо-случайное число в пределах между двумя значениями.
                bool isPlus = Random.Range(0, 1) > 0;
                float randomOffset = Random.Range(_gameData.MinEnemySpawnOffset, _gameData.MaxEnemySpawnOffset);
                
                // Метод GetOffset() требует вернуть float - наш шаг сдвига.
                // Ниже мы это и делаем через укороченную запись if
                // полная запись выглядела бы так:
                // if (isPlus)
                //     return randomOffset;
                // else
                //     return -randomOffset;
                return isPlus
                    ? randomOffset
                    : -randomOffset;
            }
        }

        // Игрок погиб, обрабатываем его гибель
        private void GameOver()
        {
            Debug.LogError("GameOver!");
            Debug.LogError("Score = " + ScoreManager.GameScore);
            Debug.LogError("ScoreMax = " + ScoreManager.ScoreMax);
            ScoreManager.CompleteMatch();
            _isGameActive = false;
        }

        // Мы пережили игровое время, успешно завершаем игру.
        private void GameVictory()
        {
            Debug.Log("Game Over! Time's up!");
            ScoreManager.CompleteMatch();
            _isGameActive = false;
        }
    }
}
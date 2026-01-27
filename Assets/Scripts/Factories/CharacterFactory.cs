using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    // Это класс Фабрика. Название как таковое номинальное. Оно просто описывает суть этого класса.
    // Фабрика подразумевает собой класс, который создает схожие объекты. Она позволяет организовать такие действия,
    // как создание схожих объектов, и удобно контролировать происходящее.
    // Это могут быть снаряды, наборы одежды, какие-то абстрактные наборы данных.
    // Например, может быть фабрика событий, на которые должны реагировать ваши NPC. Или фабрика UI элементов, кнопок и рычагов, окно или попапов.
    // В нашем случае, мы создаем фабрику наших персонажей.
    public class CharacterFactory : MonoBehaviour
    {
        // Храним ссылки на ПРЕФАБЫ наших персонажей.
        // Это не самый оптимальный способ хранения больших данных, но самый простой и легко реализуемый.
        [SerializeField] private Character _playerCharacterPrefab;
        [SerializeField] private Character _enemyCharacterPrefab;
        
        // Dictionary (словарь) - это коллекция пар «ключ — значение». Все просто, по ключу мы получаем то, что хранится в значении.
        // В нашем случае, ключом является CharacterType, а значением - queue (очередь) отключенных персонажей.
        // Преимущество "словаря" в том, что ваш компьютер с ним очень быстро взаимодействует.
        // При больших объемах данных - это хороший выигрыш в скорости работы приложения.
        
        // Queue (очередь) - также коллекция. Ее особенность в том, что положить в эту коллекцию можно только в ее конец,
        // а извлечь элемент - только из начала коллекции. Такие коллекции полезны для перебора или для хранения данных, порядок которых не важен.
        
        // Наш итоговый словарь будет хранить персонажей, которые на данный момент в игре не используются. 
        // В дальнейшем, если нам понадобится новый противник, мы сначала проверим его наличие здесь и переиспользуем его.
        // Это позволяет нам экономить мощности нашего устройства, заранее выделяя память на используемых персонажей.
        // Такой прием называется "пулл объектов". Он очень полезен, когда у вас в игре много одинаковых штук: персонажей, пуль, домов и т.д.
        private Dictionary<CharacterType, Queue<Character>> _disabledCharactersPool = new Dictionary<CharacterType, Queue<Character>>();
        
        private List<Character> _activeCharactersPool = new List<Character>(10);
        
        // По задумке нашей игры, персонаж игрока у нас всего один. Мы можем записать его отдельно для быстрого доступа.
        public Character Player { get; private set; }
        public List<Character> ActiveCharacters => _activeCharactersPool;
        
        // Метод для создания или получения персонажа из пула
        // Обратите внимание, вместо ключевого слова void написан наш Character.
        // Это значит, что метод CreateCharacter() обязан вернуть какой-то экземпляр класса Character() туда,
        // где к этому методу обратились, иначе будет ошибка и код работать не будет.
        public Character CreateCharacter(CharacterType characterType)
        {
            Character character = GetFromPool(characterType);

            // Если персонажа нет в пуле, создаем новый
            if (character == null) 
            {
                character = InstantiateCharacter(characterType);
            }
        
            // добавляем полученного персонажа в пулл активных персонажей
            _activeCharactersPool.Add(character);
            
            // Проводим первоначальную или повторную инициализацию персонажа, чтобы он в игре был как новенький.
            character.Initialize();
            
            if (characterType == CharacterType.DefaultPlayer)
                Player = character;
        
            return character;
        }
        
        // Метод для возвращения персонажа обратно в пул.
        // Например, если мы убили противника, и он на данный момент нам больше не нужен.
        // В дальнейшем, если он нам понадобится, мы получим его через метод GetFromPool()
        public void ReturnCharacterToPool(Character character)
        {
            _activeCharactersPool.Remove(character);
            var characterType = character.CharacterType;
            _disabledCharactersPool[characterType].Enqueue(character);
        }

        // Получение персонажа из пула, если в пуле такой персонаж есть
        private Character GetFromPool(CharacterType characterType)
        {
            // Проверяем, есть ли в нашем пулее вообще хоть какие-то персонажи по данному ключу characterType
            if (!_disabledCharactersPool.ContainsKey(characterType))
            {
                // Таких не нашлось. Добавляем запись в пулл, чтобы было куда таких персонажей складировать в будущем.
                _disabledCharactersPool.Add(characterType, new Queue<Character>());
                return null;
            }
        
            // Сама запись в пуле есть. Смотрим в наш Queue, есть ли там хотя бы один не используемый персонаж
            if (_disabledCharactersPool[characterType].Count > 0)
            {
                // Такой нашелся. Выдергиваем его из Queue с помощью метода Dequeue()
                return _disabledCharactersPool[characterType].Dequeue();
            }
            
            // Таких не нашлось. Передаем null, то есть пустоту. Персонажа придется создавать.
            return null;
        }
        
        // Метод для создания нового персонажа
        private Character InstantiateCharacter(CharacterType characterType)
        {
            Character characterObject = null;

            switch (characterType)
            {
                case CharacterType.DefaultPlayer:
                    // Метод Instantiate создает на активной игровой сцене новый объект копию.
                    // Копия получает все параметры и компоненты, какие были у оригинала на момент создания
                    characterObject = GameObject.Instantiate(_playerCharacterPrefab, null);
                    break;
                case CharacterType.DefaultEnemy:
                    characterObject = GameObject.Instantiate(_enemyCharacterPrefab, null);
                    break;
                // Ключевое слово default нужно для того, чтобы если в switch пришло неизвестное, не обработанное значение,
                // мы могли хоть что-то с ним сделать. В нашем случае, мы пишем в консоль, что такой тип нам неизвестен.
                default:
                    Debug.LogError("Unknown character type: " + characterType);
                    break;
            }

            return characterObject;
        }
    }
}
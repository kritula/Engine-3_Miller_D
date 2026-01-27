using UnityEngine;

namespace OmniumLessons
{
    // abstract - ключевое слово, которое говорит нам о том, что мы не можем создать экземпляр данного класса
    // Своего рода, мы говорим, что "этот класс - заготовка для других классов"
    public abstract class Character : MonoBehaviour
    {
        // ключевое слово protected - это что-то между public и private.
        // Оно говорит следующее: переменная недоступна другим классам, за исключением классов-наследников.
        // Для нас это PlayerCharacter и EnemyCharacter, они смогут пользовать данной переменной.
        [SerializeField] protected CharacterData _characterData;
        [SerializeField] protected CharacterType _characterType;
        public CharacterData CharacterData => _characterData;
        public CharacterType CharacterType => _characterType;
        
        // Добавляем ссылку на ближайшую цель нашего персонажа
        public virtual Character CharacterTarget { get; }
        
        // Компоненты наших персонажей
        // В данном случае, в эти компоненты могут сделать запись только базовый класс Character и его наследники,
        // но получить их для дальнейшего взаимодействия с ними - любой класс извне, т.к. get является public, а set - protected
        public IMovable MovableComponent { get; protected set; }
        public ILiveComponent LiveComponent { get; protected set; }
        public IAttackComponent AttackComponent { get; protected set; }
        
        // ключевое слово virtual говорит нам о том, что данный метод Start() можно переопределить.
        // Т.е. класс-наследник может дополнить его, изменить или полностью переписать выполняемые им задачи.
        
        // Здесь произошел рефактор на 3 уроке. Ввиду того, что нам нужно контролировать начальную и повторную инициализацию
        // персонажа, мы заменили метод Start() на метод Initialize(), который вызываем в нужный нам в игре момент.
        // Соответственно, override методы также были переименованы из Start() в Initialize() в классах-наследниках.
        public virtual void Initialize()
        {
            // Создаем новый экземпляр (копию) класса 
            MovableComponent = new CharacterMovementComponent();
            // Производим инициализацию, передаем нужные ему для работы данные
            MovableComponent.Initialize(_characterData);
        }
        
        // Абстрактные методы, аналогично методам из интерфейса, требуют своей реализации в классе-наследнике.
        // Это нужно по той же причине - обозначить, что класс должен уметь делать независимо от обстоятельств.
        public abstract void Update();
    }
}
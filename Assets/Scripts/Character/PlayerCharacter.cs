using UnityEngine;

namespace OmniumLessons
{
    // Наследование — это передача свойств и поведения от родителя к потомку
    // Мы говорим следующее: "Класс PlayerCharacter умеет и знает то же самое (или почти всё), что и класс Character"
    public class PlayerCharacter : Character
    {
        // игрок будет определять ближайшего противника как ближайшую цель для взаимодействия: атаки, поворота и т.д.
        public override Character CharacterTarget
        {
            get
            {
                Character target = null;
                float nearest = float.MaxValue;
                // Чтобы найти ближайшую к игроку цель, нам нужно пробежаться по всем активным на данный момент персонажам.
                var activePool = GameManager.Instance.CharacterFactory.ActiveCharacters;
                // ключевое слово foreach - по сути, сокращенная форма слова for, которое вы видели на видеоуроке в этом же месте по коду.
                // foreach также позволяет пройтись по всем элементам списка, но без дополнительных условий. 
                // Данный "foreach (var activeCharacter in activePool)" можно легко переписать в виде "for(int i = 0; i < activePool.Length; i++)"
                // и тогда вы будете работать не с activeCharacter, а с activePool[i]. В остальном будет все также. Попробуйте.
                // В нашем случае, мы пройдемся по всем активным персонажам.
                foreach (var activeCharacter in activePool)
                {
                    // нам не нужно проверять самих себя
                    if (activeCharacter.CharacterType == CharacterType.DefaultPlayer)
                        continue;
                
                    // На всякий случай проверяем, что персонаж живой.
                    // Статус активный и статус живой - это два разных понятия.
                    // Например, в дальнейшем могут появиться лежащие модельки погибших врагов.
                    // Они уже не живые, но все еще не возвращены в пул неактивных персонажей, т.е. они активны по мнению CharacterFactory.
                    // Зачем нам, игрокам, обращать внимание на таких, правда?
                    if (!activeCharacter.LiveComponent.IsAlive)
                        continue;
                
                    // Мы можем проверить дистанцию благодаря Vector3.Distance.
                    // Этот способ полезен, когда нам также важно знать дистанцию между двумя точками.
                    // Однако, если вам важно только найти ближайшего или самого дальнего, то лучше обойтись формулой квадрата расстояния.
                    // Почему? Vector3.Distance предполагает операцию извлечения квадратного корня числа для определения точной дистанции. 
                    // С точки зрения компьютерной математики, это очень тяжелая задача. 
                    // Это будет выглядеть так:
                    // float distance = (activeCharacter.transform.position - transform.position).sqrMagnitude < nearest;
                    float distance = Vector3.Distance(activeCharacter.transform.position, transform.position);
                    if (distance < nearest)
                    {
                        nearest = distance;
                        target = activeCharacter;
                    }
                }

                return target;
            }
        }
        
        // Говорим, что мы хотим изменить базовый класс Start()
        
        // override метод Start() переименован в Initialize() для нашей базовой архитектуры, заложенной в уроке #3
        public override void Initialize()
        {
            // на всякий случай проверяем, есть ли в _characterData ссылка на нужные нам данные
            if (_characterData == null)
            {
                // Благодаря компонентной структуре движка, мы можем получать данные в виде других компонентов с самого объекта,
                // с родительских или дочерних объектов.
                // Это не является хорошей практикой на постоянной основе. Настоятельно рекомендую НЕ ИСПОЛЬЗОВАТЬ
                // такой подход в местах, где данные часто обновляются. Например, в методе Update()
                _characterData = GetComponent<CharacterData>();
            }

            // Вызов кода из оригинального метода Start() из класса Character через ключевое слово base.
            base.Initialize();
            
            // для игрока мы создаем PlayerLiveComponent, с особым поведением для игрока
            // С уроком #3 передаем в класс this, то есть ЭТОТ PLayerCharacter
            LiveComponent = new PlayerLiveComponent(this);
            
            AttackComponent = new CharacterAttackComponent();
            AttackComponent.Initialize(_characterData);
        }
        
        // abstract метод или метод, объявленный в интерфейсе, всегда нужно переписывать
        // В обоих случаях использование base невозможно - реализации в родительском классе не существует.
        public override void Update()
        {
            // Мы не можем двигать персонажа, если мертвы
            if (!LiveComponent.IsAlive)
                return;
            
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            
            Vector3 movementVector = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            // Вызываем наши методы ходьбы и поворота персонажа
            MovableComponent.Move(movementVector);
            MovableComponent.Rotation(movementVector);
            
            // На уроке №3 меняем направление вращения персонажа
            if (CharacterTarget == null)
            {
                // если нет цели, вращаемся как раньше
                MovableComponent.Rotation(movementVector);
            }
            else
            {
                // Цель есть - поворачиваемся в сторону цели
                Vector3 directionToTarget = CharacterTarget.transform.position - transform.position;
                directionToTarget.Normalize();
                MovableComponent.Rotation(directionToTarget);
        
                // Если нажимаем на кнопку пробела, которая записана в InputManager как "Jump", то пытаемся атаковать цель.
                if (Input.GetButtonDown("Jump"))
                    AttackComponent.MakeDamage(CharacterTarget);
            }
            
            AttackComponent.OnUpdate();
        }
    }
}
using UnityEngine;

namespace OmniumLessons
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private AiState _aiState;
        
        // Цель наших противников всегда только мы, игрок. На уроке #3 мы убираем локальную переменную как ссылку на цель,
        // и назначаем целью игрока из нашей фабрики персонажей
        public override Character CharacterTarget => GameManager.Instance.CharacterFactory.Player;

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
            
            // для врагов мы создаем EnemyLiveComponent, с особым поведением для противника
            // С уроком #3 передаем в класс this, то есть ЭТОТ EnemyCharacter
            LiveComponent = new EnemyLiveComponent(this);
            
            AttackComponent = new CharacterAttackComponent();
            AttackComponent.Initialize(_characterData);
        }
        
        public override void Update()
        {
            // Выполняем код в зависимости от состояния нашего простого ИИ
            // Наравне с if, один из способов проверки данных на определенные значения
            switch (_aiState)
            {
                // если _aiState равно (==) AiState.None, то мы ничего не делаем
                case AiState.None:
                    return;
                // если _aiState == AiState.MoveToTarget, то мы движемся к цели (к игроку)
                case AiState.MoveToTarget:
                    Move();
                    return;
                // если _aiState == AiState.Attack, то мы бьем цель (к игроку)
                case AiState.Attack:
                    Attack();
                    return;
            }

            AttackComponent.OnUpdate();
        }

        private void Move()
        {
            Vector3 direction = CharacterTarget.transform.position - _characterData.CharacterTransform.transform.position;
            direction.Normalize();
            
            MovableComponent.Move(direction);
            MovableComponent.Rotation(direction);
            
            // меняем состояние на атаку, если приблизились
            if (Vector3.Distance(CharacterTarget.transform.position,
                    _characterData.CharacterController.transform.position) <= 3)
            {
                _aiState = AiState.Attack;
                return;
            }
        }

        private void Attack()
        {
            AttackComponent.MakeDamage(CharacterTarget);
            
            // меняем состояние на движение, если игрок отошел
            if (Vector3.Distance(CharacterTarget.transform.position,
                    _characterData.CharacterController.transform.position) > 3)
            {
                _aiState = AiState.MoveToTarget;
                return;
            }
        }
    }
}
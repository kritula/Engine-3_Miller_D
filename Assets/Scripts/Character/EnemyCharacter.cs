using UnityEngine;

namespace OmniumLessons
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private AiState _aiState;
        
        public override Character CharacterTarget => GameManager.Instance.CharacterFactory.Player;

        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }
            
            base.Initialize();
            
            LiveComponent = new EnemyLiveComponent(this);
            
            AttackComponent = new CharacterAttackComponent();
            AttackComponent.Initialize(_characterData);
        }
        
        public override void Update()
        {
            switch (_aiState)
            {
                case AiState.None:
                    return;
                case AiState.MoveToTarget:
                    Move();
                    return;
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
            
            if (Vector3.Distance(CharacterTarget.transform.position,
                    _characterData.CharacterController.transform.position) > 3)
            {
                _aiState = AiState.MoveToTarget;
                return;
            }
        }
    }
}
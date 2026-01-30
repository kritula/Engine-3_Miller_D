using UnityEngine;

namespace OmniumLessons
{
    public class PlayerCharacter : Character
    {
        // игрок будет определять ближайшего противника как ближайшую цель для взаимодействия: атаки, поворота и т.д.
        public override Character CharacterTarget
        {
            get
            {
                Character target = null;
                float nearest = float.MaxValue;
                var activePool = GameManager.Instance.CharacterFactory.ActiveCharacters;
                foreach (var activeCharacter in activePool)
                {
                    if (activeCharacter.CharacterType == CharacterType.DefaultPlayer)
                        continue;
                
                    if (!activeCharacter.LiveComponent.IsAlive)
                        continue;
                
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
        
        public override void Initialize()
        {
            if (_characterData == null)
            {
                _characterData = GetComponent<CharacterData>();
            }

            base.Initialize();
            
            LiveComponent = new PlayerLiveComponent(this);
            
            AttackComponent = new CharacterAttackComponent();
            AttackComponent.Initialize(_characterData);
        }
        
        public override void Update()
        {
            if (!LiveComponent.IsAlive)
                return;
            
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            
            Vector3 movementVector = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            MovableComponent.Move(movementVector);
            MovableComponent.Rotation(movementVector);
            
            if (CharacterTarget == null)
            {
                MovableComponent.Rotation(movementVector);
            }
            else
            {
                Vector3 directionToTarget = CharacterTarget.transform.position - transform.position;
                directionToTarget.Normalize();
                MovableComponent.Rotation(directionToTarget);
        
                if (Input.GetButtonDown("Jump"))
                    AttackComponent.MakeDamage(CharacterTarget);
            }
            
            AttackComponent.OnUpdate();
        }
    }
}
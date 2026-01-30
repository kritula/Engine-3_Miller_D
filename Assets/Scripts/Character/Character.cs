using UnityEngine;

namespace OmniumLessons
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected CharacterData _characterData;
        [SerializeField] protected CharacterType _characterType;
        public CharacterData CharacterData => _characterData;
        public CharacterType CharacterType => _characterType;
        
        public virtual Character CharacterTarget { get; }
        
        public IMovable MovableComponent { get; protected set; }
        public ILiveComponent LiveComponent { get; protected set; }
        public IAttackComponent AttackComponent { get; protected set; }
        
        public virtual void Initialize()
        {
            MovableComponent = new CharacterMovementComponent();
            MovableComponent.Initialize(_characterData);
        }
        
        public abstract void Update();
    }
}
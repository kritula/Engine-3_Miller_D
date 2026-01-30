using UnityEngine;

namespace OmniumLessons
{
    public class CharacterAttackComponent : IAttackComponent
    {
        private readonly int _lockDamageTimeMax = 1;
        private float _lockDamageTime = 0;

        private CharacterData _characterData;
        
        public int Damage => 5;


        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;
        }
        
        public void MakeDamage(Character damageTarget) 
        {
            if (_lockDamageTime > 0)
                return;
            
            if (damageTarget == null || damageTarget.LiveComponent == null)
                return;

            if (!damageTarget.LiveComponent.IsAlive)
                return;
            
            damageTarget.LiveComponent.GetDamage(Damage);

            _lockDamageTime = _lockDamageTimeMax;
        }

        public void OnUpdate()
        {
            if (_lockDamageTime > 0)
                _lockDamageTime -= Time.deltaTime;
        }
    }
}
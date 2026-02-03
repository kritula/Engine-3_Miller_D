using System;

namespace OmniumLessons
{
    public class EnemyLiveComponent : ILiveComponent
    {
        public event Action<Character>  OnCharacterDeath;
        public event Action<Character> OnCharacterHealthChange;

        private Character _characterOwner;

        private float _health;

        
        public bool IsAlive => Health > 0;
        public int MaxHealth => 10;
        public float Health
        {
            get => _health;
            private set
            {
                _health = value;
                if (_health > MaxHealth)
                    _health = MaxHealth;
                if (_health <= 0)
                {
                    _health = 0;
                    SetDeath();
                }
            }
        } 
        public EnemyLiveComponent(Character characterOwner)
        {
            _characterOwner = characterOwner;
            _health = MaxHealth;
        }
        
        public void GetDamage(int damage)
        {
            Health -= damage * 1000;
            OnCharacterHealthChange?.Invoke(_characterOwner);
        }

        private void SetDeath()
        {
            OnCharacterDeath?.Invoke(_characterOwner);
        }
    }
}
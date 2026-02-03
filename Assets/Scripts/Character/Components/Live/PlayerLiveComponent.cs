using System;
using UnityEngine;

namespace OmniumLessons
{
    public class PlayerLiveComponent : ILiveComponent
    {
        public event Action<Character>  OnCharacterDeath;
        public event Action<Character> OnCharacterHealthChange;

        private Character _characterOwner;
        private float _health;

        public bool IsAlive => Health > 0;
        public int MaxHealth => 50;

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

        public PlayerLiveComponent(Character characterOwner)
        {
            _characterOwner = characterOwner;
            _health = MaxHealth;
        }
            

        public void GetDamage(int damage)
        {
            Health -= damage;
            OnCharacterHealthChange?.Invoke(_characterOwner);
            Debug.Log($"I get damage by {damage}. My health is {Health}/{MaxHealth} now!");
        }

        private void SetDeath()
        {
            OnCharacterDeath?.Invoke(_characterOwner);
        }
    }
}
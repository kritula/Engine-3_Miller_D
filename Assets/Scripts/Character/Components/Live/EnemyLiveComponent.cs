using System;
using System.Collections;
using System.Collections.Generic;
using OmniumLessons;
using UnityEngine;

namespace OmniumLessons
{
    public class EnemyLiveComponent : ILiveComponent
    {
        public event Action<Character>  OnCharacterDeath;

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
                // В свойствах можно выполнять дополнительные расчеты...
                if (_health > MaxHealth)
                    _health = MaxHealth;
                if (_health <= 0)
                {
                    _health = 0;
                    // ... или даже вызывать методы и обращаться к другим свойствам
                    OnCharacterDeath?.Invoke(_characterOwner);
                }
                // Альтернатива: Mathf.Clamp(_health, 0, MaxHealth);
            }
        } 
        
        // Такой метод без определяющего возвращающего слова void и с именем класса является "конструктором"
        // Конструктор - это инициализирующий метод. Он нужен для правильного формирования класса перед началом его работы.
        // Он вызывается в момент создания экземпляра класса, в нашем случае, когда мы делаем: new PlayerLiveComponent().
        // Важный момент, классы, унаследованные от MonoBehaviour не имеют конструктора, т.к. они инициализируются системой Unity
        // Обновление с уроком №3: в конструктор передаем ссылку на владельца компонента, чтобы корректно вызывать триггер смерти.
        public EnemyLiveComponent(Character characterOwner)
        {
            _characterOwner = characterOwner;
            _health = MaxHealth;
        }
        
        public void GetDamage(int damage)
        {
            // Пусть враги в нашем примере получают тысячекратный урон в отличие от игрока.
            Health -= damage * 1000;
        }
    }
}
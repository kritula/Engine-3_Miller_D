using UnityEngine;

namespace OmniumLessons
{
    public class CharacterAttackComponent : IAttackComponent
    {
        // Ключевое слово readonly говорит само за себя - запрет на дальнейшие изменения, только чтение.
        // В такую переменную невозможно что-либо записать, но всегда можно получить информацию.
        private readonly int _lockDamageTimeMax = 1;
        private float _lockDamageTime = 0;

        private CharacterData _characterData;
        
        public int Damage => 5;


        public void Initialize(CharacterData characterData)
        {
            _characterData = characterData;
        }
        
        public void MakeDamage(Character damageTarget) {
            if (_lockDamageTime > 0)
                return;
            
            // Мы не можем бить уже мертвую цель или цель, у которой нет фактического здоровья
            if (damageTarget == null || damageTarget.LiveComponent == null)
                return;

            // Мы не можем бить уже мертвую цель
            // Восклицательный знак (!) выполняет функцию логического НЕ.
            // В даннои примере получается как "цель НЕ живая"
            // Альтернативное написание было на уроке: if (damageTarget.LiveComponent.IsAlive == false) return;
            if (!damageTarget.LiveComponent.IsAlive)
                return;
            
            // наносим урон по цели
            damageTarget.LiveComponent.GetDamage(Damage);

            // сбрасываем наш счетчик времени
            _lockDamageTime = _lockDamageTimeMax;
        }

        public void OnUpdate()
        {
            // Чтобы не наносить урон каждый кадр, мы ограничим возможность нанесения урона до одной атаки в секунду
            // Как писал я об этом в коммите для урока №2, это лучше делать в апдейте
            if (_lockDamageTime > 0)
                _lockDamageTime -= Time.deltaTime;
        }
    }
}
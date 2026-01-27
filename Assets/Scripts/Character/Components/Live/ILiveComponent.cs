using System;

namespace OmniumLessons
{
	public interface ILiveComponent
	{
		// event - это оповещение: объект «кричит», если персонаж умер.
		// Action — список тех, кто услышит этот крик и выполнит свою работу 
		// Свойство к уроку №3 мы изменяем. Теперь оно отправляет нам ссылку на погибшего персонажа. 
		// Подписавшиеся на него методы обязаны иметь аргумент Character в круглых скобках, т.е. они должны с ним работать.
		public event Action<Character> OnCharacterDeath;
		
		public bool IsAlive { get; }
		public int MaxHealth { get; }
		public float Health { get; }
		
		public void GetDamage(int damage);
	}
}
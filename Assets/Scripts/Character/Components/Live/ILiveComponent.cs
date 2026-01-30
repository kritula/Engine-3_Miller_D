using System;

namespace OmniumLessons
{
	public interface ILiveComponent
	{
		public event Action<Character> OnCharacterDeath;
		
		public bool IsAlive { get; }
		public int MaxHealth { get; }
		public float Health { get; }
		
		public void GetDamage(int damage);
	}
}
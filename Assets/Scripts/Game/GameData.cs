using UnityEngine;

namespace OmniumLessons
{
	[CreateAssetMenu(fileName = "GameData", menuName = "ZombieIO/GameData")]
	public class GameData : ScriptableObject
	{
		[SerializeField] private float gameTimeMinutesMax = 15;

		[Space(10), Header("Experience progress")]
		[SerializeField]
		private int baseExperience = 20;
		[SerializeField]
		private int grownRate = 10;
        
		[Space(10), Header("SpawnLogic")]
		[SerializeField]
		private float timeBetweenEnemySpawn = 2;
		[SerializeField]
		private float minEnemySpawnOffset = 5;
		[SerializeField]
		private float maxEnemySpawnOffset = 15;
        
        
		public float GameTimeMinutesMax => 
			gameTimeMinutesMax;
        
		public int BaseExperience => 
			baseExperience;
        
		public int GrownRate => 
			grownRate;
        
		public float GameTimeSecondsMax => 
			gameTimeMinutesMax * 60;
        
		public float TimeBetweenEnemySpawn => 
			timeBetweenEnemySpawn;
        
		public float MinEnemySpawnOffset =>
			minEnemySpawnOffset;
        
		public float MaxEnemySpawnOffset =>
			maxEnemySpawnOffset;
	}
}
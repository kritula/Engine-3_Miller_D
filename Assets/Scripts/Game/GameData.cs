using UnityEngine;

namespace OmniumLessons
{
	// Классы ScriptableObject в Unity позволяют создавать asset объекты для хранения данных вашей игры.
	// Это могут быть базовые настройки, ссылки на префабы, различные ключи, спрайты итд.
	// Главное правило, НЕЛЬЗЯ использовать ScriptableObject в качестве системы сохранения.
	// Они разрабатывались только для удобства получения неизменяемых данных.
	
	// Наш класс GameData, унаследованный от ScriptableObject, будет хранить базовые настройки всей игры.
	
	// Атрибут CreateAssetMenu делает то, что прописано в названии: выводит пункт в меню, для быстрого создания новых GameData
	// В данном случае, правой кнопкой мыши в окне Unity Project => Create => ZombieIO => GameData.
	// Будет создан файл с именем GameData.
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
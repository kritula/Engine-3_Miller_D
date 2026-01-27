using System;
using UnityEngine;

namespace OmniumLessons
{
	// Класс ScoreManager будет ответственным за подсчет наших игровых очков, а также за их загрузку и хранение.
	// В полноценных проектах, такой класс отвечал бы только за работу с очками, а всё, что касается сохранения 
	// делала бы другая система. Но для примера мы не будем распыляться, а сделаем всё в едином классе.
	public class ScoreManager
	{
		// Ключевое слово const позволяет создать переменную-константу. Эта переменная не подвержена никаким изменениям в коде.
		// Также, как и static классы и функции, они принадлежат классу, а не его экземпляру.
		// Такие константы полезно использовать для быстрого доступа к четко заданным неизменным значениям.
		// Например, число Pi = 3.14. В нашем случае, константы хранят ключи, по которым мы будем хранить наши сохранения.
		private const string SESSION_SCORE_MAX = "save_score_max";
		private const string CURRENT_SCORE = "save_current_score";

		// С ивентами мы знакомились на прошлом уроке, создавая ивент OnDeath в компоненте здоровья персонажа.
		public event Action<int> OnScoreUpdated;
		public event Action<int> OnSessionScoreUpdated;
		public event Action<int> OnScoreChanged;
    
    
		private int _gameScore;
		private int _globalGameScore;
		private int _scoreMax;

		public int GameScore => _gameScore;
		public int GlobalGameScore
		{
			get => _globalGameScore;
			set
			{
				_globalGameScore = value;
				if (_globalGameScore < 0)
					_globalGameScore = 0;
            
				PlayerPrefs.SetInt(CURRENT_SCORE, _globalGameScore);
			}
		}
		public int ScoreMax => _scoreMax;
		public bool IsNewScoreRecord { get; private set; }


		public ScoreManager()
		{
			// Для загрузки и сохранения данных в данном проекте мы будем использовать Unity класс PlayerPrefs.
			// PlayerPrefs предоставляет простой способ хранения данных вашей игры.
			// Оно полезно в тех случаях, когда ваша игра небольшая, и Вам нет нужды создавать сложную систему сохранения.
			// Также оно без проблем работает на всех платформах, с которыми умеет взаимодействовать Unity, будь то ПК, браузер или Android телефон.
			// В более сложных играх разработчики создают отдельные сервисы сохранения, которые записывают данные в различные созданные файлы.
			// Например, в файлы формата XML или JSON. Вы могли такое наблюдать, если игрались с сохранениями ваших любимых игр на компьютере.
			_gameScore = 0;
			_scoreMax = PlayerPrefs.GetInt(SESSION_SCORE_MAX, 0);
			_globalGameScore = PlayerPrefs.GetInt(CURRENT_SCORE, 0);
			IsNewScoreRecord = false;
		}

		// Метод StartGame() будет вызываться каждый раз, когда мы стартуем новую игру
		public void StartGame()
		{
			_gameScore = 0;
			IsNewScoreRecord = false;
		}
    
		// Вызывается, когда мы убили противника, чтобы подсчитать очки.
		public void AddScore(int scoreCost)
		{
			_gameScore += scoreCost;
			OnScoreChanged?.Invoke(_gameScore);
        
			// Если сумма очков за матч превысила предыдущий рекорд - это новый рекорд.
			if (_gameScore <= _scoreMax)
				return;

			_scoreMax = GameScore;
			PlayerPrefs.SetInt(SESSION_SCORE_MAX, _scoreMax);
			IsNewScoreRecord = true;
		}
    
		public void CompleteMatch()
		{
			GlobalGameScore += _gameScore;
		}
	}
}
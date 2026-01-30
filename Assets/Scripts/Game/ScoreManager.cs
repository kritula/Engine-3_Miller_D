using System;
using UnityEngine;

namespace OmniumLessons
{
	public class ScoreManager
	{
		private const string SESSION_SCORE_MAX = "save_score_max";
		private const string CURRENT_SCORE = "save_current_score";

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
			_gameScore = 0;
			_scoreMax = PlayerPrefs.GetInt(SESSION_SCORE_MAX, 0);
			_globalGameScore = PlayerPrefs.GetInt(CURRENT_SCORE, 0);
			IsNewScoreRecord = false;
		}

		public void StartGame()
		{
			_gameScore = 0;
			IsNewScoreRecord = false;
		}
    
		public void AddScore(int scoreCost)
		{
			_gameScore += scoreCost;
			OnScoreChanged?.Invoke(_gameScore);
        
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
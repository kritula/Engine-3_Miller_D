using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class GameplayWindow : Window
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;

        [Space][SerializeField] private Slider experienceSlider;

        [Space][SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Button pauseButton;

        protected override void OpenStart()
        {
            base.OpenStart();
            var player = GameManager.Instance.CharacterFactory.Player;

            UpdateHealthVisual(player);
            player.LiveComponent.OnCharacterHealthChange += UpdateHealthVisual;

            ScoreChangeHandler(GameManager.Instance.ScoreManager.GameScore);
            GameManager.Instance.ScoreManager.OnScoreChanged += ScoreChangeHandler;

            pauseButton.onClick.AddListener(OnPauseClicked);

            UpdateTimer();
        }

        protected override void CloseStart()
        {
            base.CloseStart();

            GameManager.Instance.ScoreManager.OnScoreChanged -= ScoreChangeHandler;

            var player = GameManager.Instance.CharacterFactory.Player;
            if (player == null)
                return;

            player.LiveComponent.OnCharacterHealthChange -= UpdateHealthVisual;

            pauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        private void UpdateHealthVisual(Character character)
        {
            int health = (int)character.LiveComponent.Health;
            int healthMax = character.LiveComponent.MaxHealth;

            healthText.text = health + "/" + healthMax;
            healthSlider.maxValue = healthMax;
            healthSlider.value = health;
        }

        private void ScoreChangeHandler(int score)
        {
            scoreText.text = score.ToString();
        }

        private void UpdateTimer()
        {
            var min = (int)(GameManager.Instance.GameTime / 60);
            var sec = (int)(GameManager.Instance.GameTime % 60);
            timerText.text = GetTime(min) + ":" + GetTime(sec);


            string GetTime(int value)
            {
                return (value < 10) ? "0" + value : value.ToString();
            }
        }

        private void UpdateExperience(int value, int maxValue)
        {
            experienceSlider.maxValue = maxValue;
            experienceSlider.value = value;
        }

        private void OnPauseClicked()
        {
            GameManager.Instance.IsGamePaused = true;
            Time.timeScale = 0f;
            GameManager.Instance.WindowsService.ShowWindow<PauseMenuWindow>(false);
        }

        private void Update()
        {
            UpdateTimer();
        }
    }
}
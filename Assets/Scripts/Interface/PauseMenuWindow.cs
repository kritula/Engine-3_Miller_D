using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class PauseMenuWindow : Window
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsMenuButton;
        [SerializeField] private Button skillsButton;
        [SerializeField] private Button exitMainMenuButton;



        public override void Initialize()
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            optionsMenuButton.onClick.AddListener(OnOptionsClicked);
            skillsButton.onClick.AddListener(OnSkillsClicked);
            exitMainMenuButton.onClick.AddListener(OnExitToMainMenuClicked);
        }

        private void OnContinueClicked()
        {
            GameManager.Instance.IsGamePaused = false;
            Time.timeScale = 1f;
            Hide(true);
        }

        private void OnOptionsClicked()
        {
            // пока пусто (позже откроем OptionsWindow)
        }

        private void OnSkillsClicked()
        {
            GameManager.Instance.WindowsService.ShowWindow<SkillsWindow>(true);
        }

        private void OnExitToMainMenuClicked()
        {
            Time.timeScale = 1f;
            GameManager.Instance.IsGamePaused = true;

            Hide(true);
            GameManager.Instance.WindowsService.HideWindow<GameplayWindow>(true);
            GameManager.Instance.WindowsService.ShowWindow<MainMenuWindow>(false);
        }
    }
}

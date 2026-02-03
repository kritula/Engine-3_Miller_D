using UnityEngine;
using UnityEngine.UI;

namespace OmniumLessons
{
    public class SkillsWindow : Window
    {
        [SerializeField] private Button backButton;

        public override void Initialize()
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            Hide(true);
            GameManager.Instance.WindowsService.ShowWindow<PauseMenuWindow>(true);
        }
    }
}



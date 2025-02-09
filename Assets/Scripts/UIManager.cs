using System.Collections;
using MyGame;
using UnityEngine;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameOverPanel _gameOverPanel;
        [SerializeField] private LevelCompletePanel _levelCompletePanel;
        [SerializeField] private MenuPanel _menuPanel;
        [SerializeField] private GamePlayPanel _gamePlayPanel;


        public GameOverPanel GameOverPanel => _gameOverPanel;
        public MenuPanel MenuPanel => _menuPanel;
        public GamePlayPanel GamePlayPanel => _gamePlayPanel;
        public static UIManager Instance { get; private set; }
        

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            LevelManager.LevelStarted += LevelManagerOnLevelStarted;
            LevelManager.LevelResulted += LevelManagerOnLevelResulted;
        }


        private void OnDisable()
        {
            LevelManager.LevelStarted -= LevelManagerOnLevelStarted;
            LevelManager.LevelResulted -= LevelManagerOnLevelResulted;
        }

        private void LevelManagerOnLevelResulted(bool success)
        {
            StartCoroutine(GameOver(success));
        }

        IEnumerator GameOver(bool success)
        {
            if (!success)
            {
                yield return new WaitForSeconds(0.1f);
                _gamePlayPanel.Hide();
                _gameOverPanel.Show();
            }
            else
            {
                _levelCompletePanel.Show();
                yield return new WaitForSeconds(2);
                GameManager.LoadGame(new LoadGameData
                {
                    Score = LevelManager.Instance.Score,
                    GameType = GameType.Continue
                });
            }
        }

        private void LevelManagerOnLevelStarted()
        {
            GamePlayPanel.Show();
        }
    }


}


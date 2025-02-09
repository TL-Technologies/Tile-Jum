using System;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MenuPanel : ShowHidable
    {
        [SerializeField] private Text _bestScoreTxt;
        [SerializeField] private string _bestScorePrefix;
        [SerializeField] private LevelProgressUI _lvlProgressUI;
        [SerializeField] private Text _lvlCompletedTxt;

        protected override void OnEnable()
        {
            base.OnEnable();
            _bestScoreTxt.text = $"{_bestScorePrefix}{MyGame.GameManager.BEST_SCORE}";
            
        }

        private void Start()
        {
            _lvlProgressUI.Level = LevelManager.Instance.Level;
            _lvlCompletedTxt.gameObject.SetActive(LevelManager.Instance.GameType == GameType.Continue);
            if (_lvlCompletedTxt.gameObject.activeSelf)
            {
                _lvlCompletedTxt.text = $"Level {LevelManager.CompletedLevel} Completed";
            }
        }

        public void OnClickPlay()
        {
            Hide();
            LevelManager.Instance.StartTheGame();
        }
        
    }
}
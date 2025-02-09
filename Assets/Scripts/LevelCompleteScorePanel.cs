using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelCompleteScorePanel:ShowHidable
    {
        [SerializeField] private Text _scoreTxt, _bestTxt;
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                _scoreTxt.text = value.ToString();
                _score = value;
            }
        }

        public override void Show(bool animate = true, Action completed = null)
        {
            Score = MyGame.GameManager.LoadGameData.Score;
            _bestTxt.text = $"BEST - {MyGame.GameManager.BEST_SCORE}";
            base.Show(animate, completed);
        }
    }
}
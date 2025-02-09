using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelCompletePanel : ShowHidable
    {
        [SerializeField] private Text _levelText;

        public override void Show(bool animate = true, Action completed = null)
        {
            _levelText.text = $"LEVEL {LevelManager.Instance.Level}";
            base.Show(animate, completed);
        }
    }
}
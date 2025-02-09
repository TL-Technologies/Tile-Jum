using System;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameOverPanel : ShowHidable
    {
//        [SerializeField] private TimeOutProgressBar _continueProgressBar;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Text _scoreTxt;
        [SerializeField] private Text _completedPercentageTxt;
        [SerializeField] private LevelProgressUI _levelProgressUI;

        public void OnClickRestart()
        {
            MyGame.GameManager.LoadGame(new LoadGameData() , false);
        }

        public override void Show(bool animate = true, Action completed = null)
        {
            ShowRestart();
            _levelProgressUI.Level = LevelManager.Instance.Level;
            _levelProgressUI.Progress = LevelManager.Instance.Progress;
            _scoreTxt.text = LevelManager.Instance.Score.ToString();
            _completedPercentageTxt.text = $"{Mathf.FloorToInt(LevelManager.Instance.Progress*100)}% Completed";

            if (LevelManager.Instance.BestScoreArchived && RatingPopUp.Available)
            {
                Invoke(nameof(ShowRating), 1);
            }
            AdsManager.ShowOrPassAdsIfCan();
            base.Show(animate, completed);
        }

        protected override void OnShowCompleted()
        {
//            var ableToContinue = LevelManager.Instance.AbleToContinue;
//            if (ableToContinue)
//            {
//                _continueProgressBar.gameObject.SetActive(true);
//                Invoke(nameof(ShowRestart),1);
//            }
//            else
//            {
//                _continueProgressBar.gameObject.SetActive(false);
//                ShowRestart();
//            }

          
            base.OnShowCompleted();
        }

        private void ShowRating()
        {
            SharedUIManager.RatingPopUp?.Show();
        }

        private void ShowRestart()
        {
            _restartBtn.gameObject.SetActive(true);
        }
    }
}

public enum Tag
{
}

public enum Layer
{
}

public static class Extensions
{
    public static int GetMask(this Layer layer) =>
        LayerMask.NameToLayer(layer.ToString());

    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.GetRandom(out var index);
    }

    public static T GetRandom<T>(this IEnumerable<T> enumerable, out int index)
    {
        var list = enumerable.ToList();
        index = Random.Range(0, list.Count);
        return list[index];
    }

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();

        while (list.Count > 0)
        {
            yield return list.GetRandom(out var index);
            list.RemoveAt(index);
        }
    }

    public static Vector2 GetSizeInScreenSpace(this RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Vector2(Mathf.Abs((corners[1] - corners[2]).magnitude),
            Mathf.Abs((corners[1] - corners[0]).magnitude));
    }
}
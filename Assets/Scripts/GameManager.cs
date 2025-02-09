using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MyGame
{

    public partial class GameManager : Singleton<GameManager>
    {
        public static event Action<int> TopScoreChanged;

        public static LoadGameData LoadGameData { get; private set; }

        public static int BEST_SCORE
        {
            get => PlayerPrefs.GetInt(nameof(BEST_SCORE));
            set
            {
                if (BEST_SCORE >= value)
                    return;
                PlayerPrefs.SetInt(nameof(BEST_SCORE), value);
                TopScoreChanged?.Invoke(value);
            }
        }

        public static int TOTAL_GAME_COUNT
        {
            get => PrefManager.GetInt(nameof(TOTAL_GAME_COUNT));
            set => PrefManager.SetInt(nameof(TOTAL_GAME_COUNT),value);
        }

        protected override void OnInit()
        {
            base.OnInit();
            Application.targetFrameRate = 60;

        }
    }

    public partial class GameManager
    {
        // ReSharper disable once FlagArgument
        public static void LoadScene(string sceneName, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            var loadingPanel = SharedUIManager.LoadingPanel;
            if (showLoading && loadingPanel != null)
            {
                loadingPanel.Speed = loadingScreenSpeed;
                loadingPanel.Show(completed: () =>
                {
                    SceneManager.LoadScene(sceneName);
                    loadingPanel.Hide();
                });
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }


        public static void LoadGame(LoadGameData data, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            LoadGameData = data;
            LoadScene("Main", showLoading, loadingScreenSpeed);
        }
    }



#if DAILY_REWARD

    public partial class GameManager
    {

        private static DateTime LastRewardTime
        {
            get
            {
                var time = long.Parse(PlayerPrefs.GetString(nameof(LastRewardTime), "0"));
                return new DateTime(time);
            }
            set => PlayerPrefs.SetString(nameof(LastRewardTime), value.Ticks.ToString());
        }

        

        public static int PendingRewardValue
        {
            get
            {
                if (!HasPendingDailyReward)
                    return -1;

                var val = PlayerPrefs.GetInt(nameof(PendingRewardValue), -1);

                if (val <= 0)
                {
                    PendingRewardValue = GameSettings.Default.DailyRewardSetting.rewards[Random.Range(0
                        , GameSettings.Default.DailyRewardSetting.rewards.Count)];
                    return PendingRewardValue;
                }

                return val;
            }
            set => PlayerPrefs.SetInt(nameof(PendingRewardValue), value);
        }

        public static bool HasPendingDailyReward => DateTime.Now.Subtract(LastRewardTime).Days > 0;

        public static bool GetReward(bool doubled)
        {
            if (!HasPendingDailyReward)
                return false;

            ResourceManager.Coins += PendingRewardValue * (doubled ? 2 : 1);
            PendingRewardValue = -1;
            LastRewardTime = DateTime.Now;
            return true;
        }
    }
#endif


    public struct LoadGameData
    {
        public GameType GameType { get; set; }
        public int Score { get; set; }
    }

    public enum GameType
    {
        New, Continue
    }
}
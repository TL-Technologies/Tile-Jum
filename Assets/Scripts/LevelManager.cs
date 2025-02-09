using System;
using System.Linq;
using MyGame;
using UnityEngine;

namespace Game
{
    public partial class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        public static event Action LevelStarted;
        public static event Action<ScoreInfo> Scored;
        public static event Action<bool> LevelResulted;

        [SerializeField] private Player _player;
        [SerializeField] private LevelCreator _levelCreator;
        [SerializeField] private CameraFollower _cameraFollower;


        private int _score;


        public Player Player => _player;

        public int Score
        {
            get { return _score; }
            private set
            {
                _score = value;
            }
        }

        public bool BestScoreArchived => Score >= MyGame.GameManager.BEST_SCORE;

        public int ContinuesPerfectCount { get; private set; }

        public State CurrentState { get; private set; }
        public float Progress => Mathf.Clamp01((float) JumpedTileCount / TotalTileCount);
        public static int CompletedLevel
        {
            get => PrefManager.GetInt(nameof(CompletedLevel),0);
            private set => PrefManager.SetInt(nameof(CompletedLevel),value);
        }

        public int JumpedTileCount { get; private set; }
        public int TotalTileCount { get; private set; }

        public int Level { get;private set; } = 1;

        public GameType GameType { get; private set; }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }


        private void ResourceManagerOnPlayerSkinSelectionChanged(PlayerSkin skin)
        {
            Player.PlayerSkin = skin;
        }


        private void Awake()
        {
            Instance = this;

            var loadGameData = GameManager.LoadGameData;
            GameType = loadGameData.GameType;
            Score = loadGameData.Score;
            Level = CompletedLevel + 1;
            _levelCreator.ApproximateColorTileCount = GetColorTileCountForLevel(Level);
            Player.JumpSpeed = GetJumpSpeedForLevel(Level);
            _levelCreator.StepDistance = Player.ForwardStepDistance;
            _levelCreator.CreateOrRefresh();
            TotalTileCount = _levelCreator.Tiles.Count();
            Player.Died +=PlayerOnDied;
            Player.HitEnd+=PlayerOnHitEnd;
            Player.JumpedOnTile +=PlayerOnJumpedOnTile;
        }

        private void PlayerOnHitEnd()
        {
            OverTheGame(true);
        }

        public static int GetColorTileCountForLevel(int lvl)
        {
            int count = 0;
            if (lvl <= 10)
                count = 10 + lvl;

            else if (lvl <= 20)
                count = 20 + (lvl - 10);

            else if (lvl < 50)
                count = Mathf.RoundToInt(30 + (lvl - 25) * 0.25f);
            else
                count = 40;

            return Mathf.Clamp(count,5,40);
        }

        public static float GetJumpSpeedForLevel(int lvl)
        {
            var val = 5f;
            if (lvl <=5)
                val = 8;
            else if (lvl <= 10)
                val = 8 - (0.09f * lvl);

            else if (lvl <= 20)
                val = 8 - (0.09f * 10) - (lvl - 10) * 0.06f;
            else
            {
                val = 8 - (0.09f * 10) - (20 - 10) * 0.06f;
            }

            return Mathf.Clamp(val, 4.5f, 8f);
        }

        private void PlayerOnJumpedOnTile(Tile tile, bool perfect, Vector3 point)
        {
            if (!perfect)
                ContinuesPerfectCount = 0;
            else
            {
                ContinuesPerfectCount++;
            }

            ResourceManager.Coins += ContinuesPerfectCount;
            Score += Level;
            JumpedTileCount++;
            Scored?.Invoke(new ScoreInfo
            {
                HitPoint = point,
                Perfect = perfect,
                RewardDiamond = ContinuesPerfectCount,
                Score = Level
            });
        }

        private void PlayerOnDied()
        {
            OverTheGame(false);
        }

   

        public void StartTheGame()
        {
            CurrentState = State.Playing;
            MyGame.GameManager.TOTAL_GAME_COUNT++;
            Player.Active = true;
            _cameraFollower.Active = true;
            Player.JumpTo(_levelCreator.Tiles.First().transform.position - Vector3.forward*0.1f);
            LevelStarted?.Invoke();
        }

        private void Start()
        {
        }


        private void Update()
        {
            if (CurrentState == State.GameOver)
                return;
        }


        [ContextMenu("OverTheGame")]
        // ReSharper disable once MethodNameNotMeaningful
        private void OverTheGame(bool success)
        {
            if (CurrentState == State.GameOver)
                return;
            Player.Active = false;
            CurrentState = State.GameOver;
            if (success)
                CompletedLevel = Level;
            if (MyGame.GameManager.BEST_SCORE < Score)
            {
                MyGame.GameManager.BEST_SCORE = Score;
            }
            LevelResulted?.Invoke(success);
        }

        public enum State
        {
            None,
            Playing,
            GameOver
        }
    }

//    //Continue Section
//    public partial class LevelManager
//    {
//        public const int MAX_CONTINUE_COUNT = 1;
//        public bool AbleToContinue => MAX_CONTINUE_COUNT > ContinueCount;
//
//        public int ContinueCount { get; private set; }
//
//        public void ContinueTheGame()
//        {
//            if (!AbleToContinue)
//            {
//                return;
//            }
//
//            ContinueCount++;
//
//            //TODO:Implement The Logic
//        }
//    }
}

public struct ScoreInfo
{
    public int Score { get; set; }
    public bool Perfect { get; set; }
    public Vector3 HitPoint { get; set; }
    public int RewardDiamond { get; set; }
}
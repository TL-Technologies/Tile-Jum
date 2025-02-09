using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GamePlayPanel : ShowHidable
    {
        [SerializeField] private Text _scoreTxt;
        [SerializeField] private string _scorePrefix = "";
        [SerializeField] private PointsFeedback _pointsFeedbackPrefab;
        [SerializeField] private PerfectFeedback _perfectFeedbackPrefab;
        [SerializeField] private LevelProgressUI _levelProgressUI;
        [SerializeField] private Image _targetCoinImg;

        private LevelManager LevelManager=>LevelManager.Instance;

        protected override void OnEnable()
        {
            base.OnEnable();
            LevelManager.Scored +=LevelManagerOnScored;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LevelManager.Scored -= LevelManagerOnScored;
        }

        private void LevelManagerOnScored(ScoreInfo score)
        {
            var pointsFeedback = Instantiate(_pointsFeedbackPrefab,transform);
            var screenPoint = Camera.main.WorldToScreenPoint(score.HitPoint);
            pointsFeedback.Value = score.Score;
            pointsFeedback.transform.position = screenPoint;
            Destroy(pointsFeedback,3f);
            
            if (score.Perfect)
            {
                var perfect = Instantiate(_perfectFeedbackPrefab,transform);
                perfect.transform.position = screenPoint;
                perfect.CoinsReward = score.RewardDiamond;
//                StartCoroutine(ShowCollectCoinEffect(score.RewardDiamond, screenPoint));
                Destroy(perfect,3f);
            }

        }

        public override void Show(bool animate = true, Action completed = null)
        {
            _levelProgressUI.Level = LevelManager.Level;
            _levelProgressUI.Progress = 0;
            base.Show(animate, completed);
        }

        private void Update()
        {
            if(_scoreTxt!=null)
            _scoreTxt.text = $"{_scorePrefix}{LevelManager.Score}";
            _levelProgressUI.Progress = LevelManager.Progress;
        }

        private IEnumerator ShowCollectCoinEffect(int count,Vector2 point)
        {
            yield return new WaitForSeconds(0.3f);

            for (var i = 0; i < count; i++)
            {
                var image = Instantiate(_targetCoinImg, transform);
                var originalScale = image.transform.localScale;
                var startPoint = point;
                var endPoint = (Vector2)_targetCoinImg.transform.position;
                image.transform.localScale = originalScale * 3f;
                image.transform.position = startPoint;
                CalculateApproximateMoveCurve(startPoint, endPoint, out var curveDistance, out var maxCurvePoint);
                StartCoroutine(SimpleCoroutine.LerpNormalizedEnumerator(targetNormalized: 1.8f,
                    onCallOnFrame: n =>
                    {
                        //                        if (n > 0.1f)
                        //                            Debug.Break();
                        image.transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, n);
                        image.transform.position =
                            CalculateCurvePoint(startPoint, endPoint, curveDistance, n, maxCurvePoint);
                    },
                    onFinished: () => { Destroy(image.gameObject); },
                    lerpSpeed: (Mathf.Clamp(1 / (endPoint - startPoint).magnitude * 3000f, 0.8f, 2f))));
            }

        }


        private void CalculateApproximateMoveCurve(Vector2 startScreenPoint, Vector2 endScreenPoint,
            out float curveDistance, out float maxCurvePoint)
        {
            //TODO:Improve Later

            curveDistance = startScreenPoint.x > endScreenPoint.x ? -100f : 100f;
            maxCurvePoint = 0.3f;
        }


        private static Vector2 CalculateCurvePoint(Vector2 startPoint, Vector2 endPoint, float curveDistance,
            float normalized, float maxCurvePoint = 0.5f)
        {
            var perpendicularVec = Vector2.Perpendicular((endPoint - startPoint).normalized).normalized;
            return Vector2.Lerp(startPoint, endPoint, normalized) +
                   (curveDistance * (normalized > maxCurvePoint
                        ? 1 - (((normalized - maxCurvePoint)) / (1 - maxCurvePoint))
                        : 1 - ((maxCurvePoint - normalized) / maxCurvePoint)) * perpendicularVec);
        }
    }
}
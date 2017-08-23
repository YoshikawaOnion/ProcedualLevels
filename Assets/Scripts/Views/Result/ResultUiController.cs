using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class ResultUiController : MonoBehaviour, Models.IResultView
    {
        [SerializeField]
        private UILabel restTimeLabel = null;
        [SerializeField]
        private UILabel scoreLabel = null;
        [SerializeField]
        private UILabel tapInstructionLabel = null;

        public IObservable<Unit> OnTap { get; set; }

        public void Initialize(int restTime, int score)
        {
            OnTap = this.UpdateAsObservable()
                        .Where(x => Input.GetMouseButtonDown(0));
            restTimeLabel.text = GetTimeString(restTime);
            scoreLabel.text = score.ToString();
            tapInstructionLabel.gameObject.SetActive(false);
        }

        public IObservable<Unit> AnimateBonusAsync(int restTime, int score, int finalScore)
        {
            float finalCount = 120;
            float count = 0;

            return this.UpdateAsObservable()
                       .TakeWhile(x => count < finalCount)
                       .Do(x =>
            {
                ++count;
                var time = restTime * (finalCount - count) / finalCount;
                var score2 = (score * (finalCount - count) + finalScore * count) / finalCount;
                restTimeLabel.text = GetTimeString((int)time);
                scoreLabel.text = ((int)score2).ToString();
            });
        }

        public void ResetScoreBoard(int restTime, int score)
        {
            restTimeLabel.text = GetTimeString(restTime);
            scoreLabel.text = score.ToString();
        }

        private string GetTimeString(int restSeconds)
        {
            var minutes = (restSeconds / 60).ToString("00");
            var seconds = (restSeconds % 60).ToString("00");
            return minutes + ":" + seconds;
        }

        public void ShowTapInstruction()
        {
            tapInstructionLabel.gameObject.SetActive(true);
        }
    }
}
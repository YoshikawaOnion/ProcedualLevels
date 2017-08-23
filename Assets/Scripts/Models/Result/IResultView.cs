using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface IResultView
    {
        IObservable<Unit> OnTap { get; }
        IObservable<Unit> AnimateBonusAsync(int restTime, int score, int finalScore);
        void ResetScoreBoard(int restTime, int score);
        void ShowTapInstruction();
    }
}
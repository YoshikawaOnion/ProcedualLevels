using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Text;
using UnityEngine.UI;
using System;

namespace ProcedualLevels.Views
{
    public class TimeLimit : MonoBehaviour
    {
        private Text Text { get; set; }
        private IDisposable Disposable { get; set; }
     
        public void Initialize(Models.AdventureContext context)
        {
            Text = GetComponent<Text>();

            Disposable = context.TimeLimit.Subscribe(x =>
            {
                var stringBuilder = new StringBuilder();
                var minutes = x / 60;
                var seconds = x % 60;
                stringBuilder.Append(minutes.ToString("00"));
                stringBuilder.Append(":");
                stringBuilder.Append(seconds.ToString("00"));
                Text.text = stringBuilder.ToString();
            });
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
            Text = null;
        }
    }
}
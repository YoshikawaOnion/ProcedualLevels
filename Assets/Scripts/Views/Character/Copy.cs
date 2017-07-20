using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public class Copy : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Observable.Timer(TimeSpan.FromSeconds(120))
                      .Subscribe(x => gameObject.SetActive(false));
        }
    }
}
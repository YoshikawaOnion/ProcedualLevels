using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class CallbackDisposable : IDisposable
    {
        private Action callback;

        public CallbackDisposable(Action callback)
        {
            this.callback = callback;
        }

        public void Dispose()
        {
            callback();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface IFlow
    {
        IObservable<IFlow> Start();
    }
}
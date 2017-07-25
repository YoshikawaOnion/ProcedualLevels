using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class BattlerKnockbackController : MonoBehaviour
	{
        private CompositeDisposable KnockbackStateDisposable { get; set; }
        private BattlerController Battler { get; set; }
        private Rigidbody2D Rigidbody { get; set; }

        private void Start()
        {
            KnockbackStateDisposable = new CompositeDisposable();
            Battler = GetComponent<BattlerController>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}
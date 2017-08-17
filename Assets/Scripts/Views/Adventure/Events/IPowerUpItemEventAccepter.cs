using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// パワーアップアイテムに関するイベントを受け付けるインターフェース。
    /// </summary>
	public interface IPowerUpItemEventAccepter
	{
		IObserver<Models.PowerUp> OnPlayerGetPowerUpSender { get; }
	}
}

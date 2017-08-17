using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゲーム本体のエントリポイント。
    /// </summary>
    public class EntryManager : MonoBehaviour
    {
        private void Start()
        {
            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            
            var model = new Models.GameManager();
            var view = Instantiate(viewPrefab);

            var dungeonGen = AssetRepository.I.DungeonGenAsset;
            var battlerGen = AssetRepository.I.GameParameterAsset;

            Observable.NextFrame()
                      .Subscribe(x =>
			{
				model.Initialize(dungeonGen, battlerGen, view);
			}); 
        }
    }
}
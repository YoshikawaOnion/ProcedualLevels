using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class EntryManager : MonoBehaviour
    {
        private void Start()
        {
            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            
            var model = new Models.GameManager();
            var view = Instantiate(viewPrefab);

			var dungeonGen = Resources.Load<DungeonGenAsset>(Models.Def.DungeonGenAssetPath);
			var battlerGen = Resources.Load<GameParameterAsset>(Models.Def.GameParameterAssetPath);

            Observable.EveryUpdate()
                      .Skip(1)
                      .First()
                      .Subscribe(x =>
			{
				model.Initialize(dungeonGen, battlerGen, view);
			}); 
        }
    }
}
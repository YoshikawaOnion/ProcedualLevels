using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class AdventureViewContext
    {
        public IGameEventReceiver EventReceiver { get; set; }
        public HeroController Hero { get; set; }
        public AdventureContext Model { get; set; }

        private Dictionary<string, EnemyController> Prefabs { get; set; }

        public AdventureViewContext()
        {
            Prefabs = new Dictionary<string, EnemyController>();
        }

        public EnemyController SpawnEnemy(Enemy enemy)
        {
            EnemyController prefab;
            if (!Prefabs.TryGetValue(enemy.Ability.PrefabName, out prefab))
            {
                prefab = Resources.Load<EnemyController>
                                  ("Prefabs/Enemy/" + enemy.Ability.PrefabName);
                Prefabs[enemy.Ability.PrefabName] = prefab;
            }

            var obj = Object.Instantiate(prefab);
            obj.transform.position = enemy.InitialPosition
                .ToVector3()
                .MergeZ(prefab.transform.position.z);
            obj.Initialize(enemy, this);
            obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);

            return obj;
        }
    }
}
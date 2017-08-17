using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameObjectManager : MonoBehaviour
    {
        public HeroController HeroController { get; private set; }
        public BattlerController[] Battlers
        {
            get
            {
                return EnemyControllers.Cast<BattlerController>()
                                       .Append(HeroController)
                                       .ToArray();
            }
        }

        private Dictionary<string, EnemyController> EnemyPrefabs { get; set; }
        private MapTipRenderer MapTipRenderer { get; set; }
        private MapView MapView { get; set; }
        private List<EnemyController> EnemyControllers { get; set; }

        private AdventureContext ModelContext { get; set; }
        private AdventureViewContext Context { get; set; }
        private GameEventFacade EventFacade { get; set; }
        private bool Quiting { get; set; }

        public void Initialize(AdventureContext context,
                               AdventureViewContext viewContext,
                               GameEventFacade eventFacade)
        {
            ModelContext = context;
            Context = viewContext;
            EventFacade = eventFacade;

            EnemyPrefabs = new Dictionary<string, EnemyController>();
            MapTipRenderer = SetMaptipUp();
            MapView = SetMapUp();
            HeroController = SetHeroUp();
            EnemyControllers = new List<EnemyController>();
            Quiting = false;

            foreach (var item in context.Map.Enemies)
            {
                EnemyControllers.Add(SpawnEnemy(item));
            }
        }

        /// <summary>
        /// 敵をスポーンさせます。
        /// </summary>
        /// <returns>生成した敵キャラクターのビュー。</returns>
        /// <param name="enemy">生成する敵キャラクターのモデル。</param>
        public EnemyController SpawnEnemy(Enemy enemy)
        {
            var obj = SetEnemyUp(enemy);
            EnemyControllers.Add(obj);
            return obj;
        }

        /// <summary>
        /// パワーアップアイテムをスポーンさせます。
        /// </summary>
        /// <param name="battlerIndex">アイテムを落とす敵のインデックス。</param>
        /// <param name="powerUp">パワーアップアイテムのモデル。</param>
        public void SpawnPowerUp(int battlerIndex, PowerUp powerUp)
        {
            PowerUpItemController obj = SetPowerupUp(battlerIndex, powerUp);
            obj.transform.SetParent(transform);
        }


        private PowerUpItemController SetPowerupUp(int battlerIndex, PowerUp powerUp)
        {
            var battler = Battlers.First(x => x.Battler.Index == battlerIndex);
            var prefab = Resources.Load<PowerUpItemController>("Prefabs/Character/PowerUp");
            var obj = Instantiate(prefab);
            obj.Initialize(powerUp, EventFacade);
            obj.transform.position = battler.transform.position.MergeZ(obj.transform.position.z);
            return obj;
        }

        private MapTipRenderer SetMaptipUp()
        {
            var maptipManagerPrefab = Resources.Load<MapTipRenderer>("Prefabs/Manager/MaptipRenderer");
            var obj = Instantiate(maptipManagerPrefab);
            obj.Initialize(ModelContext.Map);
            return obj;
        }

        private MapView SetMapUp()
        {
            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var obj = Instantiate(mapPrefab);
            obj.Initialize(ModelContext.Map, Context, EventFacade);
            return obj;
        }

        private HeroController SetHeroUp()
        {
            var heroPrefab = Resources.Load<HeroController>("Prefabs/Character/Hero");
            var obj = Instantiate(heroPrefab);
            obj.transform.position = ModelContext.Map.StartLocation + Vector2.one * 0.5f;
            obj.transform.SetPositionZ(heroPrefab.transform.position.z);
            obj.Initialize(ModelContext.Hero, EventFacade, EventFacade, Context);
            obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
            return obj;
        }

        private EnemyController SetEnemyUp(Enemy enemy)
        {
            EnemyController prefab;
            if (!EnemyPrefabs.TryGetValue(enemy.Ability.PrefabName, out prefab))
            {
                prefab = Resources.Load<EnemyController>
                                  ("Prefabs/Enemy/" + enemy.Ability.PrefabName);
                EnemyPrefabs[enemy.Ability.PrefabName] = prefab;
            }

            var obj = Instantiate(prefab);
            obj.transform.position = enemy.InitialPosition
                .ToVector3()
                .MergeZ(prefab.transform.position.z);
            obj.Initialize(enemy, Context);
            obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
            return obj;
        }

        private void OnApplicationQuit()
        {
            Quiting = true;
        }

        private void OnDestroy()
        {
            if (Quiting)
            {
                return;
            }

            foreach (var battler in Battlers)
            {
                if (battler != null)
                {
                    Destroy(battler.gameObject);
                }
            }
            Destroy(MapView.gameObject);
            Destroy(MapTipRenderer.gameObject);
        }
    }
}
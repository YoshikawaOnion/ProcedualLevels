﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour, IAdventureView
    {
        private GameEventFacade EventFacade { get; set; }
        private HeroController HeroController { get; set; }
        private EnemyController[] EnemyControllers { get; set; }
        public IObservable<Models.Enemy> BattleObservable { get; private set; }
        public BattlerController[] Battlers
        {
            get
            {
                return EnemyControllers.Cast<BattlerController>()
                                       .Append(HeroController)
                                       .ToArray();
            }
        }

        public IObservable<PowerUp> GetPowerUpObservable { get; private set; }

        private void Start()
        {
            EventFacade = new GameEventFacade();
            BattleObservable = EventFacade.OnPlayerBattleWithEnemyReceiver;
            GetPowerUpObservable = EventFacade.OnPlayerGetPowerUpReceiver;
        }

        public void Initialize(AdventureContext context)
        {
            SetMapUp(context);
            SetHeroUp(context);
            SetMaptipUp(context);
            SetEnemiesUp(context);
        }

        private void SetEnemiesUp(AdventureContext context)
        {
            var viewContext = new AdventureViewContext
            {
                Hero = HeroController,
                EventReceiver = EventFacade,
            };
            var prefabs = new Dictionary<string, EnemyController>();
            var list = new List<EnemyController>();

            foreach (var enemy in context.Enemies)
            {
                EnemyController prefab;
                if (!prefabs.TryGetValue(enemy.Ability.PrefabName, out prefab))
                {
                    prefab = Resources.Load<EnemyController>
                                      ("Prefabs/Character/" + enemy.Ability.PrefabName);
                    prefabs[enemy.Ability.PrefabName] = prefab;
                }

                var obj = Instantiate(prefab);
                obj.transform.position = enemy.InitialPosition
                    .ToVector3()
                    .MergeZ(prefab.transform.position.z);
                obj.Initialize(enemy, viewContext);
                obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
                list.Add(obj);
            }

            EnemyControllers = list.ToArray();
        }

        private static void SetMaptipUp(AdventureContext context)
        {
            var maptipManagerPrefab = Resources.Load<MapTipRenderer>("Prefabs/Manager/MaptipRenderer");
            var maptipManager = Instantiate(maptipManagerPrefab);
            maptipManager.Initialize(context.Map);
        }

        private void SetMapUp(AdventureContext context)
        {
            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var mapObj = Instantiate(mapPrefab);
            mapObj.Initialize(context.Map, this);
        }

        private void SetHeroUp(AdventureContext context)
        {
            var heroPrefab = Resources.Load<HeroController>("Prefabs/Character/Hero");
            HeroController = Instantiate(heroPrefab);
            HeroController.transform.position = context.Map.StartLocation;
            HeroController.transform.SetPositionZ(heroPrefab.transform.position.z);
            HeroController.Initialize(context.Hero, RootObjectRepository.I.GameUi, EventFacade);
            HeroController.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
        }

        // Update is called once per frame
        void Update()
        {
            if (HeroController != null)
            {
                RootObjectRepository.I.Camera.transform.position = HeroController.transform.position.AddY(2).MergeZ(-10);
            }
        }

        public void Knockback(Battler battlerSubject, Battler battlerAgainst, int power)
        {
            var battlers = Battlers;
            var subject = battlers.FirstOrDefault(x => x.Battler.Index == battlerSubject.Index);
            var against = battlers.FirstOrDefault(x => x.Battler.Index == battlerAgainst.Index);

            if (subject != null && against != null)
            {
                subject.Knockback(against, power);
            }
        }

        public void ShowDeath(Battler subject)
        {
            var obj = Battlers.FirstOrDefault(x => x.Battler.Index == subject.Index);
            obj.Die();
        }

        public void PlacePowerUp(int battlerIndex, PowerUp powerUp)
        {
            var battler = Battlers.First(x => x.Battler.Index == battlerIndex);
            var prefab = Resources.Load<PowerUpItemController>("Prefabs/Character/PowerUp");
            var obj = Instantiate(prefab);
            obj.Initialize(powerUp, EventFacade);
            obj.transform.position = battler.transform.position.MergeZ(obj.transform.position.z);
        }
    }
}
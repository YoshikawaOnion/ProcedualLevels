using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class SpawnerDatabase
    {
        public SpawnerParameter[] Spawners { get; private set; }

        public SpawnerDatabase()
        {
            var database = new EnemyDatabase();

            Spawners = new SpawnerParameter[]
            {
                new SpawnerParameter()
                {
                    Name = "GhostSpawner",
                    Generator = new AtCenterSpawnerGenStrategy(),
                    Behavior = new TimeLimitSpawnerBehavior(),
                    EnemiesAbility = database.Enemies[2],
                },
                new SpawnerParameter()
                {
                    Name = "SlimeSpawner",
                    Generator = new RandomSpawnerGenStrategy(),
                    Behavior = new TimeSpanSpawnerBehavior(TimeSpan.FromSeconds(20)),
                    EnemiesAbility = database.Enemies[0],
                },
            };
        }
    }
}
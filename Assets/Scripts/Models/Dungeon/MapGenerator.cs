using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ダンジョンを生成する機能を提供するクラス。
    /// </summary>
    public class MapGenerator
    {
        public int ActualHorizontalPathThickness
        {
            get { return DungeonAsset.HorizontalPathThickness + DungeonAsset.ColliderMargin * 2; }
        }

        public int ActualVerticalPathThickness
        {
            get { return DungeonAsset.VerticalPathThickness + DungeonAsset.ColliderMargin * 2; }
        }

        private GameParameterAsset BattlerAsset { get; set; }
        private DungeonGenAsset DungeonAsset { get; set; }

        public MapGenerator(GameParameterAsset battlerAsset, DungeonGenAsset dungeonAsset)
        {
            BattlerAsset = battlerAsset;
            DungeonAsset = dungeonAsset;
        }

        /// <summary>
        /// マップデータをランダムに生成します。
        /// </summary>
        /// <returns>生成したマップ。</returns>
        /// <param name="leftBottom">生成範囲の左下の座標</param>
        /// <param name="rightTop">生成範囲の右上の座標</param>
        public MapData GenerateMap(Vector2 leftBottom, Vector2 rightTop, IAdventureView view)
        {
            var map = new MapData();

            GenerateRooms(leftBottom, rightTop, map);

            var pathGen = new OnBottomPathGenStrategy();
            map.Connections = pathGen.ConnectRooms(map).ToList();

            ReducePathesAtRandom(map);

            PlaceStartAndGoal(map);
            PlacePlatforms(map);
            PlaceEnemies(view, map);
            PlaceSpawners(map);

            var blocks = map.Connections.SelectMany(x => x.Path.GetCollisionBlocks());
            map.CollisionBlocks.AddRange(blocks);

            return map;
        }

        private void PlaceCollisionBlock(MapData map, IMapPath path)
        {
            var pathRooms = path.GetRooms();
            foreach (var pathRoom in pathRooms)
            {
                var rooms = map.Divisions.Select(x => x.Room)
                               .Concat(pathRooms.Except(new[] { pathRoom }));
                foreach (var room in rooms)
                {
                    // 垂直通路と部屋の交点の右上
                    if (pathRoom.Bottom <= room.Top && room.Top <= pathRoom.Top
                       && room.Left <= pathRoom.Right && pathRoom.Right < room.Right)
                    {
                        var pos = new Vector2(pathRoom.Right - 1, room.Top - 1);
                        AddCollisionBlock(map, pos);
                    }

                    // 垂直通路と部屋の交点の右下
                    if (pathRoom.Bottom <= room.Bottom && room.Bottom <= pathRoom.Top
                       && room.Left <= pathRoom.Right && pathRoom.Right < room.Right)
                    {
						var pos = new Vector2(pathRoom.Right - 1, room.Bottom);
						AddCollisionBlock(map, pos);
                    }

                    if (pathRoom.Left <= room.Left && room.Left <= pathRoom.Right
                       && room.Bottom <= pathRoom.Bottom && pathRoom.Bottom <= room.Top)
                    {
                        var pos1 = new Vector2(room.Left, pathRoom.Top - 1);
                        var pos2 = new Vector2(room.Left, pathRoom.Bottom);
						AddCollisionBlock(map, pos1);
						AddCollisionBlock(map, pos2);
                    }
                }
            }
        }

        private void AddCollisionBlock(MapData map, Vector2 pos)
        {
            if (!map.IsRoom((int)pos.x, (int)pos.y)
                && !map.IsPath((int)pos.x, (int)pos.y))
            {
                map.CollisionBlocks.Add(pos);
            }
        }

        private static void PlaceSpawners(MapData map)
        {
            var spawnerDatabase = new SpawnerDatabase();
            foreach (var spawner in spawnerDatabase.Spawners)
            {
                var spawners = spawner.Generate(map);
                map.Spawners.AddRange(spawners);
            }
        }

        private static void PlaceEnemies(IAdventureView view, MapData map)
        {
            var enemyDatabase = new EnemyDatabase();
            int index = 1;
            foreach (var ability in enemyDatabase.Enemies)
            {
                ability.GenerationStrategy.PlaceEnemies(map, ability, view, ref index);
            }
        }

        /// <summary>
        /// 全ての部屋を生成します。
        /// </summary>
        /// <param name="leftBottom">部屋を生成できる範囲の左下座標。</param>
        /// <param name="rightTop">部屋を生成できる範囲の右上座標。</param>
        /// <param name="map">結果を書き込むマップデータ。</param>
        private void GenerateRooms(Vector2 leftBottom, Vector2 rightTop, MapData map)
        {
            var roomGen = new HorizontalRoomGenStrategy();
            var root = new MapRectangle(
                (int)leftBottom.x,
                (int)rightTop.x,
                (int)leftBottom.y,
                (int)rightTop.y);
            var divisions = roomGen.GenerateRooms(root);
            map.Divisions.AddRange(divisions);
        }

        /// <summary>
        /// マップに配置されている通路を、部屋が孤立しないようにランダムに削減します。
        /// </summary>
        /// <param name="map">マップデータ。</param>
        private void ReducePathesAtRandom(MapData map)
        {
            var head = map.Divisions[0];
            var loop = (int)(map.Divisions.Count * DungeonAsset.PathReducingChance);

            MarkConnectedRooms(map, head, 0);
            for (int i = 0; i < loop; i++)
            {
                var connection = map.Connections.GetRandom();
                map.Connections.Remove(connection);
                MarkConnectedRooms(map, head, i + 1);

                foreach (var item in map.Divisions)
                {
                    if (item.ReducingMarker != i + 1)
                    {
                        map.Connections.Add(connection);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 指定した部屋と繋がっている部屋をマーキングします。
        /// </summary>
        /// <param name="root">繋がっていると判定する根元の部屋。</param>
        /// <param name="index">マークの値。</param>
        private void MarkConnectedRooms(MapData map, MapDivision root, int index)
        {
            if (root.ReducingMarker == index)
            {
                return;
            }
            root.ReducingMarker = index;

            var connections = map.Connections.Where(x => x.BottomDivision.Index == root.Index
                                                    || x.TopDivision.Index == root.Index);
            foreach (var item in connections)
            {
                MarkConnectedRooms(map, item.BottomDivision, index);
                MarkConnectedRooms(map, item.TopDivision, index);
            }
        }

        /// <summary>
        /// スタート地点とゴール地点を設定します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
		private void PlaceStartAndGoal(MapData map)
        {
            var startDivision = map.Divisions.MinItem(x => x.Room.Left);

            var startX = Helper.GetRandomInRange(startDivision.Room.Left + 1, startDivision.Room.Right - 2);
            map.StartLocation = new Vector2(startX, startDivision.Room.Bottom + 1);

            MapDivision goalDivision;
            goalDivision = map.Divisions.MaxItem(x => x.Room.Right);

            var goalX = Helper.GetRandomInRange(goalDivision.Room.Left + 1, goalDivision.Room.Right - 2);
            map.GoalLocation = new Vector2(goalX, goalDivision.Room.Bottom + 1);
        }

        /// <summary>
        /// 空中の足場を生成します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
        private void PlacePlatforms(MapData map)
        {
            var rooms = map.Divisions.Select(x => x.Room)
                          .ToArray();
            var colliderMargin = DungeonAsset.ColliderMargin;

            foreach (var room in rooms)
            {
                var bottom = room.Bottom + DungeonAsset.PlatformSpan;
                var top = room.Top - colliderMargin - 1;
                for (int i = bottom; i < top; i += DungeonAsset.PlatformSpan)
                {
                    var left = GetRandomInRange(room.Left + colliderMargin, room.Right - 1 - colliderMargin);
                    var right = GetRandomInRange(left + 1, room.Right - colliderMargin);
                    var platform = new MapPlatform()
                    {
                        Left = left,
                        Bottom = i,
                        Right = right
                    };
                    map.Platforms.Add(platform);
                }
            }
        }

        /// <summary>
        /// 指定した範囲内の乱数を返します。
        /// </summary>
        /// <returns>範囲内の乱数。</returns>
        /// <param name="min">乱数の最小値。</param>
        /// <param name="max">乱数の最大値。</param>
		private int GetRandomInRange(int min, int max)
        {
            if (min > max)
            {
                Debug.LogWarning("min:" + min + " is greater than max:" + max + ".");
            }
            return (int)(UnityEngine.Random.value * (max - min)) + min;
        }

        /// <summary>
        /// 部屋の中からランダムな一点を返します。
        /// </summary>
        /// <returns>部屋の中のランダムな点。</returns>
        /// <param name="room">点が含まれる部屋。</param>
        /// <param name="margin">部屋の外周からの最小距離。</param>
		private Vector2 GetRandomLocation(MapRectangle room, int margin)
        {
            var x = GetRandomInRange(room.Left + margin, room.Right - margin);
            var y = GetRandomInRange(room.Bottom + margin, room.Top - margin);
            return new Vector2(x + 0.5f, y + 0.5f);
        }
    }
}
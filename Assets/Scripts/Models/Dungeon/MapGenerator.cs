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

        private BattlerGenAsset BattlerAsset { get; set; }
        private DungeonGenAsset DungeonAsset { get; set; }

        public MapGenerator(BattlerGenAsset battlerAsset, DungeonGenAsset dungeonAsset)
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
            pathGen.ConnectRooms(map);

            ReducePathesAtRandom(map);

			PlaceStartAndGoal(map);
			PlacePlatforms(map);

            var enemyDatabase = new EnemyDatabase();
            foreach (var ability in enemyDatabase.Enemies)
            {
                ability.GenerationStrategy.PlaceEnemies(map, ability, view);
            }

            return map;
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

            MarkConnectedRooms(head, 0);
            for (int i = 0; i < loop; i++)
            {
                var divIndex = GetRandomInRange(0, map.Divisions.Count - 1);
                var connections = map.Divisions[divIndex].Connections;
                if (connections.Count == 0)
                {
                    continue;
                }

                var pathIndex = GetRandomInRange(0, connections.Count - 1);
                if (pathIndex >= connections.Count)
                {
                    continue;
                }
                var path = connections[pathIndex];
                connections.RemoveAt(pathIndex);

                MarkConnectedRooms(head, i + 1);

                foreach (var item in map.Divisions)
                {
                    if (item.ReducingMarker != i + 1)
                    {
                        connections.Add(path);
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
        private void MarkConnectedRooms(MapDivision root, int index)
        {
            if (root.ReducingMarker == index)
            {
                return;
            }
            root.ReducingMarker = index;
            foreach (var item in root.Connections)
            {
				MarkConnectedRooms(item.BottomDivision, index);
				MarkConnectedRooms(item.TopDivision, index);
            }
        }

        /// <summary>
        /// スタート地点とゴール地点を設定します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
		private void PlaceStartAndGoal(MapData map)
		{
            var startDivision = map.Divisions.MinItem(x => x.Room.Left);

            map.StartLocation = GetRandomLocation(startDivision.Room, DungeonAsset.ColliderMargin);

			MapDivision goalDivision;
			goalDivision = map.Divisions.MaxItem(x => x.Room.Right);

			map.GoalLocation = GetRandomLocation(goalDivision.Room, DungeonAsset.ColliderMargin);
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
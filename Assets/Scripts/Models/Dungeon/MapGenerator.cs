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
        /// <summary>
        /// 部屋を分割する際の子区画の最小サイズを取得または設定します。
        /// </summary>
        public int ChildBoundMinSize { get; set; }
        /// <summary>
        /// 部屋を分割する際の親区画の最小サイズを取得または設定します。
        /// </summary>
        public int ParentBoundMinSize { get; set; }
        /// <summary>
        /// 部屋の属する区画に対する余白サイズを取得または設定します。
        /// </summary>
        public int MarginSize { get; set; }
        /// <summary>
        /// 水平な通路の幅を取得または設定します。
        /// </summary>
        public int HorizontalPathThickness { get; set; }
        /// <summary>
        /// 鉛直な通路の幅を取得または設定します。
        /// </summary>
        public int VerticalPathThickness { get; set; }
        /// <summary>
        /// 部屋の最小サイズを取得または設定します。
        /// </summary>
        public int RoomMinSize { get; set; }
        /// <summary>
        /// 部屋の最大サイズを取得または設定します。
        /// </summary>
        public int RoomMaxSize { get; set; }
        /// <summary>
        /// 一部屋あたりの、通路の削除を試みる回数を取得または設定します。
        /// </summary>
        public float PathReducingChance { get; set; }
        /// <summary>
        /// 一部屋あたりの敵がいるマスの割合を取得または設定します。
        /// </summary>
        public float EnemyCountRatio { get; set; }
        /// <summary>
        /// すり抜け防止用のColliderのための余白サイズを取得または設定します。
        /// </summary>
        public int ColliderMargin { get; set; }

        public int ActualHorizontalPathThickness
        {
            get { return HorizontalPathThickness + ColliderMargin * 2; }
        }

        public int ActualVerticalPathThickness
        {
            get { return VerticalPathThickness + ColliderMargin * 2; }
        }

        public MapGenerator()
        {
            ChildBoundMinSize = 6;
            ParentBoundMinSize = 12;
            MarginSize = 2;
            HorizontalPathThickness = 2;
            RoomMinSize = 24;
            RoomMaxSize = 64;
            PathReducingChance = 4;
            EnemyCountRatio = 0.03f;
            ColliderMargin = 1;
        }

        /// <summary>
        /// マップデータをランダムに生成します。
        /// </summary>
        /// <returns>生成したマップ。</returns>
        /// <param name="leftBottom">生成範囲の左下の座標</param>
        /// <param name="rightTop">生成範囲の右上の座標</param>
        public MapData GenerateMap(Vector2 leftBottom, Vector2 rightTop)
        {
            var map = new MapData();

            MarginSize = Mathf.Max(ActualVerticalPathThickness, ActualHorizontalPathThickness) + 1;
            GenerateRooms(leftBottom, rightTop, map);

            var pathGen = new PathGenStrategy();
            pathGen.ConnectRooms(map);

            ReducePathesAtRandom(map);

            PlaceStartAndGoal(map);
            PlaceEnemies(map);
            PlacePlatforms(map);

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
            var roomGen = new RoomGenStrategy();
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
            var loop = (int)(map.Divisions.Count * PathReducingChance);

            MarkConnectedRooms(head, 0);
            for (int i = 0; i < loop; i++)
            {
                var divIndex = GetRandomInRange(0, map.Divisions.Count - 1);
                var pathIndex = GetRandomInRange(0, map.Divisions[divIndex].ConnectedDivisions.Count - 1);
                if (pathIndex >= map.Divisions[divIndex].ConnectedDivisions.Count)
                {
                    continue;
                }
                var path = map.Divisions[divIndex].ConnectedDivisions[pathIndex];
                map.Divisions[divIndex].ConnectedDivisions.RemoveAt(pathIndex);

                MarkConnectedRooms(head, i + 1);

                foreach (var item in map.Divisions)
                {
                    if (item.ReducingMarker != i + 1)
                    {
                        map.Divisions[divIndex].ConnectedDivisions.Add(path);
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
            foreach (var item in root.ConnectedDivisions)
            {
                MarkConnectedRooms(item.BottomDivision, index);
            }
        }

        /// <summary>
        /// 敵キャラクターの配置を設定します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
		private void PlaceEnemies(MapData map)
		{
			foreach (var item in map.Divisions)
			{
				var count = (int)(item.Room.Width * item.Room.Height * EnemyCountRatio);
				for (int i = 0; i < count; i++)
				{
					var pos = GetRandomLocation(item.Room, ColliderMargin);
					if (pos != map.GoalLocation
					   && pos.x != map.StartLocation.x)
					{
						map.EnemyLocations.Add(pos);
					}
				}
			}
		}

        /// <summary>
        /// スタート地点とゴール地点を設定します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
		private void PlaceStartAndGoal(MapData map)
		{
			var startRoomIndex = GetRandomInRange(0, map.Divisions.Count - 1);
			map.StartLocation = GetRandomLocation(map.Divisions[startRoomIndex].Room, ColliderMargin);

			int goalRoomIndex;
			while (true)
			{
				goalRoomIndex = GetRandomInRange(0, map.Divisions.Count - 1);
				if (startRoomIndex != goalRoomIndex)
				{
					break;
				}
			}
			map.GoalLocation = GetRandomLocation(map.Divisions[goalRoomIndex].Room, ColliderMargin);
		}

        /// <summary>
        /// 空中の足場を生成します。
        /// </summary>
        /// <param name="map">設定を書き込むマップデータ。</param>
        private void PlacePlatforms(MapData map)
        {
            int platformSpan = 3;
            var rooms = map.Divisions.Select(x => x.Room)
                          .ToArray();
            foreach (var room in rooms)
            {
                for (int i = room.Bottom + platformSpan; i < room.Top - MarginSize; i += platformSpan)
                {
                    var left = GetRandomInRange(room.Left + ColliderMargin, room.Right - 2 - ColliderMargin);
                    var right = GetRandomInRange(left + 1, room.Right - 1 - ColliderMargin);
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
        /// 0から指定した値までの間の乱数を返します。
        /// ただし、部屋の最小・最大サイズを守るよう範囲を制限します。
        /// </summary>
        /// <returns>乱数</returns>
        /// <param name="size">乱数の最大値。</param>
        private int GetRandInRoom(int size)
        {
            var rand = (int)(UnityEngine.Random.value * size);
            return Mathf.Clamp(rand, ChildBoundMinSize, size - ParentBoundMinSize);
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
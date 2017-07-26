using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
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
        /// 通路の幅を取得または設定します。
        /// </summary>
        public int PathThickness { get; set; }
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

        public MapGenerator()
        {
            ChildBoundMinSize = 6;
            ParentBoundMinSize = 12;
            MarginSize = 2;
            PathThickness = 2;
            RoomMinSize = 24;
            RoomMaxSize = 64;
            PathReducingChance = 4;
            EnemyCountRatio = 0.03f;
            ColliderMargin = 0;
        }

        /// <summary>
        /// マップデータをランダムに生成します。
        /// </summary>
        /// <returns>生成したマップ。</returns>
        /// <param name="leftBottom">生成範囲の左下の座標</param>
        /// <param name="rightTop">生成範囲の右上の座標</param>
        public MapData GenerateMap(Vector2 leftBottom, Vector2 rightTop)
        {
            PathThickness += ColliderMargin * 2;
            RoomMinSize += ColliderMargin * 2;
            RoomMaxSize += ColliderMargin * 2;

            var map = new MapData();

            GenerateRooms(leftBottom, rightTop, map);
            ConnectRooms(map, true, (o, i) => o.Right == i.Left);
            ConnectRooms(map, false, (o, i) => o.Top == i.Bottom);
            ReducePathesAtRandom(map);
            PlaceStartAndGoal(map);
            PlaceEnemies(map);

            PathThickness -= ColliderMargin * 2;
            RoomMinSize -= ColliderMargin * 2;
            RoomMaxSize -= ColliderMargin * 2;

            return map;
        }

		private void GenerateRooms(Vector2 leftBottom, Vector2 rightTop, MapData map)
		{
			var parent = new MapRectangle(
				(int)leftBottom.x,
				(int)rightTop.x,
				(int)leftBottom.y,
				(int)rightTop.y);
			
			var divisions = GenerateDivisions(parent, true)
				.ToArray();
			
			int index = 0;
			foreach (var div in divisions)
			{
				var room = CreateRoom(div);
				map.Divisions.Add(new MapDivision()
				{
					Bound = div,
					Room = room,
					Index = index
				});
				index++;
			}
		}

		/// <summary>
		/// 指定した親区画を分割した区画のコレクションを、親から順に返します。
		/// </summary>
		/// <returns>分割済みの区画のコレクション。</returns>
		/// <param name="parent">ルートとなる親要素。</param>
		/// <param name="vertical"><c>true</c> を指定すると、垂直な線で分割します。</param>
		private IEnumerable<MapRectangle> GenerateDivisions(MapRectangle parent, bool vertical)
		{
			MapRectangle child = null;
			bool childIsToBeDivided = false;
			bool parentIsToBeDivided = false;
			var sizeToBeDivided = ChildBoundMinSize + ParentBoundMinSize;

			if (vertical)
			{
				var rand = GetRandInRoom(parent.Width);
				var left = parent.Left + rand;
				var right = parent.Right;
				child = new MapRectangle(left, right, parent.Bottom, parent.Top);
				parent.Right = left;

				childIsToBeDivided = child.Height > sizeToBeDivided;
				parentIsToBeDivided = parent.Height > sizeToBeDivided;
			}
			else
			{
				var rand = GetRandInRoom(parent.Height);
				var bottom = parent.Bottom + rand;
				var top = parent.Top;
				child = new MapRectangle(parent.Left, parent.Right, bottom, top);
				parent.Top = bottom;

				childIsToBeDivided = child.Width > sizeToBeDivided;
				parentIsToBeDivided = parent.Width > sizeToBeDivided;
			}

			if (UnityEngine.Random.value < 0.2f)
			{
				parentIsToBeDivided = false;
			}
			if (UnityEngine.Random.value < 0.2f)
			{
				childIsToBeDivided = false;
			}

			if (parentIsToBeDivided)
			{
				foreach (var item in GenerateDivisions(parent, !vertical))
				{
					yield return item;
				}
			}
			else
			{
				yield return parent;
			}
			if (childIsToBeDivided)
			{
				foreach (var item in GenerateDivisions(child, !vertical))
				{
					yield return item;
				}
			}
			else
			{
				yield return child;
			}
		}

		private MapRectangle CreateRoom(MapRectangle bound)
		{
			var room = bound.Clone();
			room.Left += MarginSize;
			room.Right -= MarginSize;
			room.Bottom += MarginSize;
			room.Top -= MarginSize;

			var widthMinReduce = Mathf.Max(0, bound.Width - RoomMaxSize);
			var heightMinReduce = Mathf.Max(0, bound.Height - RoomMaxSize);
			var widthReduce = GetRandomInRange(widthMinReduce, bound.Width - RoomMinSize);
			var heightReduce = GetRandomInRange(heightMinReduce, bound.Height - RoomMinSize);
			room.Left += widthReduce / 2;
			room.Right -= widthReduce / 2;
			room.Bottom += heightReduce / 2;
			room.Top -= heightReduce / 2;

			return room;
		}

		/// <summary>
		/// 指定した MapData に、MapData 内の部屋同士を結ぶ通路を追加します。
		/// </summary>
		/// <param name="map">更新する MapData。</param>
		/// <param name="horizontal"><c>true</c> の時、水平な通路を生成できる時のみ生成します。
		/// <c>false</c> の時、鉛直な通路を生成できる時のみ生成します。</param>
		/// <param name="isAdjacent">2つの部屋が隣接しているかどうかを判定する述語。</param>
		private void ConnectRooms(MapData map, bool horizontal, Func<MapRectangle, MapRectangle, bool> isAdjacent)
		{
			var list = new List<MapConnection>();

			foreach (var item in map.Divisions)
			{
				var adjacentes = map.Divisions
									.Where(x => isAdjacent(item.Bound, x.Bound));
				foreach (var a in adjacentes)
				{
					var path = CreatePath(item, a, horizontal, list);
					var connection = new MapConnection(item, a, path, horizontal);
					item.ConnectedDivisions.Add(connection);
					list.Add(connection);
				}
			}
		}

        /// <summary>
        /// 指定した二つの区画を繋ぐ通路を生成します。
        /// </summary>
        /// <returns>生成された通路のサイズを表す矩形。</returns>
        /// <param name="bottomDiv">もう一方の区画と繋ぐ区画。</param>
        /// <param name="topDiv">もう一方の区画と繋ぐ区画。</param>
        /// <param name="horizontal"><c>true</c> を指定すると、水平な通路を生成します。
        /// <c>false</c>を指定すると、鉛直な通路を生成します。</param>
        private MapPath CreatePath(MapDivision bottomDiv,
                                   MapDivision topDiv,
                                   bool horizontal,
                                   IEnumerable<MapConnection> connections)
        {
            MapRectangle path1 = CreatePathSegment(bottomDiv, horizontal, false);
            MapRectangle path2 = CreatePathSegment(topDiv, horizontal, true);

            MergePath(connections,
                     c => c.BottomDivision,
                     bottomDiv,
                     p => p.BottomPath,
                     path1,
                     horizontal);
            MergePath(connections,
                     c => c.TopDivision,
                     topDiv,
                     p => p.TopPath,
                     path2,
                     horizontal);

            var connection = new MapRectangle();
            if (horizontal)
            {
                connection.Left = path1.Right - PathThickness;
                connection.Right = path2.Left + PathThickness;
                connection.Bottom = Mathf.Min(path1.Bottom, path2.Bottom);
                connection.Top = Mathf.Max(path1.Top, path2.Top);
            }
            else
            {
                connection.Bottom = path1.Top - PathThickness;
                connection.Top = path2.Bottom + PathThickness;
                connection.Left = Mathf.Min(path1.Left, path2.Left);
                connection.Right = Mathf.Max(path1.Right, path2.Right);
            }

            return new MapPath(path1, connection, path2);
        }

		/// <summary>
		/// 通路の一部となる、部屋から区画の境界線まで伸びる通路パーツを生成します。
		/// </summary>
		/// <returns>通路の一部となる通路パーツ。</returns>
		/// <param name="div">通路の始点となる部屋。</param>
		/// <param name="horizontal"><c>true</c> の時、水平に通路を伸ばします。
		/// <c>false</c>の時、鉛直に通路を伸ばします。</param>
		/// <param name="isTopDiv"><c>true</c> の時、繋ぎたい2つの部屋のうち座標の大きな方から伸ばしているとみなします。
		/// <c>false</c>の時、座標の小さな方から伸ばしているとみなします。</param>
		private MapRectangle CreatePathSegment(MapDivision div, bool horizontal, bool isTopDiv)
		{
			var rect = new MapRectangle();

			if (horizontal)
			{
				if (isTopDiv)
				{
					// 少し部屋に食い込ませるために片方だけ端を広げる
					rect.Left = div.Bound.Left;
					rect.Right = div.Room.Left;
				}
				else
				{
					rect.Left = div.Room.Right;
					rect.Right = div.Bound.Right;
				}
				var pos = GetRandomInRange(
					div.Room.Bottom,
					div.Room.Top - PathThickness);
				rect.Bottom = pos;
				rect.Top = pos + PathThickness;
				rect.Left -= PathThickness / 2;
				rect.Right += PathThickness / 2;
			}
			else
			{
				if (isTopDiv)
				{
					rect.Bottom = div.Bound.Bottom;
					rect.Top = div.Room.Bottom;
				}
				else
				{
					rect.Bottom = div.Room.Top;
					rect.Top = div.Bound.Top;
				}
				var pos = GetRandomInRange(
					div.Room.Left,
					div.Room.Right - PathThickness);
				rect.Left = pos;
				rect.Right = pos + PathThickness;
				rect.Bottom -= PathThickness / 2;
				rect.Top += PathThickness / 2;
			}

			return rect;
		}

		private void MergePath(IEnumerable<MapConnection> connections,
                               Func<MapConnection, MapDivision> selectDivision,
                               MapDivision division,
                               Func<MapPath, MapRectangle> selectRect,
                               MapRectangle path,
                              bool horizontal)
        {
            var connection = connections.FirstOrDefault(x => selectDivision(x).Index == division.Index);
            if (connection != null && connection.Horizontal == horizontal)
            {
                var clone = selectRect(connection.Path).Clone();
                if (horizontal)
                {
                    path.Bottom = clone.Bottom;
                    path.Top = clone.Top;
                }
                else
                {
                    path.Left = clone.Left;
                    path.Right = clone.Right;
                }
            }
        }


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

		private void PlaceEnemies(MapData map)
		{
			foreach (var item in map.Divisions)
			{
				var count = (int)(item.Room.Width * item.Room.Height * EnemyCountRatio);
				for (int i = 0; i < count; i++)
				{
					var pos = GetRandomLocation(item.Room, 0);
					if (pos != map.GoalLocation
					   && pos.x != map.StartLocation.x)
					{
						map.EnemyLocations.Add(pos);
					}
				}
			}
		}

		private void PlaceStartAndGoal(MapData map)
		{
			var startRoomIndex = GetRandomInRange(0, map.Divisions.Count - 1);
			map.StartLocation = GetRandomLocation(map.Divisions[startRoomIndex].Room, MarginSize);

			int goalRoomIndex;
			while (true)
			{
				goalRoomIndex = GetRandomInRange(0, map.Divisions.Count - 1);
				if (startRoomIndex != goalRoomIndex)
				{
					break;
				}
			}
			map.GoalLocation = GetRandomLocation(map.Divisions[goalRoomIndex].Room, MarginSize);
		}


		private int GetRandomInRange(int min, int max)
        {
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

		private Vector2 GetRandomLocation(MapRectangle room, int margin)
		{
			var x = GetRandomInRange(room.Left + margin, room.Right - margin);
			var y = GetRandomInRange(room.Bottom + margin, room.Top - margin);
			return new Vector2(x + 0.5f, y + 0.5f);
		}
	}
}
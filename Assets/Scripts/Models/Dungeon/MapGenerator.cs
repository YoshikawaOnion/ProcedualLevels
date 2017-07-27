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
            ConnectRooms(map, true, (o, i) => o.Right == i.Left);
            ConnectRooms(map, false, (o, i) => o.Top == i.Bottom);
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

        /// <summary>
        /// 区画の中に部屋を生成します。
        /// </summary>
        /// <returns>生成した部屋の範囲。</returns>
        /// <param name="bound">部屋を生成できる区画の範囲。</param>
		private MapRectangle CreateRoom(MapRectangle bound)
		{
			var room = bound.Clone();

			var widthMinReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMaxSize);
			var heightMinReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMaxSize);
            var widthMaxReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMinSize);
            var heightMaxReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMinSize);
            var widthReduce = GetRandomInRange(widthMinReduce, widthMaxReduce);
			var heightReduce = GetRandomInRange(heightMinReduce, heightMaxReduce);
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
		/// <param name="isAdjacent">第二引数の部屋が第一引数の部屋から見て右上に隣接しているかどうかを判定する述語。</param>
		private void ConnectRooms(MapData map, bool horizontal, Func<MapRectangle, MapRectangle, bool> isAdjacent)
		{
			var list = new List<MapConnection>();

            foreach (var bottomDiv in map.Divisions)
			{
                var topDivs = map.Divisions
									.Where(x => isAdjacent(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in topDivs)
				{
					var path = CreatePath(bottomDiv, topDiv, horizontal, list);
					var connection = new MapConnection(bottomDiv, topDiv, path, horizontal);
					bottomDiv.ConnectedDivisions.Add(connection);
					list.Add(connection);
				}
			}
		}

        /// <summary>
        /// 指定した二つの区画を繋ぐ通路を生成します。
        /// </summary>
        /// <returns>生成された通路のサイズを表す矩形。</returns>
        /// <param name="bottomDiv">もう一方の区画と繋ぐ、座標の小さな方の区画。</param>
        /// <param name="topDiv">もう一方の区画と繋ぐ、座標の大きな方の区画。</param>
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
                connection.Left = path1.Right - ActualVerticalPathThickness;
                connection.Right = path2.Left + ActualVerticalPathThickness;
                connection.Bottom = Mathf.Min(path1.Bottom, path2.Bottom);
                connection.Top = Mathf.Max(path1.Top, path2.Top);
            }
            else
            {
                connection.Bottom = path1.Top - ActualHorizontalPathThickness;
                connection.Top = path2.Bottom + ActualHorizontalPathThickness;
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
					rect.Left = div.Bound.Left - ActualVerticalPathThickness;
					rect.Right = div.Room.Left + ActualVerticalPathThickness;
				}
				else
				{
                    // 2つの通路パーツが同じX座標まで伸びるようにするため、
                    // 伸ばすのは片方だけにする(こちらは伸ばさない)
					rect.Left = div.Room.Right - ActualVerticalPathThickness;
					rect.Right = div.Bound.Right;
				}
				var pos = GetRandomInRange(
					div.Room.Bottom,
					div.Room.Top - ActualHorizontalPathThickness);
				rect.Bottom = pos;
				rect.Top = pos + ActualHorizontalPathThickness;
			}
			else
			{
				if (isTopDiv)
				{
					rect.Bottom = div.Bound.Bottom - ActualHorizontalPathThickness;
					rect.Top = div.Room.Bottom + ActualHorizontalPathThickness;
				}
				else
				{
					// 2つの通路パーツが同じY座標まで伸びるようにするため、
					// 伸ばすのは片方だけにする(こちらは伸ばさない)
					rect.Bottom = div.Room.Top - ActualHorizontalPathThickness;
					rect.Top = div.Bound.Top;
				}
				var pos = GetRandomInRange(
					div.Room.Left,
					div.Room.Right - ActualVerticalPathThickness);
				rect.Left = pos;
				rect.Right = pos + ActualVerticalPathThickness;
			}

			return rect;
		}

        /// <summary>
        /// 指定した通路が伸びている元の部屋と同じ部屋から伸びている通路があれば、通路の開始点を一つにまとめます。
        /// </summary>
        /// <param name="connections">すでに生成されている部屋の接続情報のコレクション。</param>
        /// <param name="selectDivision">部屋の接続情報から判定対象の部屋を選ぶデリゲート。</param>
        /// <param name="division">判定対象の通路が伸びている元の部屋。</param>
        /// <param name="selectRect">同じ部屋から伸びていた通路から、まとめる先の矩形範囲を選ぶデリゲート。</param>
        /// <param name="path">まとめる対象の通路。</param>
        /// <param name="horizontal"><c>true</c> なら水平方向に、<c>false</c> なら垂直方向の通路とみなしてまとめます。</param>
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
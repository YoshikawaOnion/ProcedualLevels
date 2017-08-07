using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProcedualLevels.Models
{
    public class MapData
    {
        public List<MapDivision> Divisions { get; private set; }
        public Vector2 StartLocation { get; set; }
        public Vector2 GoalLocation { get; set; }
        public List<Enemy> Enemies { get; private set; }
        public List<MapPlatform> Platforms { get; private set; }
        public List<Spawner> Spawners { get; private set; }
        public List<Vector2> CollisionBlocks { get; private set; }

        public MapData()
        {
            Divisions = new List<MapDivision>();
            Enemies = new List<Enemy>();
            Platforms = new List<MapPlatform>();
            Spawners = new List<Spawner>();
            CollisionBlocks = new List<Vector2>();
        }

        public bool IsRoom(int left, int bottom)
        {
            foreach (var item in Divisions)
            {
                if (IsInRectanble(left, bottom, item.Room.ReduceOnEdge(1)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPath(int left, int bottom)
        {
            foreach (var item in Divisions)
            {
                foreach (var connection in item.Connections)
                {
                    foreach (var path in connection.Path.GetRooms())
                    {
                        if (IsInRectanble(left, bottom, path.ReduceOnEdge(1)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsInRectanble(int left, int bottom, MapRectangle room)
        {
            if (left >= room.Left && bottom >= room.Bottom
               && left < room.Right && bottom < room.Top)
            {
                return true;
            }
            return false;
        }
    }
}
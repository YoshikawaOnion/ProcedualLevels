using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class RandomRoomGenAsset : ScriptableObject
    {
        public int ChildBoundMinSize;
        public int ParentBoundMinSize;
        public int MarginSize;
        public int RoomMinSize;
        public int RoomMaxSize;
    }
}

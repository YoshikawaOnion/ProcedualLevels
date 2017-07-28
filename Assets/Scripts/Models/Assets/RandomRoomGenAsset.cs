using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class RandomRoomGenAsset : ScriptableObject
    {
        public int ChildBoundMinSize;
        public int ParentBoundMinSize;
        public int MarginSize;
        public int BoundMaxWidth;
        public int BoundMaxHeight;
        public int RoomMinWidth;
        public int RoomMaxWidth;
        public int RoomMinHeight;
        public int RoomMaxHeight;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ist;

namespace ProcedualLevels.Views
{
    public class MapTipRenderer : MonoBehaviour
    {
        [SerializeField]
        private BatchRenderer wallRendererPrefab;
        [SerializeField]
        private BatchRenderer roomRendererPrefab;

		private List<Vector3> WallLocations { get; set; }
		private List<Vector3> RoomLocations { get; set; }
        private BatchRenderer WallRenderer { get; set; }
        private BatchRenderer RoomRenderer { get; set; }

        private void Awake()
		{
			RoomLocations = new List<Vector3>();
			WallLocations = new List<Vector3>();
            WallRenderer = Instantiate(wallRendererPrefab);
            RoomRenderer = Instantiate(roomRendererPrefab);
        }

        private void OnDestroy()
        {
            WallLocations = null;
            RoomLocations = null;
            WallRenderer = null;
            RoomRenderer = null;
        }

        public void Initialize(MapData map)
        {
            var asset = Resources.Load<Models.DungeonGenAsset>
                                 ("Assets/DungeonGenAsset");
            var width = asset.WorldWidth;
            var height = asset.WorldHeight;
            var offset = new Vector3(0.5f, 0.5f, 0);
            for (int i = -height / 2; i < height / 2; i++)
            {
                for (int j = -width / 2; j < width / 2; j++)
                {
                    if (map.IsRoom(j, i) || map.IsPath(j, i))
                    {
                        RoomLocations.Add(new Vector3(j, i, 0) + offset);
                    }
                    else
					{
						WallLocations.Add(new Vector3(j, i, 0) + offset);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var p in WallLocations)
            {
                WallRenderer.AddInstanceT(p);
            }
            foreach (var p in RoomLocations)
            {
                RoomRenderer.AddInstanceT(p);
            }
        }
    }
}
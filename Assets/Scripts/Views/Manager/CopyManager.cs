using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Ist;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class CopyManager : MonoBehaviour
    {
        private List<Copy> Copies { get; set; }
        private GameObject CopyPrefab { get; set; }
        private BatchRenderer Renderer { get; set; }

        void Start()
		{
			Copies = new List<Copy>();
			CopyPrefab = Resources.Load<GameObject>("Prefabs/Character/Copy");
			Renderer = GetComponent<BatchRenderer>();
        }

        public GameObject CreateCopy(Vector3 position, Vector3 velocity)
        {
            var obj = Instantiate(CopyPrefab);
            var copy = obj.GetComponent<Copy>();

			obj.transform.position = position;
			Copies.Add(copy);
			copy.Initialize();
			copy.OnVanish.Subscribe(x => Copies.Remove(copy));

			var rigidbody = obj.GetComponent<Rigidbody2D>();
            rigidbody.velocity = velocity;

            return obj.gameObject;
        }

        private void Update()
        {
            foreach (var copy in Copies)
            {
				var uv4 = new Vector4(1, 0.33f, copy.Uv.x / 16, (48 - copy.Uv.y - 16) / 48);
                Renderer.AddInstanceTRU(copy.transform.position, Quaternion.identity, uv4);
                //Renderer.AddInstanceT(copy.transform.position);
            }
        }
    }
}
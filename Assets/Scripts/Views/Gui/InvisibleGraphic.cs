using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ProcedualLevels.Views
{
    public class InvisibleGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
        }

#if UNITY_EDITOR
        class GraphicCastEditor : Editor
        {
            public override void OnInspectorGUI()
            {
            }
        }
#endif
    }
}
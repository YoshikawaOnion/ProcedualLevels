using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
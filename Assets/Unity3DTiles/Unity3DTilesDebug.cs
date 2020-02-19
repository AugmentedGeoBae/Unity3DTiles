using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3DTiles
{
    public class Unity3DTilesDebug
    {
        public static int Layer = 5;

        public static GameObject go;

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            //works only in editor with gizmos enabled
            //UnityEngine.Debug.DrawLine(start, end, color);

            GetComponent().DrawLine(start, end, color);
        }

        private static DebugDrawBehaviour GetComponent()
        {
            if (go == null)
            {
                go = new GameObject("DebugDraw");
                go.AddComponent<DebugDrawBehaviour>();
            }
            return go.GetComponent<DebugDrawBehaviour>();
        }
    }

    public class DebugDrawBehaviour : MonoBehaviour
    {
        private struct Line
        {
            public Vector3 start;
            public Vector3 end;
            public Color color;
        }
        private List<Line> lines = new List<Line>();
        private Mesh lineMesh;
        private Material lineMaterial;
        private List<Vector3> lineVerts = new List<Vector3>();
        private List<Color> lineColors = new List<Color>();
        private List<int> lineIndices = new List<int>();

        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            lines.Add(new Line() { start = start, end = end, color = color });
        }

        public void Awake()
        {
            lineMesh = new Mesh();
            lineMesh.MarkDynamic();
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        }

		public void LateUpdate()
		{
            if (lines.Count > 0)
            {
                lineVerts.Clear();
                lineColors.Clear();
                lineIndices.Clear();

                foreach (var line in lines)
                {
                    lineVerts.Add(line.start);
                    lineColors.Add(line.color);
                    lineIndices.Add(lineVerts.Count - 1);

                    lineVerts.Add(line.end);
                    lineColors.Add(line.color);
                    lineIndices.Add(lineVerts.Count - 1);
                }
                lines.Clear();

                lineMesh.Clear();

				lineMesh.SetVertices(lineVerts);
				lineMesh.SetColors(lineColors);

#if UNITY_2019_3_OR_NEWER                
				lineMesh.SetIndices(lineIndices, MeshTopology.Lines, submesh: 0);
#else
				lineMesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, submesh: 0);
#endif

                Graphics.DrawMesh(lineMesh, Vector3.zero, Quaternion.identity, lineMaterial, Unity3DTilesDebug.Layer);

                //don't clear lineMesh here because DrawMesh() is lazy
            }
        }

		public void OnDestroy()
		{
            Unity3DTilesDebug.go = null;
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace boreal.engine
{
    internal static class PolygonHelper
    {

        public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
        {
            Console.WriteLine("Triangulate ... " + vertices.Length);
            errorMessage = string.Empty;
            triangles = null;

            if(vertices == null) { errorMessage = "The vertices Vector2 array is null"; return false; }
            else if(vertices.Length < 3) { errorMessage = "The vertices Vector2 array needs to have at least 3 vertices."; return false; }

            List<int> indexList = new List<int>();
            for (int i = 0; i < vertices.Length; i++)
            {
                indexList.Add(i);
            }

            int totalTriangleCount = vertices.Length - 2;
            int totalTriangleIndexCount = totalTriangleCount * 3;

            triangles = new int[totalTriangleIndexCount];
            int triangleIndexCount = 0;

            while (indexList.Count > 3)
            {
                Console.WriteLine(indexList.Count);
                for (int i = 0; i < indexList.Count; i++)
                {
                    int a = indexList[i];
                    int b = Utils.GetItem(indexList, i - 1);
                    int c = Utils.GetItem(indexList, i + 1);

                    Vector2 va = vertices[a];
                    Vector2 vb = vertices[b];
                    Vector2 vc = vertices[c];

                    Vector2 va_to_vb = vb - va;
                    Vector2 va_to_vc = vc - va;

                    // Is ear test vertex convex?
                    if (Vector2.Cross(va_to_vb, va_to_vc) < 0f)
                    {
                        continue;
                    }

                    bool isEar = true;

                    // Does test ear contain any polygon vertices?
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if (j == a || j == b || j == c)
                        {
                            continue;
                        }

                        Vector2 p = vertices[j];

                        if (IsPointInTriangle(p, vb, va, vc))
                        {
                            isEar = false;
                            break;
                        }
                    }

                    if (isEar)
                    {
                        triangles[triangleIndexCount++] = b;
                        triangles[triangleIndexCount++] = a;
                        triangles[triangleIndexCount++] = c;

                        indexList.RemoveAt(i);
                        break;
                    }
                }
            }

            triangles[triangleIndexCount++] = indexList[0];
            triangles[triangleIndexCount++] = indexList[1];
            triangles[triangleIndexCount++] = indexList[2];

            return true;
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;

            Vector2 ap = p - a;
            Vector2 bp = p - b;
            Vector2 cp = p - c;

            float cross1 = Vector2.Cross(ab, ap);
            float cross2 = Vector2.Cross(bc, bp);
            float cross3 = Vector2.Cross(ca, cp);

            if (cross1 > 0f || cross2 > 0f || cross3 > 0f)
            {
                return false;
            }

            return true;
        }
    }
}

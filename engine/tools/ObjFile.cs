using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boreal.engine.tools
{
    public static class ObjFile
    {

        public static Vector3[] GetVerticesFromObjFile(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException();

            Vector3[] vertices = new Vector3[0];
            string[] data = File.ReadAllLines(filePath);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].StartsWith("v "))
                {
                    string[] axes = data[i].Replace(".", ",").Remove(0, 2).Split(' ');
                    if (axes.Length <= 2) break;
                    Array.Resize(ref vertices, vertices.Length + 1);

                    vertices[vertices.Length - 1] = new Vector3(float.Parse(axes[0]), float.Parse(axes[1]), float.Parse(axes[2]));
                }
                //else if (i != 0 && data[i].StartsWith("# ")) break; //end of the vertices
            }

            return vertices;
        }

    }
}

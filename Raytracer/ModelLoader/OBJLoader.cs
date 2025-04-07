using OpenTK.Mathematics;
using Raytracer.Materials;
using Raytracer.Primitives;

namespace Raytracer.ModelLoader;

public class OBJLoader
{
    public static Model LoadModel(string filename)
    {
        string modelSource = File.ReadAllText(filename);
        string[] lines = modelSource.Split('\n');
        List<Vector3> vertices = new List<Vector3>();
        List<Triangle> triangles = new List<Triangle>();
        foreach (string line in lines)
        {
            string[] tokens = line.Split(' ');
            if (tokens[0] == "v")
            {
                var vA = Array.ConvertAll(tokens[1..], el =>
                {
                    return float.Parse(el.Replace(".", ","));
                });
                var vertex = new Vector3(vA[0], vA[1], vA[2]);
                vertices.Add(vertex);   
            }

            if (tokens[0] == "f")
            {
                var vT = (from t in tokens[1..]
                    select t.Split("/")).ToArray();
                Vector3[] tV = new Vector3[vT.Length];
                for (int i = 0; i < 3; i++)
                {
                    tV[i] = vertices[int.Parse(vT[i][0]) - 1];
                }
                triangles.Add(new Triangle(tV[0], tV[1], tV[2], new Diffuse(Vector3.One)));
            }
        }
        
        return new Model(triangles.ToArray());
    }
}
using Raytracer.Primitives;

namespace Raytracer.ModelLoader;

public class Model(Triangle[] triangles)
{
    public Triangle[] Triangles = triangles;
}
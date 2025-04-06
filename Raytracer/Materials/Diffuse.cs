using OpenTK.Mathematics;

namespace Raytracer.Materials;

public class Diffuse(Vector3 color) : IMaterial
{
    public Vector3 Color { get; set; } =  color;
}
using OpenTK.Mathematics;

namespace Raytracer.Materials;

public abstract class AbstractMaterial(Vector3 color)
{
    public Vector3 Color { get; set; } = color;
}
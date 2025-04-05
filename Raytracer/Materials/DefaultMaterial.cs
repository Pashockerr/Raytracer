using OpenTK.Mathematics;

namespace Raytracer.Materials;

public class DefaultMaterial(Vector3 color) : IMaterial
{
    public Vector3 Color = color;
}
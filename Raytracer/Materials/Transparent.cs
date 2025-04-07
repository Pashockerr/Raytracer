using OpenTK.Mathematics;

namespace Raytracer.Materials;

public class Transparent(Vector3 color, float n) : AbstractMaterial(color)   // n - refraction coefficient
{
    public float N { get; set; } = n;
}
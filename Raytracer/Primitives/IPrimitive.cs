using OpenTK.Mathematics;

namespace Raytracer.Primitives;

public interface IPrimitive
{
    public HitResult Intersect(Vector3 origin, Vector3 direction);
}
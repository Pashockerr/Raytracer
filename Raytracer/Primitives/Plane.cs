using OpenTK.Mathematics;

namespace Raytracer.Primitives;

public class Plane(Vector3 position, Vector3 normal, Vector3 color) : IPrimitive
{
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = new HitResult(false,  Vector3.Zero);
        
        
        
        return result;
    }
}
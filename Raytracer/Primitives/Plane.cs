using OpenTK.Mathematics;

namespace Raytracer.Primitives;

public class Plane(Vector3 position, Vector3 normal, Vector3 color) : IPrimitive
{
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = new HitResult(true,  new Vector3(1.0f, 0.0f, 0.0f));
        
        
        
        return result;
    }
}
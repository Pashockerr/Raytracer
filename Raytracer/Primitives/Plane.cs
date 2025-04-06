using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer.Primitives;

public class Plane(Vector3 position, Vector3 normal, IMaterial material) : IPrimitive
{
    private Vector3 _position = position;
    private Vector3 _normal = normal;
    private IMaterial _material = material;
    
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = HitResult.Skybox;

        var numerator = Vector3.Dot(_position - origin, _normal);
        var denominator = Vector3.Dot(direction, _normal);
        var t = numerator / denominator;
        if (t >= 0)
        {
            result.IsHit = true;
            result.Distance = t;
            result.Material = _material;
            result.HitPoint = origin + t * direction;
            result.Normal = _normal;
        }
        
        return result;
    }
}
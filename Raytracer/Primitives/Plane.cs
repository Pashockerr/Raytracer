using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer.Primitives;

public class Plane(Vector3 position, Vector3 normal, AbstractMaterial abstractMaterial) : IPrimitive
{
    private Vector3 _position = position;
    private Vector3 _normal = normal;
    private AbstractMaterial _abstractMaterial = abstractMaterial;
    
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = HitResult.Skybox;

        var numerator = Vector3.Dot(_position - origin, _normal);
        var denominator = Vector3.Dot(direction, _normal);
        var t = numerator / denominator;
        if (t >= 0)
        {
            result.IsHit = true;
            result.Distance1 = t;
            result.AbstractMaterial = _abstractMaterial;
            result.HitPoint1 = origin + t * direction;
            result.Normal = _normal;
            result.HitDirection = direction;
        }
        
        return result;
    }
}
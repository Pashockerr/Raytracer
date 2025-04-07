using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer.Primitives;

public class Sphere(Vector3 position, float radius, AbstractMaterial abstractMaterial) : IPrimitive
{
    private Vector3 _position =  position;
    private float _radius = radius;
    private AbstractMaterial _abstractMaterial = abstractMaterial;
    
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = HitResult.Skybox;

        var a = 1.0f;
        var b = Vector3.Dot(direction * 2, origin - _position);
        var c = Vector3.Dot(origin - _position, origin - _position) - (float)Math.Pow(_radius, 2.0);
        
        var discriminant = b * b - 4 * a * c;

        if (discriminant == 0)
        {
            var t = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
            result.Distance1 = t;
            result.AbstractMaterial = _abstractMaterial;
            result.HitPoint1 = origin + t * direction;
            var normal = Vector3.Normalize(result.HitPoint1 - _position);
            result.Normal = normal;
            result.HitDirection = direction;
        }

        if (discriminant > 0)
        {
            var t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
            result.Distance1 = Math.Min(t1, t2);
            result.HitPoint1 = origin + result.Distance1 * direction;
            result.AbstractMaterial = _abstractMaterial;
            var normal = Vector3.Normalize(result.HitPoint1 - _position);
            result.Normal = normal;
            result.HitDirection = direction;
        }
        
        return result;
    }
}
using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer.Primitives;

public class Sphere(Vector3 position, float radius, IMaterial material) : IPrimitive
{
    private Vector3 _position =  position;
    private float _radius = radius;
    private IMaterial _material = material;
    
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
            result.Distance = t;
            result.Material = _material;
        }

        if (discriminant > 0)
        {
            var t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
            var t2 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
            result.Distance = Math.Min(t1, t2);
            result.Material = _material;
        }
        
        return result;
    }
}
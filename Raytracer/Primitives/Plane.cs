using OpenTK.Mathematics;

namespace Raytracer.Primitives;

public class Plane(Vector3 position, Vector3 normal, Vector3 color) : IPrimitive
{
    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Vector3 Color = color;
    
    public const float Epsilon = 0.0001f;
    
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        HitResult result = new HitResult(false, new Vector3(0.0f, 0.0f, 0.0f), float.PositiveInfinity);

        var numerator = Vector3.Dot(Position - origin, Normal);
        var denominator = Vector3.Dot(direction, Normal);
        var t = numerator / denominator;
        if (t >= 0)
        {
            result.IsHit = true;
            result.Distance = t;
            result.Color = Color;
        }
        
        return result;
    }
}
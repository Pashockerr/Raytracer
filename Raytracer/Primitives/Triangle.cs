using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer.Primitives;

public class Triangle(Vector3 a, Vector3 b, Vector3 c, AbstractMaterial abstractMaterial) : IPrimitive
{
    private Vector3 _a = a;
    private Vector3 _b = b;
    private Vector3 _c = c;
    private AbstractMaterial _abstractMaterial = abstractMaterial;
    public HitResult Intersect(Vector3 origin, Vector3 direction)
    {
        var result = HitResult.Skybox;
        var normal = Vector3.Cross(_a - _b, _a - _c).Normalized();  // Triangle plane normal
        
        var numerator = Vector3.Dot(_a - origin, normal);
        var denominator = Vector3.Dot(direction, normal);
        var t = numerator / denominator;
        if (t >= 0)
        {
            var hitPoint = origin + t * direction;

            var ab = _b - a;
            var ca = _a - _c;
            var bc = _c - _b;

            var ah = hitPoint - _a;
            var bh = hitPoint - _b;
            var ch = hitPoint - _c;

            if (Vector3.Dot(normal, Vector3.Cross(ab, ah)) > 0 &&
                Vector3.Dot(normal, Vector3.Cross(ca, ch)) > 0 &&
                Vector3.Dot(normal, Vector3.Cross(bc, bh)) > 0)
            {
                result.IsHit = true;
                result.Distance1 = t;
                result.AbstractMaterial = abstractMaterial;
                result.HitPoint1 = origin + t * direction;
                result.Normal = normal;
                result.HitDirection = direction;
            }
        }
        
        return result;
    }
}
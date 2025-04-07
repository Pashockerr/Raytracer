using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer;

public struct HitResult(bool isHit, AbstractMaterial abstractMaterial, float distance1, Vector3 hitPoint1, Vector3 hitNormal, Vector3 hitDirection)
{
    public bool IsHit = isHit;
    public AbstractMaterial AbstractMaterial = abstractMaterial;
    public float Distance1 = distance1;
    public float Distance2 = 0;     // For sphere intersections
    public Vector3 HitPoint1 = hitPoint1;
    public Vector3 HitPoint2 = Vector3.Zero;   // For sphere intersections
    public Vector3 Normal = hitNormal;
    public Vector3 HitDirection = hitDirection;
    
    public static HitResult Skybox = new HitResult(false, new DefaultAbstractMaterial(Vector3.Zero), float.MaxValue,  Vector3.Zero, Vector3.Zero, Vector3.Zero);
}
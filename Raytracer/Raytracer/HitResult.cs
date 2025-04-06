using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer;

public struct HitResult(bool isHit, IMaterial material, float distance, Vector3 hitPoint,  Vector3 hitNormal)
{
    public bool IsHit = isHit;
    public IMaterial Material = material;
    public float Distance = distance;
    public Vector3 HitPoint = hitPoint;
    public Vector3 Normal = hitNormal;
    
    public static HitResult Skybox = new HitResult(false, new DefaultMaterial(Vector3.Zero), float.MaxValue,  Vector3.Zero, Vector3.Zero);
}
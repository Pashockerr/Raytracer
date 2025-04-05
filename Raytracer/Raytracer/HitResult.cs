using OpenTK.Mathematics;
using Raytracer.Materials;

namespace Raytracer;

public struct HitResult(bool isHit, IMaterial material, float distance)
{
    public bool IsHit = isHit;
    public IMaterial Material = material;
    public float Distance = distance;
    
    public static HitResult Skybox = new HitResult(false, new DefaultMaterial(Vector3.Zero), float.MaxValue);
}
using OpenTK.Mathematics;

namespace Raytracer;

public struct HitResult(bool isHit, Vector3 color, float distance)
{
    public bool IsHit = isHit;
    public Vector3 Color = color;
    public float Distance = distance;
    
    public static HitResult Skybox = new HitResult(false, Vector3.Zero, float.MaxValue);
}
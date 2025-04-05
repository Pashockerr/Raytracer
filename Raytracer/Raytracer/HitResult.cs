using OpenTK.Mathematics;

namespace Raytracer;

public struct HitResult(bool isHit, Vector3 color)
{
    public bool IsHit = isHit;
    public Vector3 Color = color;
    public float Distance;
}
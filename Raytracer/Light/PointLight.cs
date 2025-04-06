using OpenTK.Mathematics;

namespace Raytracer.Light;

public class PointLight(Vector3 position, Vector3 color, float power) : ILightSource
{
    private const float K = 683.0f; // Kd * sr / w
    private float _power = power;
    public Vector3 Color { get; private set; } = color;
    public Vector3 Position {  get; private set; } =  position;

    public float GetIllumination(float distance, float angle)   // Angle between normal and light direction vector
    {
        return ((K * _power) / distance / distance) * (float)Math.Cos(angle);
    }
}
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using Raytracer.Light;
using Raytracer.Materials;

namespace Raytracer;

public class Raytracer
{
    private readonly int _width;
    private readonly int _height;
    private readonly float _viewportDistance;
    private readonly float _viewportWidth;
    private readonly float _viewportHeight;
    private readonly Scene _scene;
    private readonly float _horizontalRayStep;
    private readonly float _verticalRayStep;
    private readonly Vector3 _viewportCenter;
    private readonly Vector3 _cameraPosition;

    private const float MinIllumination = 0.0003f;
    private const float MaxIllumination = 130_000.0f;
    private const int MaxRecursionDepth = 3;
    
    // No rotation support. Camera faced towards Z+ axis.
    public Raytracer(int width, int height, float viewportDistance, float viewportWidth, float viewportHeight,
        Scene scene, Vector3 cameraPosition)
    {
        _width = width;
        _height = height;
        _viewportDistance = viewportDistance;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        _scene = scene;
        if (_scene.Primitives.Length == 0)
        {
            throw new ArgumentException("Scene should have primitives");
        }
        
        _horizontalRayStep = viewportWidth / _width;
        _verticalRayStep = viewportHeight / _height;
        _cameraPosition = cameraPosition;
        
        _viewportCenter = cameraPosition + new Vector3(0, 0, _viewportDistance);
    }
    
    public void Render(byte[] texture, int startPixel, int pixelAmount)
    {
        if (texture.Length != _width * _height * 3)
        {
            throw new Exception($"Buffer size mismatch. Buffer size must be {_width * _height * 3} bytes length.");
        }

        int totalPixels = _width * _height;
        int pixelCount = 0;
        for (int pI = startPixel; pI < startPixel + pixelAmount; pI++)
        {
            int dPY = (int)Math.Floor((double)pI / _width);
            int dPX = pI - dPY * _width;
            // Compute canvas deltas based on pixels deltas
            float dVX = -_viewportWidth / 2 + dPX * _horizontalRayStep;
            float dVY = _viewportHeight / 2 - dPY * _verticalRayStep;
            var viewportPoint = _viewportCenter + new Vector3(dVX, dVY, 0);
            var direction = viewportPoint - _cameraPosition;    // Direction vector from camera center to current viewport point
            float minDistance = float.MaxValue;
            HitResult closestHitResult = HitResult.Skybox;
            for (int primI = 0; primI < _scene.Primitives.Length; primI++)   // Find the closest hit
            {
                var hitResult = _scene.Primitives[primI].Intersect(_cameraPosition, direction.Normalized());
                if (hitResult.Distance1 < minDistance)
                {
                    minDistance = hitResult.Distance1;
                    closestHitResult = hitResult;
                }
            }
            
            Vector3 pointColor = Trace(_cameraPosition, direction.Normalized(), MaxRecursionDepth);
            byte[] pixelColor = ConvertColor(pointColor);

            // Put collided color into texture
            texture[(dPY * _width + dPX) * 3] = pixelColor[0];
            texture[(dPY * _width + dPX) * 3 + 1] = pixelColor[1];
            texture[(dPY * _width + dPX) * 3 + 2] = pixelColor[2];
            pixelCount++;
        }
    }

    private Vector3 Trace(Vector3 origin, Vector3 direction, int steps) // TODO : do recursive light reflection
    {
        float minDistance = float.MaxValue;
        HitResult closestHitResult = HitResult.Skybox;
        for (int primI = 0; primI < _scene.Primitives.Length; primI++)   // Find the closest hit
        {
            var hitResult = _scene.Primitives[primI].Intersect(_cameraPosition, direction.Normalized());
            if (hitResult.Distance1 < minDistance)
            {
                minDistance = hitResult.Distance1;
                closestHitResult = hitResult;
            }
        }
        Vector3 pointColor = Vector3.Zero;
        if (steps == 0 || !closestHitResult.IsHit)
        {
            pointColor = ComputeColor(closestHitResult);
        }
        else
        {
            pointColor += Trace(closestHitResult.HitPoint1,
                Reflect(closestHitResult.HitDirection, closestHitResult.Normal), steps - 1);
        }
        return pointColor;
    }

    // Compute hit point visible color based on material and lighting
    private Vector3 ComputeColor(HitResult hitResult)
    {
        var resultColor = (HitResult.Skybox.AbstractMaterial as DefaultAbstractMaterial)!.Color;

        // With 100% diffuse material
        if (hitResult.AbstractMaterial is Diffuse diffuse)
        {
            var color = diffuse.Color;
            float illumination = 0;
            Vector3 brightness = Vector3.Zero;
            foreach(var lightSource in _scene.LightSources)
            {
                if (lightSource is PointLight pointLight)
                {
                    var lightDirection = (hitResult.HitPoint1 - pointLight.Position).Normalized();
                    var angle = (float)Math.Acos(Vector3.Dot(lightDirection, hitResult.Normal)) - (float)Math.PI;
                    var distance = Vector3.Distance(pointLight.Position, hitResult.HitPoint1);
                    illumination += pointLight.GetIllumination(distance, angle);
                    
                    float brightnessMono = IlluminationToBrightness(illumination) * (float)Math.Max(Math.Cos(angle), 0);
                    brightness += pointLight.Color * color *  brightnessMono;
                }
            }
            resultColor = brightness;
        }
        
        // With color only default material
        if(hitResult.AbstractMaterial is DefaultAbstractMaterial defaultMaterial)
        {
            resultColor = defaultMaterial.Color;
        }

        // With refractive sphere(only 1 refraction)
        if (hitResult.AbstractMaterial is Transparent transparent && hitResult.HitPoint2 != Vector3.Zero && hitResult.Distance2 != 0)   // Only for sphere double intersections
        {
            var alpha = (float)Math.Acos(Vector3.Dot((hitResult.HitDirection * -1), hitResult.Normal));
            var gamma = (Math.Asin((1 * Math.Sin(alpha) / transparent.N)));     // 1 is air refraction coefficient
            // TODO : rotate inner vector
        }
        
        return resultColor;
    }
    
    private static byte[] ConvertColor(Vector3 color)
    {
        byte[] result = new byte[3];
        result[0] = (byte)(color.X * 255);
        result[1] = (byte)(color.Y * 255);
        result[2] = (byte)(color.Z * 255);
        return result;
    }

    private float IlluminationToBrightness(float illumination)
    {
        if (illumination < MinIllumination)
        {
            return 0;
        }

        if (illumination > MaxIllumination)
        {
            return 1;
        }
        
        return (illumination - MinIllumination) / (MaxIllumination - MinIllumination);
    }

    private Vector3 Reflect(Vector3 hitVector, Vector3 normal)  // Reflects vector from surface with given normal vector
    {
        return 2 * normal + hitVector.Normalized();
    }
}
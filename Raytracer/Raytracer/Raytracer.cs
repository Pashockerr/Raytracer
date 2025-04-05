using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
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
    
    public void Render(byte[] texture)
    {
        if (texture.Length != _width * _height * 3)
        {
            throw new Exception($"Buffer size mismatch. Buffer size must be {_width * _height * 3} bytes length.");
        }

        for (int dPY = 0; dPY < _height; dPY++)
        {
            for (int dPX = 0; dPX < _width; dPX++)
            {
                // Compute canvas deltas based on pixels deltas
                float dVX = -_viewportWidth / 2 + dPX * _horizontalRayStep;
                float dVY = _viewportHeight / 2 - dPY * _verticalRayStep;
                var viewportPoint = _viewportCenter + new Vector3(dVX, dVY, 0);
                var direction = viewportPoint - _cameraPosition;    // Direction vector from camera center to current viewport point
                float minDistance = float.MaxValue;
                HitResult closestHitResult = HitResult.Skybox;
                for (int pI = 0; pI < _scene.Primitives.Length; pI++)   // Find the closest hit
                {
                    var hitResult = _scene.Primitives[pI].Intersect(_cameraPosition, direction.Normalized());
                    if (hitResult.Distance < minDistance)
                    {
                        minDistance = hitResult.Distance;
                        closestHitResult = hitResult;
                    }
                }
                
                IMaterial collidedMaterial = closestHitResult.Material;
                Vector3 pointColor = Vector3.Zero;
                if (collidedMaterial is DefaultMaterial material)
                {
                    pointColor = material.Color;
                }
                
                byte[] pixelColor = ConvertColor(pointColor);

                // Put collided color into texture
                texture[(dPY * _width + dPX) * 3] = pixelColor[0];
                texture[(dPY * _width + dPX) * 3 + 1] = pixelColor[1];
                texture[(dPY * _width + dPX) * 3 + 2] = pixelColor[2];
            }
        }
    }

    private static byte[] ConvertColor(Vector3 color)
    {
        byte[] result = new byte[3];
        result[0] = (byte)(color.X * 255);
        result[1] = (byte)(color.Y * 255);
        result[2] = (byte)(color.Z * 255);
        return result;
    }
}
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;

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

    private HitResult[] _hits;
    
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
        
        _horizontalRayStep = viewportWidth / _width;
        _verticalRayStep = viewportHeight / _height;
        _cameraPosition = cameraPosition;
        
        _viewportCenter = cameraPosition + new Vector3(0, 0, _viewportDistance);
        
        _hits = new HitResult[scene.Primitives.Length];
    }
    
    public void Render(byte[] texture)
    {
        if (texture.Length != _width * _height * 3)
        {
            throw new Exception($"Buffer size mismatch. Buffer size must be {_width * _height * 3} bytes length.");
        }

        for (float dVX = -_viewportWidth / 2; dVX < _viewportWidth / 2; dVX += _horizontalRayStep)
        {
            for (float dVY = _viewportHeight / 2; dVY > -_viewportHeight / 2; dVY -= _verticalRayStep)
            {
                var viewportPoint = _viewportCenter + new Vector3(dVX, dVY, 0);
                var direction = viewportPoint - _cameraPosition;    // Direction vector from camera center to current viewport point
                float minDistance = float.MaxValue;
                HitResult? closestHitResult = null;
                for (int pI = 0; pI < _hits.Length; pI++)   // Find closest hit
                {
                    var hitResult = _scene.Primitives[pI].Intersect(_cameraPosition, direction);
                    if (hitResult.Distance < minDistance)
                    {
                        minDistance = hitResult.Distance;
                        closestHitResult = hitResult;
                    }
                }
                
                // TODO : texture rendering
            }
        }
    }
}
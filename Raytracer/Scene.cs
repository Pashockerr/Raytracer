using Raytracer.Light;
using Raytracer.Primitives;

namespace Raytracer;

public class Scene(IPrimitive[] primitives, ILightSource[] lightSources)
{
    public readonly IPrimitive[] Primitives = primitives;
    public readonly ILightSource[] LightSources = lightSources;
}
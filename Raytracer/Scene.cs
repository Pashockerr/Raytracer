using Raytracer.Primitives;

namespace Raytracer;

public class Scene(IPrimitive[] primitives)
{
    public readonly IPrimitive[] Primitives = primitives;
}
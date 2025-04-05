using Raytracer.Primitives;

namespace Raytracer;

public class Scene(IPrimitive[] primitives)
{
    public IPrimitive[] Primitives = primitives;
}
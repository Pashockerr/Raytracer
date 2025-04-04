namespace Raytracer;

class Program
{
    static void Main(string[] args)
    {
        using (Application application = new Application(800, 600, "Raytracer"))
        {
            application.Run();
        }
    }
}
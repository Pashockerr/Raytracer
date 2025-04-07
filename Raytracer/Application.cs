using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Raytracer.Light;
using Raytracer.Materials;
using Raytracer.ModelLoader;
using Raytracer.Primitives;

namespace Raytracer;

public class Application : GameWindow
{
    private float[] _vertices =
    {
        -1.0f, -1.0f, 1.0f,     1.0f, 0.0f, 1.0f,       0.0f, 1.0f,
        -1.0f, 1.0f, 1.0f,      1.0f, 0.0f, 1.0f,       0.0f, 0.0f,
        1.0f, 1.0f, 1.0f,       1.0f, 0.0f, 1.0f,       1.0f, 0.0f,
        1.0f, -1.0f, 1.0f,        1.0f, 1.0f, 1.0f,       1.0f, 1.0f,
    };

    private int[] _indices =
    {
        0, 1, 2,
        0, 2, 3
    };
    
    private int _vbo, _vao, _ebo, _textureId;

    private byte[] _textureData = new byte[800 * 600 * 3];

    private Shader _shader;
    private Raytracer _raytracer;
    private Scene _scene;
    
    private Thread[] _renderThreads =  new Thread[4];
    
    public Application(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = (width, height),
            Title = title
        })
    {
        _textureId = GL.GenTexture();
        
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,_indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
        Console.WriteLine("Compiling shaders...");
        _shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
        Console.WriteLine("Ended shader compilation.");
        
        Console.WriteLine("Initializing raytracer...");
        List<IPrimitive> primitives = new List<IPrimitive>();
        primitives.Add(new Plane(new Vector3(0, -3, 0), new Vector3(0, 1.0f, 0), new Diffuse(new Vector3(1.0f, 1.0f, 1.0f))));
        primitives.Add(new Sphere(new Vector3(0, -.25f, 2.0f), .5f, new Diffuse(new Vector3(1.0f, 1.0f, 1.0f))));
        
        Console.WriteLine("Loading model...");
        Model monke = OBJLoader.LoadModel("./Models/monke.obj");
        primitives.AddRange(monke.Triangles);
        Console.WriteLine("Loaded model.");
        
        ILightSource[] lights = new ILightSource[1];
        lights[0] = new PointLight(new Vector3(0, 3.0f, -4), new Vector3(1.0f, 1.0f, 1.0f), 1500);
        // lights[1] = new PointLight(new Vector3(1, 2.0f, -4), new Vector3(1.0f, 1.0f, 1.0f), 500); 
        // lights[2] = new PointLight(new Vector3(-1, 2.0f, -4), new Vector3(1.0f, 1.0f, 1.0f), 500); 
        _scene = new Scene(primitives.ToArray(), lights);
        _raytracer = new Raytracer(800, 600, 0.5f, 0.8f, 0.6f, _scene, new Vector3(0, -.5f, -5));
        Console.WriteLine("Raytracer initialized.");

        for (int i = 0; i < _textureData.Length; i++)
        {
            _textureData[i] = 255;
        }
        
        _renderThreads[0] = new Thread(() => _raytracer.Render(_textureData, 0, 800 * 600 / 4));
        _renderThreads[1] = new Thread(() => _raytracer.Render(_textureData, 800 * 600 / 4, 800 * 600 / 4));
        _renderThreads[2] = new Thread(() => _raytracer.Render(_textureData, 800 * 600 / 2, 800 * 600 / 4));
        _renderThreads[3] = new Thread(() => _raytracer.Render(_textureData, 800 * 600 * 3 / 4, 800 * 600 / 4));

        foreach (var renderThread in _renderThreads)
        {
            renderThread.Start();
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            foreach (var renderThread in _renderThreads)
            {
                renderThread.Interrupt();
            }    
            Close();
        }

        // var sw = Stopwatch.StartNew();
        // _raytracer.Render(_textureData);
        // sw.Stop();
        // int renderTime = sw.Elapsed.Milliseconds;
        // Console.WriteLine("Render time: " + renderTime);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        GL.UseProgram(_shader.Handle);
        
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        GL.BindTexture(TextureTarget.Texture2D, _textureId);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 800, 600, 0, PixelFormat.Rgb, PixelType.UnsignedByte, _textureData);
        
        GL.BindVertexArray(_vao);
        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
        
        SwapBuffers();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0, 0, 0, 1);
    }
}
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Raytracer.Primitives;

namespace Raytracer;

public class Application : GameWindow
{
    private float[] vertices =
    {
        -1.0f, -1.0f, 1.0f,     1.0f, 0.0f, 1.0f,       0.0f, 1.0f,
        -1.0f, 1.0f, 1.0f,      1.0f, 0.0f, 1.0f,       0.0f, 0.0f,
        1.0f, 1.0f, 1.0f,       1.0f, 0.0f, 1.0f,       1.0f, 0.0f,
        1.0f, -1.0f, 1.0f,        1.0f, 1.0f, 1.0f,       1.0f, 1.0f,
    };

    private int[] indices =
    {
        0, 1, 2,
        0, 2, 3
    };
    
    private int vbo, vao, ebo, textureId;

    private byte[] textureData = new byte[800 * 600 * 3];

    private Shader shader;
    private Raytracer _raytracer;
    private Scene _scene;
    
    public Application(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = (width, height),
            Title = title
        })
    {
        textureId = GL.GenTexture();
        
        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        ebo = GL.GenBuffer();
        
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        
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
        shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
        Console.WriteLine("Ended shader compilation.");
        
        Console.WriteLine("Initializing raytracer...");
        IPrimitive[] primitives = new IPrimitive[1];
        primitives[0] = new Plane(Vector3.Zero, new Vector3(0, 1.0f, 0), new Vector3(1.0f, 0.0f, 0.0f));
        _scene = new Scene(primitives);
        _raytracer = new Raytracer(800, 600, 0.5f, 0.8f, 0.6f, _scene, Vector3.Zero);
        Console.WriteLine("Raytracer initialized.");
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
        
        _raytracer.Render(textureData);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        // GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        // GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.UseProgram(shader.Handle);
        
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
//        GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 800, 600, 0, PixelFormat.Rgb, PixelType.UnsignedByte, textureData);
        
        GL.BindVertexArray(vao);
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
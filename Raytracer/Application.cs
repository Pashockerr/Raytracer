using OpenTK.Graphics.ES20;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Raytracer;

public class Application : GameWindow
{
    private float[] vertices =
    {
        -1.0f, -1.0f, 0.0f,
        -1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 0.0f,
        1.0f, 0.0f, 0.0f
    };

    private int vbo;
    
    public Application(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = (width, height),
            Title = title
        })
    {
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        SwapBuffers();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0, 0, 0, 1);
    }
}
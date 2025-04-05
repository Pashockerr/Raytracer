using OpenTK.Graphics.ES20;

namespace Raytracer;

public class Shader
{
    public int Handle;
    private int vertexShader;
    private int fragmentShader;

    public Shader(string vertexShaderPath, string fragmentShaderPath)
    {
        string vst = File.ReadAllText(vertexShaderPath);
        string fst = File.ReadAllText(fragmentShaderPath);
        
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vst);
        
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fst);
        
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var code);
        if (code == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
        }
        
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var code2);
        if (code2 == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
        }
        
        Handle = GL.CreateProgram();
        
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, ProgramParameter.LinkStatus, out var code3);
        if (code3 == 0)
        {
            Console.WriteLine(GL.GetProgramInfoLog(Handle));
        }
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }
}
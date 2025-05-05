using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task1;

public partial class Form1 : Form
{

    Matrix4 _model = Matrix4.Identity;
    Matrix4 _view = Matrix4.Identity;
    Matrix4 _projection = Matrix4.Identity;

    bool _isDragging = false;
    Point _lastMousePos = Point.Empty;
    float _cameraZ = 15f;
    Vector3 _cameraPos = new(0, 20f, 15f);

    Vector3 _lightPos = new Vector3(5.0f, 0.0f, 5.0f);
    Vector3 _lightColor = new Vector3(1.0f, 1.0f, 1.0f);
    float _ambientStrength = 0.3f;

    Picture.Picture _picture = new();

    public Form1()
    {
        InitializeComponent();
    }
    private void GLControlLoad(object sender, EventArgs args)
    {
        GL.ClearColor(0.8f, 1.0f, 1.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.ColorMaterial);

        CalculateProjectionMatrix();
        LoadProjectionMatrix();
        UpdateViewMatrix();

        _picture.LoadPicture();
    }

    private void LoadProjectionMatrix()
    {
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref _projection);
        GL.MatrixMode(MatrixMode.Modelview);
    }

    private void GLControlDisposed(object sender, EventArgs e)
    {
    }
    private void GLControlResize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
        CalculateProjectionMatrix();
        LoadProjectionMatrix();
    }
    private void GLControlPaint(object sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _picture.Paint();

        glControl1.SwapBuffers();
    }

    // Mouse handle
    private void GLControlMouseDown(object sender, MouseEventArgs args)
    {
        if (_isDragging)
        {
            return;
        }

        _lastMousePos = args.Location;
        _isDragging = true;
    }
    private void GLControlMouseUp(object sender, MouseEventArgs args)
    {
        if (!_isDragging)
        {
            return;
        }

        _lastMousePos = Point.Empty;
        _isDragging = false;
    }
    private void GLControlMouseMove(object sender, MouseEventArgs args)
    {
        if (!_isDragging)
        {
            return;
        }

        float dx = ((float)_lastMousePos.X - (float)args.Location.X) / 20;

        _lastMousePos = args.Location;

        _cameraPos = RotateVectorByXY(_cameraPos, dx, _cameraZ);
        UpdateViewMatrix();

        glControl1.Invalidate();
    }

    // Update view matrix
    private void UpdateViewMatrix()
    {
        _view = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitZ);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref _view);

        glControl1.Invalidate();
    }

    // Rotate vector
    private static Vector3 RotateVectorByXY(Vector3 vector, float angle, float z)
    {
        float x = vector.X;
        float y = vector.Y;

        float sin = (float)Math.Sin(angle);
        float cos = (float)Math.Cos(angle);

        return new Vector3(x * cos - y * sin, x * sin + y * cos, z);
    }

    // Calculate projection matrix on resize
    private void CalculateProjectionMatrix()
    {
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),
            (float)glControl1.Width / (float)glControl1.Height, 0.1f, 100f);
    }
}

using Assimp;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace Task1.Picture;

public class Shape
{
    Scene _scene;

    float _x;
    float _y;
    float _z;
    float _scale;

    bool _isInverse;

    public Shape(
        float x,
        float y,
        float z,
        float scale,
        bool isInverse = false)
    {
        _x = x;
        _y = y;
        _z = z;
        _scale = scale;
        _isInverse = isInverse;
    }

    public void LoadPicture(string path)
    {
        AssimpContext context = new AssimpContext();
        _scene = context.ImportFile(path,
            PostProcessSteps.Triangulate |
            PostProcessSteps.GenerateNormals |
            PostProcessSteps.CalculateTangentSpace
        );
    }

    public void Paint()
    {
        if (_scene == null)
            return;

        foreach (Mesh mesh in _scene.Meshes)
        {
            Color4D materialColor = _scene.Materials[mesh.MaterialIndex].ColorDiffuse;

            float r = _isInverse ? 1 - materialColor.R : materialColor.R;
            float g = _isInverse ? 1 - materialColor.G : materialColor.G;
            float b = _isInverse ? 1 - materialColor.B : materialColor.B;

            GL.Color3(r, g, b);

            GL.Begin(PrimitiveType.Triangles);
            foreach (var face in mesh.Faces)
            {
                foreach (int index in face.Indices)
                {
                    var normal = mesh.Normals[index];
                    GL.Normal3(normal.X, normal.Y, normal.Z);

                    var vertex = mesh.Vertices[index];
                    GL.Vertex3(
                        _x + vertex.X * _scale,
                        _y + vertex.Y * _scale,
                        _z + vertex.Z * _scale
                    );
                }
            }
            GL.End();
        }
    }
}
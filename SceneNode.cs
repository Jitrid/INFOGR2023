using OpenTK.Mathematics;

namespace Rasterization;

public class SceneNode
{
    public Mesh Mesh { get; set; }
    public Matrix4 Transform { get; set; }
    public List<SceneNode> Children { get; set; }

    public SceneNode(Mesh mesh)
    {
        Children = new List<SceneNode>();
        Transform = mesh.ObjectToWorld;
        Mesh = mesh;
    }

    public void AddChild(SceneNode child)
    {
        Children.Add(child);
    }

    public void Render(Shader shader, Matrix4 parentTransform, Matrix4 worldToCamera, Matrix4 cameraToScreen)
    {
        // Combine this node's transform with the parent's transform
        Matrix4 combinedTransform = Transform * parentTransform;

        // Render the mesh at this node with the combined transform
        Mesh.Render(shader, combinedTransform * worldToCamera, cameraToScreen);

        // Render all child nodes with the combined transform
        foreach (var child in Children)
        {
            child.Render(shader, combinedTransform, worldToCamera, cameraToScreen);
        }
    }
}

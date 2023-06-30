using OpenTK.Mathematics;

namespace Rasterization;

/// <summary>
/// Represents the recursive scene graph datastructure.
/// </summary>
public class SceneGraph
{
    public List<SceneNode> Nodes;

    public SceneGraph(List<Mesh> meshes)
    {
        Nodes = new List<SceneNode>();
        foreach (Mesh m in meshes)
            Nodes.Add(new SceneNode(m));
    }
    public void Render(Shader s, Matrix4 pt, Matrix4 wc, Matrix4 cs)
    {
        foreach (SceneNode node in Nodes)
            node.Render(s, pt, wc, cs, 0.03f);
    }
}

/// <summary>
/// Represents an individual node in the scene graph.
/// </summary>
public class SceneNode
{
    public Mesh Mesh { get; set; }
    public Matrix4 Transform { get; set; }
    public List<SceneNode> Children { get; set; }

    public float RotationSpeed { get; set; }

    public bool IsChild { get; set; }

    public SceneNode(Mesh mesh, float rotationSpeed = 1f, bool isChild = false)
    {
        Children = new List<SceneNode>();
        Transform = mesh.AffineTransformation;
        Mesh = mesh;
        RotationSpeed = rotationSpeed;
        IsChild = isChild;

    }

    public void AddChild(SceneNode child)
    {
        Children.Add(child);
        child.IsChild = true;
    }

    public void Render(Shader shader, Matrix4 parentTransform, Matrix4 worldToCamera, Matrix4 cameraToScreen, float deltatime)
    {
        // Add a rotation effect to demonstrate children.
        if (IsChild)
        {
            Vector3 relativePos = Transform.ExtractTranslation() - parentTransform.ExtractTranslation();
            
            Matrix4 rot = Matrix4.CreateRotationY(RotationSpeed * deltatime);
            
            Quaternion rotation = rot.ExtractRotation();
            
            Vector3 rotatedPos = Vector3.Transform(relativePos, rotation);
            
            Vector3 finalPos = rotatedPos + parentTransform.ExtractTranslation();
            
            Transform = Matrix4.CreateTranslation(finalPos);
        }

        // Combine this node's transform with the parent's transform
        Matrix4 combinedTransform = Transform * parentTransform;

        // Render the mesh at this node with the combined transform
        Mesh.Render(shader, combinedTransform * worldToCamera, cameraToScreen);

        // Render all child nodes with the combined transform
        foreach (SceneNode child in Children)
            child.Render(shader, combinedTransform, worldToCamera, cameraToScreen, deltatime);
    }
}

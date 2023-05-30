using OpenTK.Mathematics;

namespace INFOGR2023Template;

internal class Light
{
    public Vector3 Location;

    public float r, g, b;

    public Light(Vector3 location, float red, float green, float blue)
    {
        this.Location = location;
        this.r = red;
        this.g = green;
        this.b = blue;
    }
}
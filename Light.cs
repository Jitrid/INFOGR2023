
using Vector3 = OpenTK.Mathematics.Vector3;

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

    public Vector3 LightRay(Vector3 intersection)
    {
        Vector3 lightray = new Vector3(
            Location.X - intersection.X,
            Location.Y - intersection.Y,
            Location.Z - intersection.Z); 

        return lightray;
    }
}
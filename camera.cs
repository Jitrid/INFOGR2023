﻿using OpenTK.Mathematics;

namespace INFOGR2023Template;

public class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; } // look-at direction

    public Vector3 Up { get; set; }
    public Vector3 Right { get; set; }

    public float fov { get; set; }
    public float Yaw { get; set; }
    public float Sensitivity { get; set; }

    public float ScreenDistance { get; set; }

    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }

    public float AspectRatio;

    public Vector3 ImagePlaneCenter;
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;

    // screen plane orthonormal basis
    public Vector3 u;
    public Vector3 v;

    public Camera(int width, int height, Vector3 pos)
    {
        Position = pos;
        ScreenWidth = width;
        ScreenHeight = height;

        AspectRatio = (float)ScreenHeight/ScreenWidth;
        fov = 90f;
        Sensitivity = 0.1f;

        UpdateVectors(AspectRatio);
    }


    public Ray GetRay(Vector3 point)
    {
        Vector3 rayDirection = Vector3.Normalize(point - Position);
            
        return new Ray(Position, rayDirection);
    }

    public void UpdateVectors(float ratio)
    {
        Direction = Vector3.UnitZ;
        Up = Vector3.UnitY;
        Right = Vector3.UnitX;

        ScreenDistance = Vector3.Distance(Vector3.Zero, ratio * Right) /
                         (float)MathHelper.Tan(MathHelper.DegreesToRadians(0.5 * fov));

        ImagePlaneCenter = Position + ScreenDistance * Direction;
        p0 = ImagePlaneCenter + Up - AspectRatio * Right;
        p1 = ImagePlaneCenter + Up + AspectRatio * Right;
        p2 = ImagePlaneCenter - Up - AspectRatio * Right;

        u = p1 - p0;
        v = p2 - p0;
    }
}
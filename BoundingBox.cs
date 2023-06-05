using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using static INFOGR2023Template.Utilities;

namespace INFOGR2023Template
{
    public class BoundingBox
    {
        public Vector3 P0; // top left
        public Vector3 P3; // bottom right

        public BoundingBox(Vector3 p0, Vector3 p3)
        {
            P0 = p0;
            P3 = p3;
        }

        public bool intersectBox(Ray r)
        {
            float lambdasmall = (P0.X - r.Origin.X) / r.Direction.X;
            float lambdabig = (P3.X - r.Origin.X) / r.Direction.X;

            if (!(lambdasmall <= lambdabig)) 
            {
                (lambdasmall, lambdabig) = (lambdabig, lambdasmall);
            }

            float lymin = (P0.Y - r.Origin.Y) / r.Direction.Y;
            float lymax = (P3.Y - r.Origin.Y) / r.Direction.Y;

            if (!(lymin <= lymax))
            {
                (lymin,  lymax) = (lymax,lymin);
            }

            if (!(lambdasmall <= lymax))
            {
                return false;
            }

            if (!(lymin <= lambdabig))
            {
                return false;
            }

            if (lymax < lambdabig)
            {
                lambdabig = lymax;
            }

            if (!(lymin <= lambdasmall))
            {
                lambdasmall = lymin;
            }

                float lzmin = (P0.Z - r.Origin.Z) / r.Direction.Z;
                float lzmax = (P3.Z - r.Origin.Z) / r.Direction.Z;

                if (!(lzmin <= lzmax))
                {
                    (lzmin, lzmax) = (lzmax, lzmin);
                }

                if (!(lambdasmall <= lzmax))
                {
                    return false;
                }

                if (!(lzmin <= lambdabig))
                {
                    return false;
                }

                return true;
        } 
            

        

    }
    
    

}

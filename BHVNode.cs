using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    public class BHVNode
    {
        public BoundingBox BHVNodeBox;
        public BHVNode l { get; set; }
        public BHVNode r { get; set; }

        public BHVNode(BoundingBox b)
        {
            BHVNodeBox = b;
            l = null;
            r = null;

        }

        //public BHVNode build(List<Primitive> prims)
        //{
        //    if (prims.Count = 0) return null;
        //    return null;
        //}

    }
}

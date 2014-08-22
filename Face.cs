using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openGltest
{
    /*
     * A face simply holds the indexes of vertices 
     * within the vertex table. 
     */
    class Face
    {
        public int a, b, c, d;


        // triangluar face
        public Face(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        // quad face
        public Face(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        // Console specific Debug
        public void toConsoleString()
        {
            System.Diagnostics.Debug.WriteLine("[" + a + "," + b + "," + c + "," + d + "]");
        }

    }
}

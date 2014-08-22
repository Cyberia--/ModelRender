using System;
using Tao.FreeGlut;
using OpenGL;
using openGltest;

/*
 *   Model Loader is Released under GNU GPL Version 3+ or compatible
 *   licence.
 * 
 *   Model Loader is distributed without the implied warranty of
 *   merchantability or firness for a partcular purpose.  See the
 *   GNU General Public License for more details.
 *   
 *   See <http://www.gnu.org/licenses/>.
 */

namespace ModelViewer
{

    /*
     * ModelViewer
     * Oliver Smithson
     * 
     * This program is designed to take in a .obj file and 
     * display it rotating in an OpenGL context window. 
     * 
     * The program can take both triangulated and quad shapes 
     * and display them on screen colouring each face differently 
     * in order to highlight the complexity of the model.
     * 
     * The program itself is a little rough around the edges as
     * this is my first project in C#. I think that the ArrayLists
     * that are heavily used are deprecated in favour of List<T>
     * which should be changed.
     * 
     * For .obj files, the file cannot contain UV coordinates
     * or the program will crash. (Each face should be nn, not nn/nn/nn)
     * 
     * [Controls]
     * Press Space to pause the rotation then control the model using WASD.
     * Press F to toggle fullscreen mode.  
     * 
     */

    class Program
    {
        private static int width = 1280, height = 720;
        private static ShaderProgram program;
        private static Model model;
        private static System.Diagnostics.Stopwatch watch;
        private static InputHandler input;

        static void Main(string[] args)
        {          
            // create an OpenGL window
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGl Model Render");

            // now that gl has been initialized we can create anything that depends on it
            model = new Model(@"..\..\Resources\bunny.obj");

            // Create an input handler to perovide callbacks fot keyboard events 
            input = new InputHandler(); 

            // Callbacks Form Functions 
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);

            // Callbacks Input Events 
            Glut.glutKeyboardFunc(input.OnKeyboardDown);
            Glut.glutKeyboardUpFunc(input.OnKeyboardUp);
            Glut.glutReshapeFunc(OnReshape);

            // Compile the shader program
            Gl.Enable(EnableCap.DepthTest);

            // Create a basic Global light using GLSL shaders 
            program = new ShaderProgram(ShaderLibrary.VertexShader, ShaderLibrary.FragmentShader);

            // use the shader and set up a preliminary model and projection matrix
            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up));

            // create a watch to provide timing for the main loop
            watch = System.Diagnostics.Stopwatch.StartNew();

            //  Enter the main loop to call the render/update function
            Glut.glutMainLoop();
        }

        // The lower level libraries still need stack attention
        private static void OnClose()
        {
            model.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
        }

        // Stub
        private static void OnDisplay(){}

        /*
         * [1] Calculate the Delta time from the last frame to create a seamless
         *     rotation animation. The delta frequency dictates the speed of 
         *     animation. 
         * [2] Check for keyboard input and rotate accordingly. The key 
         *     events were specified in the callbacks during initialization.
         * [3] Clear the OpenGL Buffers to draw fresh frame. Set up the OpenGL 
         *     viewport and clear both the color and depth bits. This is just
         *     housekeeping.
         * [4] Update the current models position and rotation based on [2]
         * [5] Bind the Vertices, Color and Faces VBO's from Model. Check If 
         *     the model is supposed to have colour added. 
         * [6] Check the draw method then actually draw the model to the frame.
         *     Quads or Triangles must be set depending on the shape type. The
         *     shape type is calculated and stored in Model.Triangles / Model.Quads
         * [7] Swap buffers to ensure smoothness.
         */

        private static void OnRenderFrame()
        {         
            // [1]
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            // [2]
            input.checkRotate(deltaTime);

            // [3] 
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.UseProgram(program);

            // [4] 
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(input.yangle / 2) * Matrix4.CreateRotationX(input.xangle));

            // [5] 
            Gl.BindBufferToShaderAttribute(openGltest.Model.ModelVertexBuffer, program, "vertexPosition");
            if (openGltest.Model.falseColor) Gl.BindBufferToShaderAttribute(openGltest.Model.ModelVertexColor, program, "vertexColor");
            Gl.BindBuffer(openGltest.Model.ModelVertexFaces);

            // [6] 
            if (openGltest.Model.triangles)
            {
                // Render Triangles (3 Vertices)
                Gl.DrawElements(BeginMode.Triangles, openGltest.Model.ModelVertexFaces.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
            else if (openGltest.Model.quads)
            {
                // Render Quads (4 Vertices)
                Gl.DrawElements(BeginMode.Quads, openGltest.Model.ModelVertexFaces.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
            
            // [7]
            Glut.glutSwapBuffers();
        }

        // when the window is resized
        private static void OnReshape(int width, int height)
        {
            Program.width = width;
            Program.height = height;

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
        }

    }
}
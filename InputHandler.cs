using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using OpenGL;

namespace openGltest
{

    /*
     * Handle input from the keyboard and 
     * provide a bank of data for the model matrix.
     */
    class InputHandler
    {
        
        public Boolean autoRotate = true, fullscreen = false;
        public Boolean up = false, down = false, left = false, right = false;
        public float xangle, yangle;

        // Callback from main
        public void OnKeyboardDown(byte key, int x, int y)
        {
            // ESC
            if (key == 27)
            {
                Glut.glutLeaveMainLoop();
            }
            
            else if (key == 'w') up = true;
            else if (key == 'a') left = true;
            else if (key == 's') down = true;
            else if (key == 'd') right = true;
        }

        // Callback from main
        public void OnKeyboardUp(byte key, int x, int y)
        {
           // Stop the shape rotating when space is pressed
            if (key == ' ') autoRotate = !autoRotate;
            if (key == 'f')
            {
                fullscreen = !fullscreen;
                if (fullscreen) Glut.glutFullScreen();
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
            else if (key == 'w') up = false;
            else if (key == 'a') left = false;
            else if (key == 's') down = false;
            else if (key == 'd') right = false;

        }

        // Rotate the shape each frame
        public void checkRotate(float deltaTime)
        {
            if (autoRotate)
            {
                xangle += deltaTime;
                yangle += deltaTime / 2;
            }
            else
            {
                if (right)  yangle += deltaTime;
                if (left)   yangle -= deltaTime;
                if (up)     xangle += deltaTime;
                if (down)   xangle -= deltaTime;
            }
        }

    }
}

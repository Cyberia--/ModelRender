using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Holding space for vertex and fragment GLSL shaders. 
 * 
 * These basic shaders create a global light.
 */
namespace openGltest
{
    class ShaderLibrary
    {

        public static string VertexShader = @"
        #version 130
        in vec3 vertexPosition;
        in vec3 vertexColor;

        out vec3 color;

        uniform mat4 projection_matrix;
        uniform mat4 view_matrix;
        uniform mat4 model_matrix;

        void main(void)
        {
            color = vertexColor;
            gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
        }
        ";

        public static string FragmentShader = @"
        #version 130
        in vec3 color;
        
        out vec4 fragment;
        
        void main(void)
        {
            fragment = vec4(color, 1);
        }
        ";
    }
}

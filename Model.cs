using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Tao.FreeGlut;
using OpenGL;

namespace openGltest
{
    class Model
    {
        // To load the .obj file
        private ModelLoader loader;

        // two tables for parsing because the VBO's aren't mutable
        public ArrayList vertexTable = new ArrayList();
        public ArrayList faceTable = new ArrayList();

        // VBO output
        public static VBO<Vector3> ModelVertexBuffer;
        public static VBO<Vector3> ModelVertexColor;
        public static VBO<int> ModelVertexFaces;

        // Give each face a random color
        public static bool falseColor = true;

        // the type of shape
        public static bool triangles = false;
        public static bool quads = false;

        // generate a model from a file path
        public Model(String filePath)
        {
            loader = new ModelLoader(filePath);
            parseModel();
        }

        // create a model from existing data
        public Model(ArrayList vertexTable, ArrayList faceTable)
        {
            this.vertexTable = vertexTable;
            this.faceTable = faceTable;
        }

        /**
         * Parse the raw text from the .obj file into usable arrays of 
         * data. Use this data to produce a VBO that can be modelled by
         * OpenGl.
         * 
         * [1] Split the base file into three array lists representing the 
         *     verteces, vertexNormals and faces according to the .obj 
         *     specification http://www.martinreddy.net/gfx/3d/OBJ.spec
         *     
         * [2] Parse the arrays of strings into usable data. Vertex data 
         *     begins with a 'v', vertex normal data 'vn' and face data 
         *     'f'. Each vertex is comprised of a <x,y,z> coordinate and 
         *     each face is comprised of a <vertex/face/normal> table 
         *     index.
         * 
         * [3] Parse the arrays of usable data into drawable VBO objects. 
         *     ModelVertexBuffer contains a list of vertices to draw.
         *     ModelColorBuffer  is a clone of MVB with added color
         *     ModelFaceBuffer   contains vertex lookups for each face
         */
        private void parseModel(){

            // [1] parse the .obj into Strings 

            char[] delimiterChars = { '\n', ' ' };
            string[] words = loader.ObjectFile.Split(delimiterChars[0]);

            ArrayList vertices      = new ArrayList();
            ArrayList vertexNormals = new ArrayList();
            ArrayList faces         = new ArrayList();
            String substring;

            foreach (string s in words)
            {
                if (s.Length > 0)
                {
                    substring = s.Substring(0, 2);

                    if      (substring == "v ") vertices.Add(s);
                    else if (substring == "vn") vertexNormals.Add(s);
                    else if (substring == "f ") faces.Add(s);
                }

            }

            // [2] Parse the strings into data

            ArrayList p_vertices = new ArrayList();
            ArrayList p_faces = new ArrayList();

            // Parse each string in vertex into a Vector3 object
            foreach (string s in vertices)
            {
                words = s.Split(delimiterChars[1]);
                p_vertices.Add(new Vector3(float.Parse(words[1]),
                                           float.Parse(words[2]),
                                           float.Parse(words[3])));
            }

            // Check how many verteces the .obj uses per face
            // 3    -> The model is triangulated
            // 4    -> The model is not triangulated
            // 5    -> fail    
            String[] faceNumber = (faces[0] as String).Split(delimiterChars[1]); 

            // For triangulated faces
            if (faceNumber.Length == 4)
            {
                // set the render mode
                triangles = true;

                // Parse each string in faces into a face object
                foreach (String s in faces)
                {
                    words = s.Split(delimiterChars[1]);

                    p_faces.Add(new Face(int.Parse(words[1]),
                                         int.Parse(words[2]),
                                         int.Parse(words[3])));
                }

                // create a model using the parsed data  
                int lenfaces = p_faces.Count * 3;

                // create an array to be inserted into the VBO
                Vector3[] vertextInsert = new Vector3[lenfaces];

                // Use the data in the face table to index the vertices 
                int i = 0;

                foreach (Face f in p_faces)
                {
                    vertextInsert[i]     = (Vector3)p_vertices[f.a - 1];
                    vertextInsert[i + 1] = (Vector3)p_vertices[f.b - 1];
                    vertextInsert[i + 2] = (Vector3)p_vertices[f.c - 1];
                    i += 3;
                }

                // insert the vertex array into the VBO
                ModelVertexBuffer = new VBO<Vector3>(vertextInsert);

                // create element buffer based on the number of faces
                int[] face_insert = new int[lenfaces];

                for (i = 0; i < lenfaces; i++)
                {
                    face_insert[i] = i;
                }

                // insert the element array into the VBO
                ModelVertexFaces = new VBO<int>(face_insert, BufferTarget.ElementArrayBuffer);

                // Give each face a false color 
                if (falseColor) createFauxColor(faceNumber.Length, vertextInsert);
            }
            else if (faceNumber.Length == 5)
            {
                

                // set the render mode
                quads = true;

                // Parse each string in faces into a face object
                foreach (String s in faces)
                {
                    words = s.Split(delimiterChars[1]);

                    p_faces.Add(new Face(int.Parse(words[1]),
                                         int.Parse(words[2]),
                                         int.Parse(words[3]),
                                         int.Parse(words[4])));
                }

                // create a model using the parsed data  
                int lenfaces = p_faces.Count * 4;

                // create an array to be inserted into the VBO
                Vector3[] vertextInsert = new Vector3[lenfaces];

                // Use the data in the face table to index the vertices
                int i = 0;

                foreach (Face f in p_faces)
                {
                    vertextInsert[i]     = (Vector3)p_vertices[f.a - 1];
                    vertextInsert[i + 1] = (Vector3)p_vertices[f.b - 1];
                    vertextInsert[i + 2] = (Vector3)p_vertices[f.c - 1];
                    vertextInsert[i + 3] = (Vector3)p_vertices[f.d - 1];
                    i += 4;
                }

                // insert the vertex array into the VBO 
                ModelVertexBuffer = new VBO<Vector3>(vertextInsert);

                // create element buffer based on the number of faces
                int[] face_insert = new int[lenfaces];

                for (i = 0; i < lenfaces; i++)
                {
                    face_insert[i] = i;
                }

                // insert the element array into the VBO
                ModelVertexFaces = new VBO<int>(face_insert, BufferTarget.ElementArrayBuffer);

                // Give each face a false color 
                if (falseColor) createFauxColor(faceNumber.Length, vertextInsert);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The .obj cannot be processed");
                Environment.Exit(1); 
            }
        }

        private void createFauxColor(int faceNumber, Vector3[] vertex)
        {
            // create faux colors to distinguish 
            //  [1] Pass in the vertex array because it is the same structure 
            //      as the VBO
            //  [2] Change each set of <faceNumber> verteces into a Vertex3 
            //      a color representing 
            //  [3] Pass the new vertex colors into the ModelVeretexArray
                
            Vector3 color = getRandomColor();

            // triangularfaces 
            if (faceNumber == 4)
            {
                for (int i = 0; i < vertex.Length; i++)
                {
                    vertex[i] = color;
                    if ((i % faceNumber) == 0) color = getRandomColor();
                }
            }

            // square faces 
            else if (faceNumber == 5)
            {
                // represent each face with a color
                for (int i = 0; i < vertex.Length; i++)
                {
                    vertex[i] = color;
                    if ((i % faceNumber) == 0)color = getRandomColor();
                }
            }

            // Pass the final color into the vertex buffer
            ModelVertexColor = new VBO<Vector3>(vertex);
        }

        // If Random is called within a short space of time it 
        // just returns the same number
        Random r = new Random();

        private Vector3 getRandomColor()
        {
            return new Vector3(r.NextDouble(), 
                               r.NextDouble(),
                               r.NextDouble());
        }

        // remove the data from the lower level OGL libraries
        public void Dispose()
        {
            ModelVertexBuffer.Dispose();
            ModelVertexFaces.Dispose();
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Load a model from a .obj file using a stream reader. 
 * This is in a separate class for easier stack tracing
 */
namespace openGltest
{
    class ModelLoader
    {
        String filepath;
        public String ObjectFile;

        public ModelLoader(String filepath)
        {
            // load the object file into a string ObjectFile
            this.filepath = filepath;
            load();
        }

        private void load()
        {         
            try
            {
                // default filepath for testing
                using (StreamReader sr = new StreamReader(filepath))
                {
                    ObjectFile = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read. ");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}

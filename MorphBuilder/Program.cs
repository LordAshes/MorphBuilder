using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphBuilder
{
    /// <summary>
    /// Program for easily creating Multi Stage Morph Delta files
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Calculates the delta moprh data between two morphs and a given source
        /// </summary>
        /// <param name="morph2">Next stage moprh OBJ class implementation</param>
        /// <param name="morph1">Previous stage morph OBJ class implementation</param>
        /// <param name="source">Source OBJ class implementation</param>
        /// <returns>OBJ class holding the delta morph (or partial delta morph)</returns>
        static OBJ Process(OBJ morph2, OBJ morph1, OBJ source)
        {
            foreach (KeyValuePair<string, OBJGroup> group in morph2.groups)
            {
                if (source != null)
                {
                    for (int i = 0; i < group.Value.v.Count; i++)
                    {
                        morph2.groups[group.Key].v.ElementAt(i).x = morph2.groups[group.Key].v.ElementAt(i).x - morph1.groups[group.Key].v.ElementAt(i).x + source.groups[group.Key].v.ElementAt(i).x;
                        morph2.groups[group.Key].v.ElementAt(i).y = morph2.groups[group.Key].v.ElementAt(i).y - morph1.groups[group.Key].v.ElementAt(i).y + source.groups[group.Key].v.ElementAt(i).y;
                        morph2.groups[group.Key].v.ElementAt(i).z = morph2.groups[group.Key].v.ElementAt(i).z - morph1.groups[group.Key].v.ElementAt(i).z + source.groups[group.Key].v.ElementAt(i).z;
                    }
                }
                else
                {
                    for (int i = 0; i < group.Value.v.Count; i++)
                    {
                        morph2.groups[group.Key].v.ElementAt(i).x = morph2.groups[group.Key].v.ElementAt(i).x - morph1.groups[group.Key].v.ElementAt(i).x;
                        morph2.groups[group.Key].v.ElementAt(i).y = morph2.groups[group.Key].v.ElementAt(i).y - morph1.groups[group.Key].v.ElementAt(i).y;
                        morph2.groups[group.Key].v.ElementAt(i).z = morph2.groups[group.Key].v.ElementAt(i).z - morph1.groups[group.Key].v.ElementAt(i).z;
                    }
                }
            }
            return morph2;
        }

        /// <summary>
        /// Program for calculating Delta Morphs based on a source OBJ file and Moprh OBJ files
        /// </summary>
        /// <param name="args">List of OBJ files in order (with no spaces in location/name)</param>
        static void Main(string[] args)
        {
            // Create a list of OBj files
            List<OBJ> morphs = new List<OBJ>();

            // Load each of the specified OBJ files into the OBJ list
            foreach(string obj in args)
            {
                morphs.Add(OBJ.loadOBJ(obj));
            };

            // Process each OBJ file
            for (int s = 0; s < morphs.Count(); s++)
            {
                // Obtain a reference to the current Morph
                OBJ partial = morphs.ElementAt(s);
                OBJ.saveOBJ("Morph." + s.ToString("d2") + ".Source.obj", partial);
                // Subtract out all of the previous Morphs
                Console.WriteLine("Creating Morph " + s + " Delta...");
                for (int d = s-1; d >0; d--)
                {
                    Console.WriteLine("  Subtracting Morph " + d + "...");
                    partial = Process(partial, morphs.ElementAt(d), morphs.ElementAt(0));
                }
                // Save the final Delta Morph
                Console.WriteLine("  Saving Morph " + s + " Delta...");
                OBJ.saveOBJ("Morph." + s.ToString("d2") + ".Delta.obj", partial);
            }
        }
    }
}

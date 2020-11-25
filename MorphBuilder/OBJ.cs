using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphBuilder
{
    /// <summary>
    /// Class for holding an OBJ files's V data
    /// </summary>
    public class OBJV
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public OBJV(string x, string y, string z)
        {
            this.x = double.Parse(x);
            this.y = double.Parse(y);
            this.z = double.Parse(z);
        }
    }

    /// <summary>
    /// Class for holding an OBJ files's T data
    /// </summary>
    public class OBJT
    {
        public double x { get; set; }
        public double y { get; set; }

        public OBJT(string x, string y)
        {
            this.x = double.Parse(x);
            this.y = double.Parse(y);
        }
    }

    /// <summary>
    /// Class for holding an OBJ files's F point details
    /// </summary>
    public class OBJFPoint
    {
        public int v { get; set; }
        public int t { get; set; }
        public int n { get; set; }

        public OBJFPoint(string v, string t, string n)
        {
            this.v = int.Parse(v);
            this.t = int.Parse(t);
            this.n = int.Parse(n);
        }
    }

    /// <summary>
    /// Class for holding an OBJ files's F face data
    /// </summary>
    public class OBJF
    {
        public List<OBJFPoint> f { get; set; } = new List<OBJFPoint>();
    }

    /// <summary>
    /// Class for holding an OBJ file's G data
    /// </summary>
    public class OBJGroup
    {
        public List<OBJV> v { get; set; } = new List<OBJV>();
        public List<OBJV> vn { get; set; } = new List<OBJV>();
        public List<OBJT> vt { get; set; } = new List<OBJT>();
        public List<OBJF> f { get; set; } = new List<OBJF>();
        public string m { get; set; }
        public string u { get; set; } = "___default___";
    }

    /// <summary>
    /// Class for holding an OBJ file's data
    /// </summary>
    public class OBJ
    {
        public Dictionary<string, OBJGroup> groups { get; set; } = new Dictionary<string, OBJGroup>();

        /// <summary>
        /// Method for loading an OBJ file into the OBJ class
        /// </summary>
        /// <param name="file">Location and name of the OBJ file to be read</param>
        /// <param name="ignoreGroups">Allows the load to ignore group information (thus combining all group data into one common group). Default false.</param>
        /// <returns>OBJ class represeting the data from the OBJ file</returns>
        public static OBJ loadOBJ(string file, bool ignoreGroups = false)
        {
            OBJ obj3d = new OBJ();

            string material = "";
            foreach (string entry in System.IO.File.ReadAllLines(file))
            {
                string[] parts = entry.Split(' ');
                if (entry.Trim() != "")
                {
                    switch (entry.Trim().Substring(0, 2))
                    {
                        case "mt":
                            material = entry.Substring("mtllib ".Length);
                            break;
                        case "us":
                            if (obj3d.groups.Count() == 0) { obj3d.groups.Add("DefaultGroup", new OBJGroup()); }
                            obj3d.groups.ElementAt(obj3d.groups.Count() - 1).Value.u = entry.Substring("usemtl ".Length);
                            break;
                        case "g ":
                            if ((ignoreGroups == false) || (obj3d.groups.Count == 0))
                            {
                                obj3d.groups.Add(entry.Substring("g ".Length), new OBJGroup());
                                obj3d.groups[entry.Substring("g ".Length)].m = material;
                            }
                            break;
                        case "v ":
                            if (obj3d.groups.Count() == 0) { obj3d.groups.Add("DefaultGroup", new OBJGroup()); }
                            obj3d.groups.ElementAt(obj3d.groups.Count() - 1).Value.v.Add(new OBJV(parts[1], parts[2], parts[3]));
                            break;
                        case "vn":
                            if (obj3d.groups.Count() == 0) { obj3d.groups.Add("DefaultGroup", new OBJGroup()); }
                            obj3d.groups.ElementAt(obj3d.groups.Count() - 1).Value.vn.Add(new OBJV(parts[1], parts[2], parts[3]));
                            break;
                        case "vt":
                            if (obj3d.groups.Count() == 0) { obj3d.groups.Add("DefaultGroup", new OBJGroup()); }
                            obj3d.groups.ElementAt(obj3d.groups.Count() - 1).Value.vt.Add(new OBJT(parts[1], parts[2]));
                            break;
                        case "f ":
                            if (obj3d.groups.Count() == 0) { obj3d.groups.Add("DefaultGroup", new OBJGroup()); }
                            OBJF newFaceGroup = new OBJF();
                            for (int i = 1; i < parts.Count(); i++)
                            {
                                string[] subparts = parts[i].Split('/');
                                if (subparts.Count() == 3)
                                {
                                    newFaceGroup.f.Add(new OBJFPoint(subparts[0], subparts[1], subparts[2]));
                                }
                                else if (subparts.Count() == 2)
                                {
                                    newFaceGroup.f.Add(new OBJFPoint(subparts[0], subparts[1], "-1"));
                                }
                                else
                                {
                                    newFaceGroup.f.Add(new OBJFPoint(subparts[0], "-1", "-1"));
                                }
                            }
                            obj3d.groups.ElementAt(obj3d.groups.Count() - 1).Value.f.Add(newFaceGroup);
                            break;
                    }
                }
            }
            Console.WriteLine(file + " Contains...");
            Console.WriteLine("   " + obj3d.groups.Count() + " Groups");
            foreach (KeyValuePair<string, OBJGroup> group in obj3d.groups)
            {
                Console.WriteLine("     Group " + group.Key + " Contains...");
                Console.WriteLine("       " + group.Value.v.Count() + " v entries");
                Console.WriteLine("       " + group.Value.vn.Count() + " vn entries");
                Console.WriteLine("       " + group.Value.vt.Count() + " vt entries");
                Console.WriteLine("       " + group.Value.f.Count() + " f entries");
            }
            Console.WriteLine("");
            return obj3d;
        }

        /// <summary>
        /// Method for saving an OBJ class to an OBJ file
        /// </summary>
        /// <param name="file">Location and name of the file in which the OBJ class should be saved</param>
        /// <param name="data">OBJ class instance to be save</param>
        public static void saveOBJ(string file, OBJ data)
        {
            List<string> output = new List<string>();
            foreach (KeyValuePair<string, OBJGroup> group in data.groups)
            {
                output.Add("# Material Library For " + group.Key);
                output.Add("mtllib " + group.Value.m);
                output.Add(" ");
                output.Add("g " + group.Key);
                output.Add(" ");
                output.Add("# V Count = " + group.Value.v.Count());
                foreach (OBJV v in group.Value.v)
                {
                    output.Add("v " + v.x + " " + v.y + " " + v.z);
                }
                output.Add(" ");
                output.Add("# VN Count = " + group.Value.vn.Count());
                foreach (OBJV vn in group.Value.vn)
                {
                    output.Add("vn " + vn.x + " " + vn.y + " " + vn.z);
                }
                output.Add(" ");
                output.Add("# VT Count = " + group.Value.vt.Count());
                foreach (OBJT vt in group.Value.vt)
                {
                    output.Add("vt " + vt.x + " " + vt.y);
                }
                output.Add(" ");
                output.Add("# Material For " + group.Key);
                output.Add("usemtl " + group.Value.u);
                output.Add(" ");
                output.Add("# F Count = " + group.Value.f.Count());
                foreach (OBJF f in group.Value.f)
                {
                    string outline = "f ";
                    foreach (OBJFPoint fp in f.f)
                    {
                        if (fp.n != -1)
                        {
                            outline = outline + fp.v + "/" + fp.t + "/" + fp.n + " ";
                        }
                        else if (fp.t != -1)
                        {
                            outline = outline + fp.v + "/" + fp.t + " ";

                        }
                        else
                        {
                            outline = outline + fp.v + " ";
                        }
                    }
                    outline = outline.Substring(0, outline.Length - 1);
                    output.Add(outline);
                }
                output.Add(" ");
                output.Add("# End Of Object");
                System.IO.File.WriteAllLines(file, output.ToArray());
            }
        }
    }
}

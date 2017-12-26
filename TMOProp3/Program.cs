using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TMOProp3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: TMOProp3 tmo_file_or_folder [base_a base_b]");
                return;
            }

            string source_file = args[0];
            string defo_file;
            string prop_file;
            if (args.Length > 2)
            {
                defo_file = args[1];
                prop_file = args[2];
            }
            else
            {
                defo_file = Path.Combine(Application.StartupPath, @"base_a.tmo");
                prop_file = Path.Combine(Application.StartupPath, @"base_b.tmo");
            }

            if (Path.GetExtension(source_file).ToLower() == ".tmo")
            {
                Program program = new Program(defo_file, prop_file);
                program.Process(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                Program program = new Program(defo_file, prop_file);
                foreach (string file in Directory.GetFiles(source_file, "*.tmo"))
                {
                    program.Process(file);
                }
            }
        }

        TMOFile tmo_defo;
        TMOFile tmo_prop;
        int nodes_length;

        public Program(string defo_file, string prop_file)
        {
            tmo_defo = new TMOFile();
            tmo_defo.Load(defo_file);

            tmo_prop = new TMOFile();
            tmo_prop.Load(prop_file);

            nodes_length = tmo_defo.nodes.Length;
            Debug.Assert(nodes_length == tmo_prop.nodes.Length, "nodes length mismatch between base_a and base_b.");

            tmo_defo.LoadTransformationMatrixFromFrame(0);
            tmo_prop.LoadTransformationMatrixFromFrame(0);
        }

        void tmo_Transform(TMOFile tmo)
        {
            Debug.Assert(nodes_length == tmo.nodes.Length, "nodes length mismatch between base and source.");

            int[] id_pair = tmo_defo.CreateNodeIdPair(tmo);

            int len = tmo.frames.Length;

            for (int i = 0; i < len; i++)
            {
                tmo.LoadTransformationMatrixFromFrame(i);

                for (int x = 0; x < nodes_length; x++)
                {
                    int y = id_pair[x];

                    Vector3 S0 = tmo_defo.nodes[x].Scaling;
                    Vector3 S1 = tmo_prop.nodes[x].Scaling;
                    Vector3 S2 = tmo.nodes[y].Scaling;

                    Quaternion R0 = tmo_defo.nodes[x].Rotation;
                    Quaternion R1 = tmo_prop.nodes[x].Rotation;
                    Quaternion R2 = tmo.nodes[y].Rotation;

                    Vector3 T0 = tmo_defo.nodes[x].Translation;
                    Vector3 T1 = tmo_prop.nodes[x].Translation;
                    Vector3 T2 = tmo.nodes[y].Translation;

                    // dS = S2 / S1
                    // S. = S0 * dS
                    tmo.nodes[y].Scaling = new Vector3(S0.X * S2.X / S1.X, S0.Y * S2.Y / S1.Y, S0.Z * S2.Z / S1.Z);

                    // dR = inv(R1) * R2
                    // R. = R0 * dR
                    tmo.nodes[y].Rotation = R0 * Quaternion.Invert(R1) * R2;

                    // dT = T2 - T1
                    // T. = T0 + dT
                    tmo.nodes[y].Translation = T0 + T2 - T1;
                }

                tmo.SaveTransformationMatrixToFrame(i);
            }
        }

        public bool Process(string source_file)
        {
            TMOFile tmo = new TMOFile();
            tmo.Load(source_file);

            if (tmo.nodes[0].Path == "|W_Hips")
            {
                tmo_Transform(tmo);
            }

            string dest_file = source_file + ".tmp";
            Console.WriteLine("Save File: " + dest_file);
            tmo.Save(dest_file);

            File.Delete(source_file);
            File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return true;
        }
    }
}

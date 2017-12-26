using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace PNGProp3
{
    public class TSOData
    {
        internal uint opt1;
        internal byte[] ftso;
    }

    public class TSOFigure
    {
        internal List<TSOData> TSOList = new List<TSOData>();
        internal TMOFile tmo = null;

        internal byte[] lgta;
        internal byte[] figu;
    }

    class Program
    {
        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: PNGProp3 png_file_or_folder");
                return;
            }

            string source_file = args[0];
            string defo_file = Path.Combine(Application.StartupPath, @"base_a.tmo");
            string prop_file = Path.Combine(Application.StartupPath, @"base_b.tmo");

            if (Path.GetExtension(source_file).ToLower() == ".png")
            {
                Program program = new Program(defo_file, prop_file);
                program.Process(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                Program program = new Program(defo_file, prop_file);
                foreach (string file in Directory.GetFiles(source_file, "*.png"))
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

        internal List<TSOFigure> TSOFigureList = null;

        byte[] cami;

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

                    Vector3 S0 = tmo_prop.nodes[x].Scaling;
                    Vector3 S1 = tmo_defo.nodes[x].Scaling;
                    Vector3 S2 = tmo.nodes[y].Scaling;

                    Quaternion R0 = tmo_prop.nodes[x].Rotation;
                    Quaternion R1 = tmo_defo.nodes[x].Rotation;
                    Quaternion R2 = tmo.nodes[y].Rotation;

                    Vector3 T0 = tmo_prop.nodes[x].Translation;
                    Vector3 T1 = tmo_defo.nodes[x].Translation;
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
            List<TSOFigure> fig_list = new List<TSOFigure>();

            Console.WriteLine("Load File: " + source_file);
            PNGFile source = new PNGFile();
            string source_type = "";

            try
            {
                TSOFigure fig = null;
                TMOFile tmo = null;

                source.Hsav += delegate(string type)
                {
                    source_type = type;

                    fig = new TSOFigure();
                    fig_list.Add(fig);
                };
                source.Pose += delegate(string type)
                {
                    source_type = type;
                };
                source.Scne += delegate(string type)
                {
                    source_type = type;
                };
                source.Cami += delegate(Stream dest, int extract_length)
                {
                    cami = new byte[extract_length];
                    dest.Read(cami, 0, extract_length);
                };
                source.Lgta += delegate(Stream dest, int extract_length)
                {
                    byte[] lgta = new byte[extract_length];
                    dest.Read(lgta, 0, extract_length);

                    fig = new TSOFigure();
                    fig.lgta = lgta;
                    fig_list.Add(fig);
                };
                source.Ftmo += delegate(Stream dest, int extract_length)
                {
                    tmo = new TMOFile();
                    tmo.Load(dest);
                    fig.tmo = tmo;
                };
                source.Figu += delegate(Stream dest, int extract_length)
                {
                    byte[] figu = new byte[extract_length];
                    dest.Read(figu, 0, extract_length);

                    fig.figu = figu;
                };
                source.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    byte[] ftso = new byte[extract_length];
                    dest.Read(ftso, 0, extract_length);

                    TSOData tso = new TSOData();
                    tso.opt1 = BitConverter.ToUInt32(opt1, 0);
                    tso.ftso = ftso;
                    fig.TSOList.Add(tso);
                };

                source.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            TSOFigureList = fig_list;

            foreach (TSOFigure fig in TSOFigureList)
                if (fig.tmo != null)
                if (fig.tmo.nodes[0].Path == "|W_Hips")
                {
                    tmo_Transform(fig.tmo);
                }

            string dest_file = source_file + ".tmp";
            Console.WriteLine("Save File: " + dest_file);
            source.WriteTaOb += delegate(BinaryWriter bw)
            {
                PNGWriter pw = new PNGWriter(bw);
                switch (source_type)
                {
                case "HSAV":
                    WriteHsav(pw);
                    break;
                case "POSE":
                    WritePose(pw);
                    break;
                case "SCNE":
                    WriteScne(pw);
                    break;
                }
            };
            source.Save(dest_file);

            File.Delete(source_file);
            File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return true;
        }

        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            foreach (TSOFigure fig in TSOFigureList)
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
        }

        protected void WritePose(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WritePOSE();
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
            }
        }

        protected void WriteScne(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteSCNE(FigureCount());
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
                pw.WriteFIGU(fig.figu);
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
            }
        }

        protected int FigureCount()
        {
            return TSOFigureList.Count;
        }
    }
}

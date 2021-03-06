using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;

namespace PNGProportion
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
                System.Console.WriteLine("Usage: PNGProportion <source png>");
                return;
            }

            string source_file = args[0];

            if (Path.GetExtension(source_file).ToLower() == ".png")
            {
                Program program = new Program();
                program.Process(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                Program program = new Program();
                foreach (string file in Directory.GetFiles(source_file, "*.png"))
                {
                    program.Process(file);
                }
            }
        }

        internal List<TSOFigure> TSOFigureList = null;

        byte[] cami;

        TPOFileList tpo_list = new TPOFileList();
    
        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public Program()
        {
            LoadTPOFileList();
            LoadPortions();
        }

        void LoadTPOFileList()
        {
            tpo_list.Load();
        }

        Dictionary<string, Proportion> portion_map = null;

        void LoadPortions()
        {
            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in config.Proportions)
                portion_map[portion.ClassName] = portion;
        }

        void tmo_Transform(TMOFile tmo)
        {
            tpo_list.Tmo = tmo;

            for (int i = 0; i < tpo_list.Count; i++)
            {
                TPOFile tpo = tpo_list[i];
                {
                    Debug.Assert(tpo.Proportion != null, "tpo.Proportion should not be null");
                    Proportion portion;
                    if (portion_map.TryGetValue(tpo.Proportion.ToString(), out portion))
                        tpo.Ratio = portion.Ratio;
                }
            }

            tpo_list.Transform();
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

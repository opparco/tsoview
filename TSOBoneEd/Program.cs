using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TSOBoneEd
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            TSOConfig tso_config;

            string tso_config_file = Path.Combine(Application.StartupPath, @"config.xml");
            if (File.Exists(tso_config_file))
                tso_config = TSOConfig.Load(tso_config_file);
            else
                tso_config = new TSOConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Form1 form1 = new Form1(tso_config, args))
            using (Form2 form2 = new Form2())
            {
                form2.TopLevel = false;
                form2.Location = new System.Drawing.Point(0, 26);
                form1.Controls.Add(form2);
                form2.BringToFront();
                form2.viewer = form1.viewer;
                form2.TransformationEvent += delegate(object sender, EventArgs e)
                {
                    form1.UpdateSelectedNodeControls();
                    form1.Invalidate(false);
                };

                form1.Show();
                form2.Show();

                Application.Run(form1);
            }
        }
    }
}

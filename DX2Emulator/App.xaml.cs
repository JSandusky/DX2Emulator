using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace DX2Emulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppUtil.AppDataPath, "DX2Emulator/teams.xml");
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("dx2_teams"));
            foreach (var team in AppData.GetInst().Teams)
                team.Save(doc.DocumentElement);

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            System.Threading.Thread.Sleep(10);
            doc.Save(filePath);
        }
    }
}

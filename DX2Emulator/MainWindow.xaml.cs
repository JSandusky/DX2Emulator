using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using FirstFloor.ModernUI;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;

namespace DX2Emulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ContentLoader = new ContentLoader();
            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;

            TitleLinks.Add(new Link { DisplayName = "Team Designer", Source = new Uri("/Pages/TeamBuilder.xaml", UriKind.Relative) });
            TitleLinks.Add(new Link { DisplayName = "Turn Planner", Source = new Uri("/Pages/TurnPlanner.xaml", UriKind.Relative) });
            TitleLinks.Add(new Link { DisplayName = "Mock Battle", Source = new Uri("/Pages/MockBattle.xaml", UriKind.Relative) });

            string filePath = System.IO.Path.Combine(AppUtil.AppDataPath, "DX2Emulator/teams.xml");
            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(filePath);
                    var teams = doc.DocumentElement.SelectNodes("team");
                    foreach (var team in teams)
                    {
                        var teamElem = team as XmlElement;
                        if (teamElem == null)
                            continue;
                        DX2Data.Team t = new DX2Data.Team();
                        if (t.Load(teamElem, AppData.GetInst().DemonDB, AppData.GetInst().Skills))
                            AppData.GetInst().Teams.Add(t);
                        else
                        {
                            //todo message box
                        }
                    }
                }
                catch (Exception ex)
                {
                    //todo message box
                }
            }

            ContentSource = new Uri("Pages/TeamBuilder.xaml", UriKind.Relative);
        }

        public string Img
        {
            get { return "Demons/Ishtar.png"; }
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

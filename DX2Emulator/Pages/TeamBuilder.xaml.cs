using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DX2Emulator.Pages
{
    /// <summary>
    /// Interaction logic for TeamBuilder.xaml
    /// </summary>
    public partial class TeamBuilder : UserControl
    {
        public TeamBuilder()
        {
            InitializeComponent();

            teamList.ItemsSource = AppData.GetInst().Teams;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var team = teamList.SelectedValue as DX2Data.Team;
            if (team == null)
            { 
                frame.Source = new Uri("/Pages/Null.xaml", UriKind.Relative);
                return;
            }

            frame.Source = new Uri(string.Format("/Pages/MainScreen.xaml#{0}", AppData.GetInst().Teams.IndexOf(team)), UriKind.Relative);
        }

        private void Add_Team(object sender, EventArgs e)
        {
            AppData.GetInst().Teams.Add(new DX2Data.Team() { Name = "New Team" });
        }

        private void Clone_Team(object sender, EventArgs e)
        {
            var team = teamList.SelectedValue as DX2Data.Team;
            if (team == null)
                return;
            
            DX2Data.Team newTeam = new DX2Data.Team { Name = team.Name + "- Copy" };
            foreach (var d in team.Demons)
            {
                if (d == null)
                {
                    newTeam.Demons.Add(null);
                    continue;
                }
                else
                    newTeam.Demons.Add(d.Clone());
            }
            AppData.GetInst().Teams.Add(newTeam);
        }

        private void Delete_Team(object sender, EventArgs e)
        {
            var team = teamList.SelectedValue as DX2Data.Team;
            if (team == null)
                return;
            AppData.GetInst().Teams.Remove(team);
        }
    }
}

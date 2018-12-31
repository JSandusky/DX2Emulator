using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
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
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : UserControl, IContent
    {
        DX2Data.Team team;
        DX2Data.Demon aDemon;
        DX2Data.Demon bDemon;
        DX2Data.Demon cDemon;
        DX2Data.Demon dDemon;

        public MainScreen()
        {
            InitializeComponent();
        }

        public string img
        {
            get { return "Demons/Ishtar.png"; }
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            int idx = int.Parse(e.Fragment);

            team = AppData.GetInst().Teams[idx];
            teamName.DataContext = team;

            while (team.Demons.Count < 4)
                team.Demons.Add(null);

            aCard.Team = team;
            bCard.Team = team;
            cCard.Team = team;
            dCard.Team = team;

            aCard.Slot = 0;
            bCard.Slot = 1;
            cCard.Slot = 2;
            dCard.Slot = 3;

            aCard.DataContext = team.Demons[0];
            bCard.DataContext = team.Demons[1];
            cCard.DataContext = team.Demons[2];
            dCard.DataContext = team.Demons[3];
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (team != null)
            {
                aCard.DataContext = team.Demons[0];
                bCard.DataContext = team.Demons[1];
                cCard.DataContext = team.Demons[2];
                dCard.DataContext = team.Demons[3];
            }
        }
    }
}

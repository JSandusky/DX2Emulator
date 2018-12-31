using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DX2Data;
using DX2Emulator.Converters;

namespace DX2Emulator.Controls
{
    /// <summary>
    /// Interaction logic for DemonCard.xaml
    /// </summary>
    public partial class DemonCard : UserControl
    {
        public Team Team { get; set; }
        public int Slot { get; set; }

        public DemonCard()
        {
            InitializeComponent();

            Image[] srcs = new Image[]
            {
                new Image { Source = ArchImageConverter.clear, Height = 20 },
                new Image { Source = ArchImageConverter.red, Height = 20 },
                new Image { Source = ArchImageConverter.yellow, Height = 20 },
                new Image { Source = ArchImageConverter.purple, Height = 20 },
                new Image { Source = ArchImageConverter.teal, Height = 20 },
            };

            string[] names = new string[]
            {
                "Clear",
                "Red",
                "Yellow",
                "Purple",
                "Teal"
            };

            for (int i = 0; i < names.Length; ++i)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Children.Add(srcs[i]);
                sp.Children.Add(new Label { Content = names[i] });
                archCombo.Items.Add(sp);
            }

            var demons = AppData.GetInst().DemonDB.DemonNames;
            demonCombo.Items.Add("--- none ---");
            foreach (var d in demons)
                demonCombo.Items.Add(d);

            demonCombo.SelectionChanged += DemonCombo_SelectionChanged;
            archCombo.SelectionChanged += ArchCombo_SelectionChanged;

            SetupTransferCombo(transfer1);
            SetupTransferCombo(transfer2);

            transfer1.SelectionChanged += Transfer1_SelectionChanged;
            transfer2.SelectionChanged += Transfer2_SelectionChanged;

            DataContextChanged += DemonCard_DataContextChanged;
        }

        private void Transfer2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataContextChanging)
                return;

            Demon d = DataContext as Demon;
            if (d != null)
            {
                d.Skills[5] = AppData.GetInst().Skills.FirstOrDefault(s => s.Name == transfer2.SelectedValue.ToString());
                d.RecalcStats();
                ForceReload();
            }
        }

        private void Transfer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataContextChanging)
                return;

            Demon d = DataContext as Demon;
            if (d != null)
            {
                d.Skills[4] = AppData.GetInst().Skills.FirstOrDefault(s => s.Name == transfer1.SelectedValue.ToString());
                d.RecalcStats();
                ForceReload();
            }
        }

        private void ArchCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataContextChanging)
                return;

            Demon d = DataContext as Demon;
            if (d != null)
            {
                var kind = AppData.GetInst().DemonDB.Demons.FirstOrDefault(s => s.Name == d.Kind.Name && s.Archetype == (Archetype)archCombo.SelectedIndex);
                if (kind != null)
                {
                    d.SetKind(kind, true, true);
                    ForceReload();                    
                }
            }
        }

        bool dataContextChanging = false;

        private void DemonCard_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            dataContextChanging = true;

            if (DataContext != null)
            {
                Demon d = DataContext as Demon;
                demonCombo.SelectedValue = d.Kind.Name;
                archCombo.SelectedIndex = (int)d.Kind.Archetype;
                if (d.Kind.GachaSkill != null)
                    transfer1.SelectedItem = d.Kind.GachaSkill.Name;
                else
                    transfer1.SelectedIndex = 0;

                if (d.Skills[4] != null)
                    transfer1.SelectedItem = d.Skills[4].Name;
                else
                    transfer1.SelectedIndex = 0;

                if (d.Skills[5] != null)
                    transfer2.SelectedItem = d.Skills[5].Name;
                else
                    transfer2.SelectedIndex = 0;
            }
            else
                demonCombo.SelectedIndex = 0;

            dataContextChanging = false;
        }

        private void DemonCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataContextChanging)
                return;

            if (demonCombo.SelectedIndex == 0)
            {
                Team.Demons[Slot] = null;
                DataContext = null;
                ForceReload();
                return;
            }

            Demon d = DataContext as Demon;
            if (d != null)
            {
                var kind = AppData.GetInst().DemonDB.Demons.FirstOrDefault(db => db.Name == demonCombo.SelectedValue && db.Archetype == d.Kind.Archetype);
                d.SetKind(kind, true, true);
            }
            else
            {
                Team.Demons[Slot] = new Demon();
                var kind = AppData.GetInst().DemonDB.Demons.FirstOrDefault(db => db.Name == demonCombo.SelectedValue && db.Archetype == Archetype.Clear);
                Team.Demons[Slot].SetKind(kind, true, true);
                DataContext = Team.Demons[Slot];
            }
            ForceReload();
        }

        void SetupTransferCombo(ComboBox box)
        {
            box.Items.Add("--- empty ---");
            for (int i = 0; i < AppData.GetInst().Skills.Count; ++i)
                box.Items.Add(AppData.GetInst().Skills[i].Name);
        }

        void ForceReload()
        {
            var oldDc = DataContext;
            DataContext = null;
            DataContext = oldDc;
        }
    }
}

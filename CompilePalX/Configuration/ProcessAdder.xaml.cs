﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CompilePalX
{
    /// <summary>
    /// Interaction logic for ProcessAdder.xaml
    /// </summary>
    public partial class ProcessAdder
    {
        private class SteamIDPropertyGroup : PropertyGroupDescription
        {
            private readonly int? SteamAppID;
            public SteamIDPropertyGroup(int? steamAppID)
            {
                this.SteamAppID = steamAppID;
            }

            // Split processes into 2 groups, IsCompatible and Incompatible
            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                // should never happen
                if (item is not CompileProcess p) return "IsCompatible";

                return p.IsCompatible ? "IsCompatible" : "Incompatible";
            }
        }
        public string ChosenItem;
        public ProcessAdder()
        {
            InitializeComponent();

            ICollectionView processView = CollectionViewSource.GetDefaultView(ConfigurationManager.CompileProcesses);
            using (processView.DeferRefresh())
            {
                processView.GroupDescriptions.Clear();
                processView.GroupDescriptions.Add(new SteamIDPropertyGroup(GameConfigurationManager.GameConfiguration?.SteamAppID));
            }
            ProcessDataGrid.DataContext = processView;
        }

        private void ConfigDataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // walk up dependency tree to make sure click source was not a group header
            DependencyObject? dep = e.OriginalSource as DependencyObject;
            while ((dep != null) && !(dep is GroupItem) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            // ignore if double click came from group item
            if (dep is GroupItem)
                return;

            // only close if they actually selected an item
            if (ProcessDataGrid.SelectedItem != null)
                Close();
        }
    }
}

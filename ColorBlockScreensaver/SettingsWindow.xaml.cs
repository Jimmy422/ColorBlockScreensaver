#region copyright
// C# Screensaver Template Copyright (c) 2015 Wm. Barrett Simms wbsimms.com
// Base template modified by James Keith
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
#endregion
using Microsoft.Win32;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaveSim;

namespace ColorBlockScreensaver
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public SettingsWindow()
        {
            InitializeComponent();
            loadSettings();
        }

        private void saveSettings()
        {
            //checkBox_animBounce
            RegistryKey newKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ColorBlockScreensaver");
            newKey.SetValue("autoDark", checkBox_autodark.IsChecked);
            newKey.SetValue("changeColors", checkBox_changeColors.IsChecked);
            newKey.SetValue("hexCodes", checkBox_hexCodes.IsChecked);
            newKey.SetValue("animMode", checkBox_animMode.IsChecked);
            newKey.SetValue("animBounce", checkBox_animBounce.IsChecked);
            newKey.SetValue("sweepSpeed", textBox_sweepSpeed.Text);
            newKey.SetValue("blockPixelOffset", textBox_blockPixelOffset.Text);
            newKey.SetValue("runoffBlocks", textBox_runoffBlocks.Text);
            newKey.SetValue("blockWidth", textBox_blockWidth.Text);
            newKey.SetValue("fontSize", textBox_fontSize.Text);
            newKey.SetValue("blockTimeOffset", textBox_blockTimeOffset.Text);
            
        }

        private void loadSettings()
        {
            RegistryKey newKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\ColorBlockScreensaver");
            if(newKey == null)
            {
                checkBox_autodark.IsChecked = false;
                checkBox_changeColors.IsChecked = true;
                checkBox_hexCodes.IsChecked = false;
                checkBox_animMode.IsChecked = false;
                checkBox_animBounce.IsChecked = true;
                textBox_sweepSpeed.Text = "15";
                textBox_blockPixelOffset.Text = "100";
                textBox_runoffBlocks.Text = "8";
                textBox_blockWidth.Text = "125";
                textBox_fontSize.Text = "36";
                textBox_blockTimeOffset.Text = "0.75";
            }
            else
            {
                checkBox_autodark.IsChecked = Convert.ToBoolean(newKey.GetValue("autoDark"));
                checkBox_changeColors.IsChecked = Convert.ToBoolean(newKey.GetValue("changeColors"));
                checkBox_hexCodes.IsChecked = Convert.ToBoolean(newKey.GetValue("hexCodes"));
                checkBox_animMode.IsChecked = Convert.ToBoolean(newKey.GetValue("animMode"));
                checkBox_animBounce.IsChecked = Convert.ToBoolean(newKey.GetValue("animBounce"));
                textBox_sweepSpeed.Text = (string)newKey.GetValue("sweepSpeed");
                textBox_blockPixelOffset.Text = (string)newKey.GetValue("blockPixelOffset");
                textBox_runoffBlocks.Text = (string)newKey.GetValue("runoffBlocks");
                textBox_blockWidth.Text = (string)newKey.GetValue("blockWidth");
                textBox_fontSize.Text = (string)newKey.GetValue("fontSize");
                textBox_blockTimeOffset.Text = (string)newKey.GetValue("blockTimeOffset");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            this.Close();
        }

        private void button_quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Content = "Enabled";
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Content = "Disabled";
        }

        private void checkBox_animMode_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Content = "Fade";
        }

        private void checkBox_animMode_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Content = "Sweep";
        }
    }
}

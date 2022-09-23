﻿using System.Windows;
using AdmProperties = AutoDarkModeConfig.Properties;

namespace AutoDarkModeApp
{
    /// <summary>
    /// Interaction logic for MsgBox.xaml
    /// Parameters:
    /// Icons: error, info, update, smiley
    /// Buttons: close, yesno
    /// </summary>
    public partial class MsgBox
    {
        public MsgBox(string pMessageText, string pTitle, string pIcon, string pButton)
        {
            InitializeComponent();
            Text_Textblock.Text = pMessageText;
            Title = pTitle;
            PickIcon(pIcon);
            ButtonLayout(pButton);
        }

        private void PickIcon(string pIcon)
        {
            if (pIcon.Equals("error"))
            {
                IconTextBlock.Text = "\xEA39";
            }
            else if (pIcon.Equals("info"))
            {
                IconTextBlock.Text = "\xE946";
            }
            else if (pIcon.Equals("update"))
            {
                IconTextBlock.Text = "\xECC5";
            }
            else if (pIcon.Equals("smiley"))
            {
                IconTextBlock.Text = "\xED54";
            }
        }

        private void ButtonLayout(string pButton)
        {
            if (pButton.Equals("close"))
            {
                CloseButton.Content = AdmProperties.Resources.msgClose;
                YesButton.Visibility = Visibility.Hidden;
            }
            else if (pButton.Equals("yesno"))
            {
                CloseButton.Content = AdmProperties.Resources.msgNo;
                YesButton.Content = AdmProperties.Resources.MsgYes;
            }
            else if (pButton.Equals("none"))
            {
                CloseButton.Visibility = Visibility.Hidden;
                YesButton.Visibility = Visibility.Hidden;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

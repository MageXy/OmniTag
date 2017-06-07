﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using NCGLib.WPF.Templates;
using NCGLib.WPF.Utility;
using OmniTagWPF.ViewModels;
//using WpfControls;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for EditOmniView.xaml
    /// </summary>
    public partial class EditOmniView : NCGLibView
    {
        public EditOmniView()
        {
            InitializeComponent();
        }

        private void OnTagTextChanged(object sender, TextCompositionEventArgs e)
        {
            var vm = DataContext as EditOmniViewModel;
            if (vm == null)
                return;

            if (TagTextBox.SelectionLength == 0)
                vm.SearchText = TagTextBox.Text;
            else
                vm.SearchText = TagTextBox.Text.Substring(TagTextBox.SelectionStart);
        }

        private void OnWebBrowserLinkClicked(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri == null) // is navigating to string HTML
                return;

            var proc = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = e.Uri.ToString()
            };
            Process.Start(proc);
            e.Cancel = true;
        }

        private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as TabControl).SelectedItem as TabItem).Header as string == "Preview")
            {
                var vm = DataContext as EditOmniViewModel;
                if (vm == null)
                    return;

                vm.PreviewHtml();
            }
        }

        private void SurroundDescriptionTextWith(string characters, string tempText)
        {
            SurroundDescriptionTextWith(characters, characters, tempText);
        }

        private void SurroundDescriptionTextWith(string leftChars, string rightChars, string tempText)
        {
            string part1;
            string part2;
            string part3;

            if (DescriptionTextBox.SelectionLength > 0)
            {
                var startIndex = DescriptionTextBox.SelectionStart;
                var stringLength = DescriptionTextBox.SelectionLength;

                part1 = DescriptionTextBox.Text.Substring(0, startIndex);
                part2 = DescriptionTextBox.Text.Substring(startIndex, stringLength);
                part3 = DescriptionTextBox.Text.Substring(startIndex + stringLength);

                DescriptionTextBox.Text = part1 + leftChars + part2 + rightChars + part3;
                DescriptionTextBox.SelectionStart = startIndex + leftChars.Length;
                DescriptionTextBox.SelectionLength = stringLength;
            }
            else
            {
                var startIndex = DescriptionTextBox.CaretIndex;

                part1 = DescriptionTextBox.Text.Substring(0, startIndex);
                part2 = leftChars + tempText + rightChars;
                part3 = DescriptionTextBox.Text.Substring(startIndex);

                DescriptionTextBox.Text = part1 + part2 + part3;
                DescriptionTextBox.SelectionStart = startIndex + leftChars.Length;
                DescriptionTextBox.SelectionLength = tempText.Length;
            }
            DescriptionTextBox.Focus();
        }

        private void OnBoldClicked(object sender, RoutedEventArgs e)
        {
            SurroundDescriptionTextWith("**", "bold text");
        }

        private void OnItalicsClicked(object sender, RoutedEventArgs e)
        {
            SurroundDescriptionTextWith("*", "emphasized text");
        }

        private void OnStrikeClicked(object sender, RoutedEventArgs e)
        {
            SurroundDescriptionTextWith("<strike>", "</strike>", "strike text");
        }

        private void OnCodeClicked(object sender, RoutedEventArgs e)
        {
            SurroundDescriptionTextWith("`", "code text");
        }

        private void OnAddImageClicked(object sender, RoutedEventArgs e)
        {
            string result;

            if (sender == AddImageLocalButton)
            {
                var ofd = new OpenFileDialog
                {
                    Title = "Select image file...",
                    Multiselect = false,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };
                var ofdResult = ofd.ShowDialog();
                if ((ofdResult ?? false) == false)
                    return;

                result = ofd.FileName;
            }
            else
            {
                var vm = new SimpleInputViewModel("Enter file URL:", "Add Image");
                var view = ViewFactory.CreateViewWithDataContext<SimpleInputView>(vm);
                view.ShowDialog();
                if (vm.IsCancelled)
                    return;

                result = vm.Input;
            }

            try
            {
                var uri = new Uri(result);
                SurroundDescriptionTextWith("![", $"]({uri.AbsoluteUri})", "image alt text");
            }
            catch (UriFormatException)
            {
                SurroundDescriptionTextWith("![", $"]({result})", "image alt text");
            }
        }

        private void OnAddHyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var vm = new SimpleInputViewModel("Enter the URL:", "Add Hyperlink");
            vm.Input = "https://";
            var view = ViewFactory.CreateViewWithDataContext<SimpleInputView>(vm);
            view.ShowDialog();
            if (vm.IsCancelled)
                return;

            var result = vm.Input;
            SurroundDescriptionTextWith("[", $"]({result})", "link display text");
        }

        private void OnAddTableClicked(object sender, RoutedEventArgs e)
        {
            var tableSizeVm = new TableMarkdownSizeViewModel("Enter table size:", "Enter table size");
            var view = ViewFactory.CreateViewWithDataContext<TableMarkdownSizeView>(tableSizeVm);
            view.ShowDialog();

            if (tableSizeVm.IsCancelled)
                return;

            var tableVm = new TableMarkdownViewModel(tableSizeVm.NumCols, tableSizeVm.NumRows);
            view = ViewFactory.CreateViewWithDataContext<TableMarkdownView>(tableVm);
            view.ShowDialog();

            if (!tableVm.IsCancelled)
            {
                var startIndex = DescriptionTextBox.CaretIndex;

                var part1 = DescriptionTextBox.Text.Substring(0, startIndex);
                var part2 = "\n" + tableVm.GetTableString();
                var part3 = DescriptionTextBox.Text.Substring(startIndex);

                DescriptionTextBox.Text = part1 + part2 + part3;
                DescriptionTextBox.Focus();
            }
        }
    }
}

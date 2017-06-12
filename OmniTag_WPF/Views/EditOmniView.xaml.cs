using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ColorCode;
using Microsoft.Win32;
using NCGLib.WPF.Templates;
using NCGLib.WPF.Utility;
using OmniTagWPF.Utility;
using OmniTagWPF.ViewModels;
using OmniTagWPF.Views.Controls;

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

        private void OnTagTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                return;

            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;
            var textBox = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);

            if (textBox.SelectionLength > 0)
            {
                textBox.SelectionLength = 0;
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
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
            var startLineIndex = DescriptionTextBox.GetLineIndexFromCharacterIndex(DescriptionTextBox.SelectionStart);
            var endLineIndex = DescriptionTextBox.GetLineIndexFromCharacterIndex(DescriptionTextBox.SelectionStart +
                                                                                 DescriptionTextBox.SelectionLength);

            if (startLineIndex != endLineIndex)
            {
                var collection = ColorCode.Languages.All.Select(l => l.Name).OrderBy(n => n).ToList();
                collection.Insert(0, "None");
                var vm = new ComboInputViewModel<string>(collection, "Please choose a code language:");
                var view = ViewFactory.CreateViewWithDataContext<ComboInputView>(vm);
                view.ShowDialog();

                var language = String.Empty;
                if (vm.UserConfirmed && vm.SelectedItem != "None")
                    language = vm.SelectedItem;

                for (var index = startLineIndex; index <= endLineIndex; index++)
                {
                    var lineStartIndex = DescriptionTextBox.GetCharacterIndexFromLineIndex(index);
                    DescriptionTextBox.Text = DescriptionTextBox.Text.Insert(lineStartIndex, "    ");
                }

                if (language != String.Empty)
                {
                    DescriptionTextBox.Text =
                        DescriptionTextBox.Text.Insert(
                            DescriptionTextBox.GetCharacterIndexFromLineIndex(startLineIndex),
                            "    " + OmniTextRenderer.LangDefinitionText + language + Environment.NewLine);
                    endLineIndex++;
                }
                DescriptionTextBox.SelectionStart = DescriptionTextBox.GetCharacterIndexFromLineIndex(startLineIndex);
                var endOfLastLineIndex = DescriptionTextBox.GetCharacterIndexFromLineIndex(endLineIndex) +
                                         DescriptionTextBox.GetLineText(endLineIndex).Length;
                DescriptionTextBox.SelectionLength = endOfLastLineIndex - DescriptionTextBox.SelectionStart;

                DescriptionTextBox.Focus();
            }
            else
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

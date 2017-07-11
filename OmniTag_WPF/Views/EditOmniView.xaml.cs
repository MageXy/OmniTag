using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using NCGLib.WPF.Utility;
using NCGLib.WPF.Utility.Input;
using OmniTagWPF.Utility;
using OmniTagWPF.ViewModels;

//using WpfControls;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for EditOmniView.xaml
    /// </summary>
    public partial class EditOmniView : CenteredView
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
            var textwrap = DescriptionTextBox.TextWrapping;
            if (textwrap == TextWrapping.Wrap)
                DescriptionTextBox.TextWrapping = TextWrapping.NoWrap;
            
            var startLineIndex = DescriptionTextBox.GetLineIndexFromCharacterIndex(DescriptionTextBox.SelectionStart);
            var endLineIndex = DescriptionTextBox.GetLineIndexFromCharacterIndex(DescriptionTextBox.SelectionStart +
                                                                                 DescriptionTextBox.SelectionLength);

            if (startLineIndex != endLineIndex)
            {
                var collection = ColorCode.Languages.All.Select(l => l.Name).OrderBy(n => n).ToList();
                collection.Insert(0, "None");
                var vm = InputViewFactory.ShowComboBoxInput("Please choose a code language:", "Code Language", collection);
                
                var language = String.Empty;
                if (vm.UserCancelled)
                {
                    DescriptionTextBox.TextWrapping = textwrap;
                    return;
                }
                
                if (vm.SelectedValue != "None")
                    language = vm.SelectedValue;
                
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

            DescriptionTextBox.TextWrapping = textwrap;
        }

        private void OnAddImageClicked(object sender, RoutedEventArgs e)
        {
            string result;

            if (sender == AddImageLocalButton)
            {
                var ofd = SelectImage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (ofd.FileName == null)
                    return;

                result = ofd.FileName;
            }
            else if (sender == AddImageOnlineButton)
            {
                //var vm = new SimpleInputViewModel("Enter file URL:", "Add Image");
                //var view = ViewFactory.CreateViewWithDataContext<SimpleInputView>(vm);
                //view.ShowDialog();
                var vm = InputViewFactory.ShowTextBoxInput("Enter file URL:", "Add Image");
                if (vm.UserCancelled)
                    return;

                result = vm.SelectedValue;
            }
            else
            {
                var ofd = SelectImage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (ofd == null)
                    return;

                var vm = DataContext as EditOmniViewModel;
                if (vm == null)
                    return;

                var imageDesc = vm.EmbedImage(ofd.FileName, ofd.SafeFileName).Replace(" ", "%20");
                
                SurroundDescriptionTextWith("![",$@"](dbfile:///{imageDesc})", "image description");
                return;
            }

            try
            {
                var uri = new Uri(result);
                SurroundDescriptionTextWith("![", $"]({uri.AbsoluteUri})", "image description");
            }
            catch (UriFormatException)
            {
                SurroundDescriptionTextWith("![", $"]({result})", "image alt text");
            }
        }

        private void OnAddHyperlinkClicked(object sender, RoutedEventArgs e)
        {
            var vm = new TextBoxInputViewModel("Enter the URL:", "Add Hyperlink");
            vm.SelectedValue = "https://";
            var view = ViewFactory.CreateViewWithDataContext<TextBoxInputView>(vm);
            view.ShowDialog();
            if (vm.UserCancelled)
                return;

            var result = vm.SelectedValue;
            SurroundDescriptionTextWith("[", $"]({result})", "link display text");
        }

        private void OnAddTableClicked(object sender, RoutedEventArgs e)
        {
            var tableSizeVm = new TableMarkdownSizeViewModel("Enter table size:", "Enter table size");
            var view = ViewFactory.CreateViewWithDataContext<TableMarkdownSizeView>(tableSizeVm);
            view.ShowDialog();

            if (tableSizeVm.UserCancelled)
                return;

            var tableVm = new TableMarkdownViewModel(tableSizeVm.NumCols, tableSizeVm.NumRows);
            view = ViewFactory.CreateViewWithDataContext<TableMarkdownView>(tableVm);
            view.ShowDialog();

            if (!tableVm.UserCancelled)
            {
                var startIndex = DescriptionTextBox.CaretIndex;

                var part1 = DescriptionTextBox.Text.Substring(0, startIndex);
                var part2 = $"\n{tableVm.SelectedValue}\n";
                var part3 = DescriptionTextBox.Text.Substring(startIndex);

                DescriptionTextBox.Text = part1 + part2 + part3;
                DescriptionTextBox.Focus();
            }
        }

        private void ToggleTextWrapping(object sender, RoutedEventArgs e)
        {
            DescriptionTextBox.TextWrapping = (DescriptionTextBox.TextWrapping == TextWrapping.NoWrap)
                ? TextWrapping.Wrap
                : TextWrapping.NoWrap;
        }

        public OpenFileDialog SelectImage(string initialDirectory)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select image file...",
                Multiselect = false,
                InitialDirectory = initialDirectory
            };
            var ofdResult = ofd.ShowDialog();
            if ((ofdResult ?? false) == false)
                return null;

            return ofd;
        }
    }
}

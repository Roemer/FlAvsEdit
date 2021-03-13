using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;

namespace FlAvsEdit.Gui.Controls
{
    public class AvsEditorControl : TextEditor
    {
        CompletionWindow completionWindow;

        public AvsEditorControl()
        {
            // General settings
            ShowLineNumbers = true;
            FontFamily = new FontFamily("Consolas");
            FontSize = 13;
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("AviSynth");

            // Add the search panel
            SearchPanel.Install(this);

            // Events
            TextArea.MouseWheel += TextArea_MouseWheel;
            TextArea.TextEntering += TextArea_TextEntering;
            TextArea.TextEntered += TextArea_TextEntered;
        }

        private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (e.Delta > 0)
                {
                    TextArea.FontSize = Math.Min(TextArea.FontSize + 1, 72);
                }
                else if (e.Delta < 0)
                {
                    TextArea.FontSize = Math.Max(TextArea.FontSize - 1, 8);
                }
            }
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(TextArea);
                var data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                data.Add(new MyCompletionData("xItem1"));
                data.Add(new MyCompletionData("xItem2"));
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //if (e.Key == Key.Space && e.HasModifiers(ModifierKeys.Control))
            //{
            //    e.Handled = true;
            //    var mode = e.HasModifiers(ModifierKeys.Shift)
            //        ? TriggerMode.SignatureHelp
            //        : TriggerMode.Completion;
            //    var task = ShowCompletion(mode);
            //}
        }
    }

    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            this.Text = text;
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get
            {
                var result = new System.Windows.Controls.TextBlock { TextWrapping = System.Windows.TextWrapping.Wrap };

                System.Collections.Generic.IEnumerable<TaggedText> text = new List<TaggedText>()
                {
                    new TaggedText("Keyword", "Hallo"),
                    new TaggedText("", " "),
                    new TaggedText("Class", "Welt"),
                };
                foreach (var part in text)
                {
                    var s = part.Text;
                    var run = new Run(s);
                    switch (part.Tag)
                    {
                        case "Keyword":
                            run.Foreground = Brushes.Blue;
                            break;
                        case "Struct":
                        case "Enum":
                        case "TypeParameter":
                        case "Class":
                        case "Delegate":
                        case "Interface":
                            run.Foreground = Brushes.Teal;
                            break;
                    }

                    result.Inlines.Add(run);
                }

                return result;
            }
        }

        public double Priority => 1.0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }

    public class TaggedText
    {
        //
        // Summary:
        //     Creates a new instance of Microsoft.CodeAnalysis.TaggedText
        //
        // Parameters:
        //   tag:
        //     A descriptive tag from Microsoft.CodeAnalysis.TextTags.
        //
        //   text:
        //     The actual text to be displayed.
        public TaggedText(string tag, string text)
        {
            Tag = tag;
            Text = text;
        }

        //
        // Summary:
        //     A descriptive tag from Microsoft.CodeAnalysis.TextTags.
        public string Tag { get; set; }
        //
        // Summary:
        //     The actual text to be displayed.
        public string Text { get; set; }

    }
}

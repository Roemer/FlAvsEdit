using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using FlAvsEdit.Gui.Core;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace FlAvsEdit.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var reader = new XmlTextReader(new StringReader(ResourceGetter.GetContentResource("/Resources/AviSynthSyntax.xshd"))))
            {
                HighlightingManager.Instance.RegisterHighlighting("AviSynth", Array.Empty<string>(),
                    HighlightingLoader.Load(reader, HighlightingManager.Instance));

                var def = HighlightingManager.Instance.GetDefinition("AviSynth");
                var rules = def.MainRuleSet.Rules;

                var customKeywordRule = new HighlightingRule();
                customKeywordRule.Color = rules[0].Color;
                var wordList = new string[]
                {
                    "switch",
                    "case",
                };
                customKeywordRule.Regex = new Regex($@"\b({string.Join("|", wordList)})\w*\b");
                def.MainRuleSet.Rules.Add(customKeywordRule);
            }
        }
    }
}

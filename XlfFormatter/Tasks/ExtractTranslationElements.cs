using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XlfFormatter.Extensions;
using XlfFormatter.Models;
using XlfFormatter.Utilities;

namespace XlfFormatter.Tasks
{
    public class ExtractTranslationElements : BaseUtility
    {
        private const string WebDir = @"My.App.Web\site";
        private const string TransDir = @"app\i18n";
        private const string MessagesXlf = @"messages.xlf";
        private readonly string _messagesFile = Path.Combine(ProjectPath, Path.Combine(WebDir, MessagesXlf));
        private readonly string _transMsgFiles = Path.Combine(ProjectPath, Path.Combine(WebDir, TransDir));

        private IEnumerable<string> _mainMessagesIds;
        private IEnumerable<XElement> _mainMessagesElements;


        public override void Run()
        {
            Log.Msg("Begin Extract Translation Elements");

            LoadMainMessages();
            CrawlTranslationFiles();

            Log.Msg("End Extract Translation Elements");
        }

        private void CrawlTranslationFiles()
        {
            var transFiles = Directory.GetFiles(_transMsgFiles, "*.xlf");

            foreach (var transFile in transFiles)
            {
                var filename = Path.GetFileName(transFile);

                Log.Msg($"Begin extract for {filename}");

                var main = XElement.Load(transFile);
                var ns = main.Name.Namespace;
                var transUnitElements = main.GetTransUnitElements(ns);
                var tItems = transUnitElements.ToTranslationItems(ns);
                var transIds = tItems.Select(x => x.Id);

                // always generate this before removing/adding elements to preserve translated 'target'
                var localizedLookUp = GetLocalizedDictionary(tItems);

                RemoveElements(transUnitElements, transIds.Except(_mainMessagesIds));
                AddElements(transUnitElements.FirstOrDefault().Parent, _mainMessagesIds.Except(transIds), localizedLookUp, ns);

                main.Save(transFile);

                Log.Msg($"End extract for {filename}");
            }
        }

        private void AddElements(XElement parent, IEnumerable<string> addItems, IDictionary<string, string> lookUp, XNamespace ns)
        {
            foreach (var addItem in addItems)
            {
                var cloned = new XElement(_mainMessagesElements.FirstOrDefault(x => x.Attribute("id").Value == addItem));
                var sourceKey = cloned.Element(ns + "source").Value.ToLowerNoWhitespaces();
                var targetValue = cloned.Element(ns + "source").Value;

                if (lookUp.TryGetValue(sourceKey, out var translatedTarget))
                    targetValue = translatedTarget;

                var target = new XElement("target", targetValue);
                cloned.Element(ns + "source").AddAfterSelf(target);
                cloned.Attribute("datatype").RemoveExisting();

                parent.Add(cloned);
            }
        }

        private void RemoveElements(IEnumerable<XElement> transElements, IEnumerable<string> removeItems)
        {
            var list = transElements.Where(x => removeItems.Contains(x.Attribute("id").Value));
            foreach (var t in list)
                t.Remove();
        }

        private void LoadMainMessages()
        {
            var main = XElement.Load(_messagesFile);
            var ns = main.Name.Namespace;
            _mainMessagesElements = main
                .GetTransUnitElements(ns);
            _mainMessagesIds = _mainMessagesElements
                .ToTranslationItems(ns)
                .Select(x => x.Id);
        }

        private static IDictionary<string, string> GetLocalizedDictionary(IEnumerable<TranslationItem> items)
        {
            return items
                .GroupBy(x => x.Source.ToLowerNoWhitespaces())
                .Select(x => new
                {
                    SourceKey = x.Key,
                    x.FirstOrDefault()?.Target
                })
                .ToDictionary(d => d.SourceKey, d => d.Target);
        }
    }
}

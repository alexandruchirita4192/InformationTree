using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace InformationTree.TextProcessing
{
    [Obsolete("Break this into several classes")]
    // TODO: Split class into it's purposes: TreeNodeData (or text) processing (like an extension of TreeNodeData??)
    public static class TextProcessingHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // TODO: 1. Find common code because it seems copy-paste and both could call an internal method
        // TODO: 2. Move to a string extension class or TreeNodeData class
        // (need to see it's use cases, how it's used when it's called to see it's proper place and if the method could do more or not)
        public static string GetTextAndProcentCompleted(string attrText, ref decimal attrPercentCompleted, bool getTextWithoutProgress = false)
        {
            // The format of text is:
            // 1. Text [X% completed]
            // 2. Text text2 text3 ][[ [X% completed]
            // Where X is attrPercentCompleted
            var words = attrText.Trim().Split('[');
            if (words.Length < 2)
            {
            }
            else if (words.Length == 2) // 1. Text [X% completed]
            {
                attrText = words[0].TrimEnd(); // fix attrText to be simple
                try
                {
                    attrPercentCompleted = decimal.Parse(words[1].Split('%')[0]); // fix attrPercentCompleted from text (if necessary)
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }
            else if (words.Length > 2) // 2. Text text2 text3 ][[ [X% completed]
            {
                try
                {
                    attrPercentCompleted = decimal.Parse(words[words.Length - 1].Split('%')[0]);
                    attrText = string.Join("[", words.Except(new List<string>() { words[words.Length - 1] })).Trim();
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }

            if (attrPercentCompleted < 0)
                attrPercentCompleted = 0;
            else if (attrPercentCompleted > 100)
                attrPercentCompleted = 100;

            if (!getTextWithoutProgress)
                attrText = attrText.Trim() + " [" + attrPercentCompleted.ToString() + "% completed]";
            attrText = attrText.Trim();

            return attrText;
        }

        public static string UpdateTextAndProcentCompleted(string attrText, ref decimal attrPercentCompleted, bool getTextWithoutProgress = false)
        {
            if (attrPercentCompleted < 0)
                attrPercentCompleted = 0;
            else if (attrPercentCompleted > 100)
                attrPercentCompleted = 100;

            var words = attrText.Trim().Split('[');
            if (words.Length < 2)
            {
            }
            else if (words.Length == 2)
                attrText = words[0].TrimEnd(); // fix attrText to be simple
            else if (words.Length > 2)
            {
                try
                {
                    attrText = string.Join("[", words.Except(new List<string>() { words[words.Length - 1] }));
                }
                catch // either there's no % or there's no number
                {
                    attrText = string.Join("[", words); // fix it
                }
            }

            if (!getTextWithoutProgress)
                attrText = attrText.Trim() + " [" + attrPercentCompleted.ToString() + "% completed]";
            attrText = attrText.Trim();

            return attrText;
        }

        public static string GetToolTipText(string text, int linesCount = 10, int charsCount = 200)
        {
            try
            {
                var lines = text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > linesCount)
                    return string.Join(Environment.NewLine, lines.Take(linesCount)) + Environment.NewLine + "[...]";
                else
                    return string.Join(Environment.NewLine, lines) + Environment.NewLine;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            if (text.Length <= charsCount)
                return text;

            return new string(text.Take(charsCount).ToArray()) + "[...]";
        }
    }
}
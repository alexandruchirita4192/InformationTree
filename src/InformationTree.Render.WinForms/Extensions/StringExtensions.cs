using System;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using NLog;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class StringExtensions
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static string StripRTF(this string rtfString)
        {
            var result = rtfString;

            try
            {
                if (rtfString.IsRichText())
                {
                    // Put body into a RichTextBox so we can strip RTF
                    using var rtfTemp = new RichTextBox();
                    
                    rtfTemp.Rtf = rtfString;
                    result = rtfTemp.Text;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }
    }
}
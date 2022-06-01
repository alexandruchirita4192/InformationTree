using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeViewExtensions
    {
        public static string[] GenerateStringGraphicsLinesFromTree(this TreeView tvTaskList)
        {
            if (tvTaskList == null)
                throw new ArgumentNullException(nameof(tvTaskList));

            var lines = new List<string>();

            foreach (TreeNode task in tvTaskList.Nodes)
                lines.Add(task.Text);

            return lines.ToArray();
        }
    }
}
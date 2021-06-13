using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InformationTree.Forms
{
    public partial class SearchForm : Form
    {
        public string TextToFind { get { return tbFind.Text.Replace("\n", "").Replace("\r",""); } set { tbFind.Text = value; } }

        public SearchForm(string text = null)
        {
            InitializeComponent();
            TextToFind = text;

            if (text != null)
                tbFind.Select(text.Length, 0);
        }

        private void DoSearch(object sender, EventArgs e)
        {
            if (tbFind.Text.Length > 3)
            {
                //MessageBox.Show(tbFind.Text);
                //tbFind.Text = string.Empty;
                this.Close();
            }
        }

        private void tbFind_KeyUp(object sender, KeyEventArgs e)
        {
            if(object.ReferenceEquals(tbFind, sender) && e.KeyData == Keys.Enter)
                DoSearch(sender, e);

            if (e.KeyData == Keys.Escape)
            {
                tbFind.Text = null;
                this.Close();
            }
        }
    }
}

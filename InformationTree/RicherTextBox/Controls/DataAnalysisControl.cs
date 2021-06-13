using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RicherTextBox.Controls
{
    public partial class DataAnalysisControl : UserControl
    {
        #region Consts
        const string DataAnalysisTableSignature = "AnalysisTable";
        const string DefaultSplitter = "#";
        #endregion Consts

        #region Properties
        private BindingSource bindingSource;
        private DataTable dataTable;
        private RicherTextBox parent;
        private int selectionStart, selectionLength;
        #endregion Properties

        #region Constructors
        public DataAnalysisControl()
        {
            InitializeComponent();
        }

        public DataAnalysisControl(DataTable dataTableRef, RicherTextBox parent)
            : this()
        {
            this.parent = parent;

            selectionStart = parent != null ? parent.TextBox.SelectionStart : 0;
            selectionLength = parent != null ? parent.TextBox.SelectionLength : 0;

            if(parent != null)
            {
                parent.TextBox.Controls.Add(this);
                TextBox_Resize(this, EventArgs.Empty);
                parent.TextBox.Resize += TextBox_Resize;
            }

            if (dataTableRef == null) // nothing to bind here, but create a table
                dataTableRef = new DataTable();

            dataTable = dataTableRef;
            bindingSource = new BindingSource();

            if (dataGridView.ColumnCount > 0)
                dataGridView.Columns.Clear();

            bindingSource.DataSource = dataTableRef;
            dataGridView.DataSource = bindingSource;
        }

        private void TextBox_Resize(object sender, EventArgs e)
        {
            var positionStart = parent.TextBox.GetPositionFromCharIndex(selectionStart);
            this.Location = positionStart;
            this.Width = parent.TextBox.Width - 25;
            this.Height = parent.TextBox.Height - 25;
        }

        public DataAnalysisControl(string initializationData, RicherTextBox parent)
            : this(GetDataTableFromString(initializationData), parent)
        {
        }
        #endregion Constructors

        #region Methods
        public static DataTable GetDataTableFromString(string initializationData)
        {
            var dataTable = new DataTable();

            if (string.IsNullOrEmpty(initializationData))
                return dataTable;

            var lines = initializationData.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            if (lines.Count == 0)
                return dataTable;

            var lineZeroIsInfo = lines[0].StartsWith(DataAnalysisTableSignature);

            string[] splitter;

            if (lineZeroIsInfo)
            {
                splitter = new string[1] { lines[0].Substring(DataAnalysisTableSignature.Length) };

                lines.RemoveAt(0);

                if (lines.Count == 0)
                    return dataTable;
            }
            else
                splitter = new string[1] { DefaultSplitter };

            foreach (var line in lines)
            {
                var items = line.Split(splitter, StringSplitOptions.None);
                
                if(dataTable.Columns.Count < items.Length)
                    for (int i = dataTable.Columns.Count; i < items.Length; i++)
                        dataTable.Columns.Add(new DataColumn("Column " + i.ToString(), typeof(string)));

                dataTable.Rows.Add(items);
            }
            return dataTable;
        }

        public static string GetStringFromDataTable(DataTable dataTableRef = null, BindingSource bindingSourceRef = null, string splitter = null)
        {
            if (dataTableRef == null && bindingSourceRef != null && bindingSourceRef.DataSource is DataTable)
                dataTableRef = bindingSourceRef.DataSource as DataTable;

            if (dataTableRef == null)
                return string.Empty;

            var result = DataAnalysisTableSignature + DefaultSplitter + Environment.NewLine;
            splitter = string.IsNullOrEmpty(splitter) ? DefaultSplitter : splitter;

            foreach(DataRow row in dataTableRef.Rows)
                result += string.Join(splitter, row.ItemArray) + Environment.NewLine;

            return string.IsNullOrEmpty(result) ? string.Empty : result;
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            var len = dataTable.Columns.Count;
            if (dataTable != null)
                for (int i = len; i < len + 1; i++)
                    dataTable.Columns.Add(new DataColumn("Column " + i.ToString(), typeof(string)));
        }

        private void btnSaveTableData_Click(object sender, EventArgs e)
        {
            if(parent != null)
            {
                parent.TextBox.SelectionStart = selectionStart;
                parent.TextBox.SelectionLength = selectionLength;
                parent.TextBox.SelectedText = GetStringFromDataTable(dataTable, bindingSource, DefaultSplitter);

                this.Hide();

                if (parent.TextBox.Controls.Contains(this))
                    parent.TextBox.Controls.Remove(this);
                Dispose();
            }
        }

        private new void Dispose()
        {
            if (parent != null)
                parent.TextBox.Resize -= TextBox_Resize;
            parent = null;
            dataGridView = null;
            dataTable = null;

            base.Dispose();
        }
        #endregion Methods
    }
}

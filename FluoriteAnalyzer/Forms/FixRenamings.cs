﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.PatternDetectors;
using FluoriteAnalyzer.Pipelines;

namespace FluoriteAnalyzer.Forms
{
    public partial class FixRenamings : Form
    {
        public FixRenamings()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = true;
            openDialog.Filter = "Log Files|*.xml";

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            listBox1.Items.AddRange(openDialog.FileNames.Where(x => !listBox1.Items.Contains(x)).ToArray());
        }

        private void buttonRemoveFiles_Click(object sender, EventArgs e)
        {
            List<int> indices = listBox1.SelectedIndices.Cast<int>().OrderByDescending(x => x).ToList();
            foreach (int index in indices)
            {
                listBox1.Items.RemoveAt(index);
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Please add log files", "Error", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(textPostfix.Text))
            {
                MessageBox.Show("Please enter any postfix to be used, or this tool would overwrite existing files.", "Warning", MessageBoxButtons.OK);
                textPostfix.Focus();
                return;
            }

            List<FileInfo> fileInfos =
                listBox1.Items
                .Cast<string>()
                .OrderBy(x => x)
                .Select(x => new FileInfo(x))
                .ToList();

            FixRenamingsFromFile(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FixRenamingsFromFile(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(FixRenamingsFromFile(fileInfo));
            }

            MessageBox.Show(builder.ToString(), "RemoveTypos");
        }

        private string FixRenamingsFromFile(FileInfo fileInfo)
        {
            FixRenamingsFilter filter = new FixRenamingsFilter(string.Empty, textPostfix.Text);
            FileInfo resultingFile = filter.Compute(fileInfo);

            return string.Format("Renamings have been removed from \"{0}\"", fileInfo.FullName);
        }

    }
}

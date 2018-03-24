using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VocalUtau.Formats.Model.VocalSqlObject;

namespace VocalUtau.Formats.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ProjectVector vector = new ProjectVector();
        private void Form1_Load(object sender, EventArgs e)
        {
            string tempfile=ProjectVector.createNewProjectFile();
            vector.loadProjectFile(tempfile);
        }
    }
}

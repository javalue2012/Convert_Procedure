using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convert_Procedure
{
    public partial class Main : Form
    {
        public static string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\OuputFile.txt";
        private int button1WasClicked = 0;
        public Main()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Do you want to convert?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (result == DialogResult.No)
            {
                return;
            }
            if (result == DialogResult.Yes)
            {
                try
                {
                    string inputValue = txtInput.Text;
                    var tableName = txtTableName.Text;
                    string type = txtTypeName.Text;
                    string moduleName = txtModule.Text;

                    ConvertToProcedure(inputValue, tableName, type, moduleName);
                }
                catch (Exception err)
                {
                        MessageBox.Show(err.ToString());
                }
            }
        }

        private void ConvertToProcedure(string inputValue, string tableName, string type, string moduleName)
        {
            string procedure = $"CREATE PROCEDURE [{type}].[{moduleName}_Update_{tableName}]";
            var listColumns = inputValue.Split(',');
            var listParameter = new List<string>();
            var listParameter2 = new List<string>();
            var listColumnSet = new List<string>();
            foreach (var column in listColumns)
            {
                var columns = column.Replace("[", "").Replace("]", "").Replace("NULL", "").Replace("NOT", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
                listParameter.Add(columns);
            }
            listParameter.RemoveAt(listParameter.Count - 1);


            foreach (var parameter in listParameter)
            {
                procedure += $"\n    @{parameter},";
            }
            procedure = procedure.Remove(procedure.Length - 2);
            procedure += $" \nAS \nUPDATE [{type}].[{moduleName}_{tableName}] SET";

            foreach (var parameter in listParameter)
            {
                var firstSpaceIndex = parameter.IndexOf(" ");
                var columnName = parameter.Substring(0, firstSpaceIndex);
                listColumnSet.Add(columnName);
            }
           
            foreach (var Column in listColumnSet)
            {
                procedure += $"\n    [{Column}]  =  @{Column},";
            }

            procedure = procedure.Remove(procedure.Length - 1);
            procedure += $"\nWHERE [{listColumnSet[0]}] = @{listColumnSet[0]} \nGO";

            string executeQuery = $"EXEC  [{type}].[{moduleName}_Update_{tableName}]";
            for (int i = 0; i < listParameter.Count; i++)
            {
                executeQuery += $" ?,";
            }

            executeQuery = executeQuery.Remove(executeQuery.Length - 1);
            procedure += $"\n\n {executeQuery}";

            if (rdbOutputFile.Checked == true)
            {

                FileStream fParameter = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
                StreamWriter m_WriterParameter = new StreamWriter(fParameter);
                m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
                m_WriterParameter.Write(procedure);
                m_WriterParameter.Flush();
                m_WriterParameter.Close();
            }
            
            else if(rdbOutputText.Checked == true)
            {
                txtOutput.Text = procedure.ToString();
            }
        

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1WasClicked = 1;
            SaveEvent();
        }

        public void SaveEvent()
        {
            DialogResult result;
            result = MessageBox.Show("Do you want to convert?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (result == DialogResult.No)
            {
                return;
            }
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (txtInput.Text != null && button1WasClicked == 1)
                    {
                        saveFile(txtInput.Text, button1WasClicked);

                    }
                    else if (txtInput.Text != null && button1WasClicked == 2)
                    {
                        saveFile(txtInput.Text, button1WasClicked);

                    }
                    else if (txtInput.Text != null && button1WasClicked == 3)
                    {
                        saveFile(txtInput.Text, button1WasClicked);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private void saveFile(string text, int button1WasClicked)
        {
            var Msg = text.Split(',');
            var listParameter = new List<string>();

            if (button1WasClicked == 1)
            {

                string procedure = $"CREATE PROCEDURE [raw].[CAM_Update_ClassRun]";
                foreach (var column in Msg)
                {
                    var columnReplace = column.Replace("[", "").Replace("]", "").Replace("NULL", "").Replace("NOT", "");
                    listParameter.Add(columnReplace);
                }

                foreach (var parameter in listParameter)
                {
                    procedure += $"\n@{String.Concat(parameter.Where(c => !Char.IsWhiteSpace(c)))},";
                }

                FileStream fParameter1 = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
                StreamWriter m_WriterParameter1 = new StreamWriter(fParameter1);
                m_WriterParameter1.BaseStream.Seek(0, SeekOrigin.End);
                m_WriterParameter1.Write(procedure);
                m_WriterParameter1.Flush();
                m_WriterParameter1.Close();
            }
            if (button1WasClicked == 2)
            {
                foreach (var column in Msg)
                {

                    var columnReplace = column.Replace(",", "").Replace("[nvarchar](25)", "").Replace("[uniqueidentifier]", "").Replace("[datetime2](7)", "").Replace("[bit]", "").Replace("[nvarchar](max)", "").Replace("NULL", "").Replace("NOT", "");
                    listParameter.Add(columnReplace);
                }
                FileStream fParameter2 = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
                StreamWriter m_WriterParameter2 = new StreamWriter(fParameter2);
                m_WriterParameter2.BaseStream.Seek(0, SeekOrigin.End);
                m_WriterParameter2.Write(Msg);
                m_WriterParameter2.Flush();
                m_WriterParameter2.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

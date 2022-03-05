using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace QRbarkodolustur
{
    public partial class Form1 : Form
    {
        List<Bitmap> bmps;
        int index = 0;
        int genislik = 0;
        int yukseklik = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            
            Graphics graphicsImage = Graphics.FromImage(qrCodeImage);
            string Str_TextOnImage = "F14-105446-00-07";
            StringFormat stringformat = new StringFormat();

            Color StringColor = System.Drawing.Color.Black;//direct color adding  
            graphicsImage.DrawString(Str_TextOnImage, new Font("arial", 40,
            FontStyle.Regular), new SolidBrush(StringColor), new Point(qrCodeImage.Width / 5, (qrCodeImage.Height-(qrCodeImage.Height/10))),
            stringformat);


            pictureBox1.Image = qrCodeImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                bmps = new List<Bitmap>();

                if (textBox2.Text == "")
                {
                    MessageBox.Show("Adet Bilgisi girişi yapınız.");
                    return;
                }

                int index1 = Convert.ToInt32(textBox2.Text);

                for (int i = 1; i <= 10; i++)
                {
                    string yenikod = textBox1.Text + "|" + Convert.ToString(i).PadLeft(6, '0');
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(yenikod, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    Graphics graphicsImage = Graphics.FromImage(qrCodeImage);
                    string Str_TextOnImage = "F14-105446-00-07";
                    StringFormat stringformat = new StringFormat();

                    Color StringColor = System.Drawing.Color.Black;//direct color adding  
                    graphicsImage.DrawString(Str_TextOnImage, new Font("arial", 45,
                    FontStyle.Bold), new SolidBrush(StringColor), new Point(qrCodeImage.Width / 6, (qrCodeImage.Height - (qrCodeImage.Height / 10))),
                    stringformat);
                    Bitmap bmp = new Bitmap(qrCodeImage.Width, qrCodeImage.Height, graphicsImage);

                    bmps.Add(qrCodeImage);
                }



                //bmps.Add(qrCodeImage);

                /*
                genislik = qrCodeImage.Width;
                yukseklik = qrCodeImage.Height;
                MessageBox.Show(genislik.ToString()+"*"+yukseklik.ToString());
                */
                genislik = 60;
                yukseklik = 60;

                PrintDocument pd = new PrintDocument();

                pd.PrinterSettings.PrinterName = yazicidi.Text;

                pd.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Card", genislik, yukseklik);
                pd.DefaultPageSettings.Landscape = true;
                pd.DefaultPageSettings.PrinterResolution = new PrinterResolution
                {
                    Kind = PrinterResolutionKind.Custom,
                    X = 300,
                    Y = 300
                };

                pd.DefaultPageSettings.Margins = new Margins(2, 2, 2, 2);

                pd.PrintPage += new PrintPageEventHandler(imprimirDocumento);


                pd.Print();
                //PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                //previewDialog.Document = pd;
                //previewDialog.ShowDialog();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void imprimirDocumento(System.Object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(bmps[index++], 0, 0, genislik, yukseklik);
            if (index < bmps.Count) e.HasMorePages = true; else index = 0;
        }

        private void yazicidi_TextChanged(object sender, EventArgs e)
        {

        }
        String Con = "Provider=SQLNCLI10;Server=10.0.1.13; Database=UNVAS_2022; Uid=sa; Pwd=1q2w3e4r--";

        private void Form1_Load(object sender, EventArgs e)
        {
            

            String sql_query = "SELECT * FROM UFLD02E WITH(NOLOCK) WHERE TABLE1='STOK00' AND QS_VARCODE='MKODU' AND QS_VALUE<>'' ORDER BY KRITER";

            string sqlWorkorderModel = Con;
            SqlDataAdapter sqlDataAdapter;
            OleDbDataAdapter workOrdAdap, mainStockAdap, woDetailAdap, additionalStockAdap;
            DataTable dataWorkOrderModel, dataMainStockModel, dataWoDetailModel, dataAdditionalStockModel, dataTrexToDinamo;
            OleDbCommand oleDbCommand;
            OleDbConnection oleDbConnection;

            oleDbConnection = new OleDbConnection(sqlWorkorderModel);
            oleDbConnection.Open();

            workOrdAdap = new OleDbDataAdapter(sql_query, oleDbConnection);
            dataWorkOrderModel = new DataTable();
            workOrdAdap.Fill(dataWorkOrderModel);
            // MessageBox.Show(dataWorkOrderModel.Rows.Count.ToString());
            ComboboxItem item = new ComboboxItem();

            for (int j = 0; j < dataWorkOrderModel.Rows.Count; j++)
            {

                //item.Text = dataWorkOrderModel.Rows[j]["KRITER"].ToString().Trim();
                //item.Value = dataWorkOrderModel.Rows[j]["QS_VALUE"].ToString().Trim();
                //comboBox1.Items.Add(item);
                comboBox1.Items.Add(dataWorkOrderModel.Rows[j]["KRITER"].ToString().Trim());

            }
            this.Controls.Add(comboBox1);
            comboBox1.SelectedIndex = 0;
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selected = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
           // MessageBox.Show(selected);


            //ComboBox cmb = (ComboBox)sender;
            //int selectedIndex = cmb.SelectedIndex;
            //string selectedValue = (string)cmb.SelectedValue;
            //ComboboxItem selectedCar = (ComboboxItem)cmb.SelectedItem;
            //MessageBox.Show(String.Format("Index: [{0}] CarName={1}; Value={2}", selectedIndex, selectedCar.Text, selectedCar.Value));

            string sql = "SELECT QS_VALUE FROM UFLD02E WITH(NOLOCK) WHERE TABLE1='STOK00' AND QS_VARCODE='MKODU' AND QS_VALUE<>'' AND KRITER='" + selected + "'";

            string sqlWorkorderModel = Con;
            OleDbDataAdapter workOrdAdap;
            DataTable dataWorkOrderModel;
            OleDbConnection oleDbConnection;

            oleDbConnection = new OleDbConnection(sqlWorkorderModel);
            oleDbConnection.Open();

            workOrdAdap = new OleDbDataAdapter(sql, oleDbConnection);
            dataWorkOrderModel = new DataTable();
            workOrdAdap.Fill(dataWorkOrderModel);

            
            if (dataWorkOrderModel.Rows.Count > 0 )
            { 
                textBox1.Text =  dataWorkOrderModel.Rows[0]["QS_VALUE"].ToString().Trim();
            }

            

            //int selectedIndex = comboBox1.SelectedIndex;
            //comboBox1.SelectedItem.ToString();
            //string selectedValue = comboBox1.Items[selectedIndex].ToString();
            // ComboBox cmb = (ComboBox)sender;
            //int selectedIndex = cmb.SelectedIndex;
            //string selectedText = this.comboBox1.Text;
            //string selectedValue = ((ComboboxItem)comboBox1.SelectedItem).Value.ToString();
            //string selectedValue01 = comboBox1.Items.ToString();
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
            //   textBox1.Text = selectedValue;// comboBox1.SelectedItem.ToString();
            //textBox1.Text = comboBox1.SelectedValue.ToString();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}

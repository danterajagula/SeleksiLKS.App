using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using static Microsoft.Data.SqlClient.Internal.SqlClientEventSource;

namespace SeleksiLKS.App
{
    public partial class Form1 : Form
    {
        private readonly string
            connectionString = "Data Source=localhost\\TEW_SQLEXPRESS;Initial Catalog=ATKInventoryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        private int selectedId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TampilData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow || row.Cells["id"].Value == null)
                return;

            selectedId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

            textBox1.Text = row.Cells["itemCode"].Value?.ToString() ?? "";
            textBox2.Text = row.Cells["itemName"].Value?.ToString() ?? "";
            textBox3.Text = row.Cells["category"].Value?.ToString() ?? "";
            textBox4.Text = row.Cells["price"].Value?.ToString() ?? "";
            textBox5.Text = row.Cells["stock"].Value?.ToString() ?? "";

            MessageBox.Show("ID terpilih: " + selectedId);
        }

        private void BersihkanForm()
        {
            selectedId = 0;

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            dataGridView1.ClearSelection();
        }

        void TampilData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.items ORDER BY id ASC", conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.items (itemCode, itemName, category, price, stock) " +
                               "VALUES (@code, @name, @cat, @price, @stock)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@code", textBox1.Text);
                    cmd.Parameters.AddWithValue("@name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@cat", textBox3.Text);
                    cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(textBox4.Text));
                    cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(textBox5.Text));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Data berhasil ditambah!");
            TampilData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Pilih baris di tabel dulu!");
                return;
            }

            if (!decimal.TryParse(textBox4.Text, out decimal price) ||
                !int.TryParse(textBox5.Text, out int stock))
            {
                MessageBox.Show("Price harus angka desimal dan Stock harus angka bulat!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE dbo.items
                         SET itemCode = @code,
                             itemName = @name,
                             category = @cat,
                             price = @price,
                             stock = @stock
                         WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    cmd.Parameters.AddWithValue("@code", textBox1.Text);
                    cmd.Parameters.AddWithValue("@name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@cat", textBox3.Text);
                    cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@stock", SqlDbType.Int).Value = stock;

                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();

                    if (affected == 0)
                    {
                        MessageBox.Show("Data tidak ditemukan!");
                        return;
                    }
                }

                MessageBox.Show("Data sudah diubah");
                TampilData();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (selectedId == 0)
            {
                MessageBox.Show("Pilih baris di tabel dulu!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (selectedId == 0)
                {
                    MessageBox.Show("Pilih baris di tabel dulu!");
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Yakin ingin menghapus data ini?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result != DialogResult.Yes)
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.items WHERE id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = selectedId;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!");

                TampilData();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BersihkanForm();
        }
    }
}
    
  

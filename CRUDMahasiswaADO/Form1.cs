using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        string koneksi = "Data Source=LAPTOP-07AAA94J\\SQLEXPRESS;Initial Catalog=DBAkademikADO;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
            conn = new SqlConnection(koneksi);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                MessageBox.Show("Koneksi Berhasil!");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM Mahasiswa";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rd = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rd);
                dataGridView1.DataSource = dt;
                rd.Close();
                conn.Close();
                MessageBox.Show("Load Berhasil! Total: " + dt.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNIM.Text == "") { MessageBox.Show("NIM harus diisi"); return; }
                if (txtNama.Text == "") { MessageBox.Show("Nama harus diisi"); return; }
                if (cmbJK.Text == "") { MessageBox.Show("Jenis Kelamin harus dipilih"); return; }
                if (txtKodeProdi.Text == "") { MessageBox.Show("Kode Prodi harus diisi"); return; }

                conn.Open();
                string sql = "INSERT INTO Mahasiswa (NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi, TanggalDaftar) VALUES (@1,@2,@3,@4,@5,@6,GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@1", txtNIM.Text);
                cmd.Parameters.AddWithValue("@2", txtNama.Text);
                cmd.Parameters.AddWithValue("@3", cmbJK.Text);
                cmd.Parameters.AddWithValue("@4", dtpTanggalLahir.Value);
                cmd.Parameters.AddWithValue("@5", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@6", txtKodeProdi.Text);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Insert Berhasil!");
                btnLoad.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNIM.Text == "") { MessageBox.Show("Klik data yang mau diupdate di tabel!"); return; }

                conn.Open();
                string sql = "UPDATE Mahasiswa SET Nama=@2, JenisKelamin=@3, TanggalLahir=@4, Alamat=@5, KodeProdi=@6 WHERE NIM=@1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@1", txtNIM.Text);
                cmd.Parameters.AddWithValue("@2", txtNama.Text);
                cmd.Parameters.AddWithValue("@3", cmbJK.Text);
                cmd.Parameters.AddWithValue("@4", dtpTanggalLahir.Value);
                cmd.Parameters.AddWithValue("@5", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@6", txtKodeProdi.Text);
                int hasil = cmd.ExecuteNonQuery();
                conn.Close();

                if (hasil > 0)
                {
                    MessageBox.Show("Update Berhasil!");
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data tidak ditemukan!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNIM.Text == "") { MessageBox.Show("Klik data yang mau dihapus di tabel!"); return; }

                DialogResult confirm = MessageBox.Show("Hapus data " + txtNIM.Text + "?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.No) return;

                conn.Open();
                string sql = "DELETE FROM Mahasiswa WHERE NIM=@1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@1", txtNIM.Text);
                int hasil = cmd.ExecuteNonQuery();
                conn.Close();

                if (hasil > 0)
                {
                    MessageBox.Show("Delete Berhasil!");
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data tidak ditemukan!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtNIM.Text = row.Cells["NIM"].Value.ToString();
                txtNama.Text = row.Cells["Nama"].Value.ToString();
                cmbJK.Text = row.Cells["JenisKelamin"].Value.ToString();
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtKodeProdi.Text = row.Cells["KodeProdi"].Value.ToString();
                if (row.Cells["TanggalLahir"].Value != DBNull.Value)
                {
                    dtpTanggalLahir.Value = Convert.ToDateTime(row.Cells["TanggalLahir"].Value);
                }
                MessageBox.Show("Data terpilih! Silakan UPDATE atau DELETE.", "Info");
            }
        }
    }
}

// COMMIT UDAH BENER DAN FINAL
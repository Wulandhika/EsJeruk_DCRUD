using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

// Komponen yang ditambahkan:
// - txtNIM, txtNama, txtAlamat, txtKodeProdi
// - cmbJK (items: L, P)
// - dtpTanggalLahir
// - btnConnect, btnLoad, btnInsert, btnUpdate, btnDelete
// - dataGridView1
// - labels yang sesuai

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        private SqlConnection conn;
        private string connectionString = @"Data Source=LAPTOP-07AAA94J\SQLEXPRESS;Initial Catalog=DBAkademikADO;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Isi ComboBox JK
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");
            cmbJK.DropDownStyle = ComboBoxStyle.DropDownList;

            // Set awal tombol
            btnLoad.Enabled = false;
            btnInsert.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void OpenConnection()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        private void CloseConnection()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        private void ClearForm()
        {
            txtNIM.Text = "";
            txtNama.Text = "";
            cmbJK.SelectedIndex = -1;
            dtpTanggalLahir.Value = DateTime.Now;
            txtAlamat.Text = "";
            txtKodeProdi.Text = "";
            txtNIM.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
            {
                MessageBox.Show("NIM harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIM.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNama.Focus();
                return false;
            }
            if (cmbJK.SelectedItem == null)
            {
                MessageBox.Show("Jenis Kelamin harus dipilih!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbJK.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtKodeProdi.Text))
            {
                MessageBox.Show("Kode Prodi harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKodeProdi.Focus();
                return false;
            }
            return true;
        }

        // TOMBOL CONNECT
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                OpenConnection();
                MessageBox.Show("Koneksi ke database berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnLoad.Enabled = true;
                btnInsert.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal koneksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        // TOMBOL LOAD / MENAMPILKAN DATA
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenConnection();
                string query = "SELECT NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi FROM Mahasiswa";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView1.DataSource = dt;
                reader.Close();
                CloseConnection();

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                MessageBox.Show($"Data berhasil dimuat. Total: {dt.Rows.Count} record", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TOMBOL INSERT / MENAMBAH DATA
        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                OpenConnection();
                string query = @"INSERT INTO Mahasiswa (NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi, TanggalDaftar) 
                                VALUES (@NIM, @Nama, @JK, @TanggalLahir, @Alamat, @KodeProdi, @TanggalDaftar)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", txtNIM.Text.Trim());
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                cmd.Parameters.AddWithValue("@JK", cmbJK.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text.Trim());
                cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text.Trim());
                cmd.Parameters.AddWithValue("@TanggalDaftar", DateTime.Now);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Data berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    btnLoad.PerformClick();
                }
                CloseConnection();
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("NIM sudah terdaftar! Gunakan NIM lain.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TOMBOL UPDATE / MENGUBAH DATA
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNIM.Text))
                {
                    MessageBox.Show("Pilih data dari DataGridView terlebih dahulu!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!ValidateInput()) return;

                OpenConnection();
                string query = @"UPDATE Mahasiswa 
                                SET Nama = @Nama, 
                                    JenisKelamin = @JK, 
                                    TanggalLahir = @TanggalLahir, 
                                    Alamat = @Alamat, 
                                    KodeProdi = @KodeProdi 
                                WHERE NIM = @NIM";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", txtNIM.Text.Trim());
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                cmd.Parameters.AddWithValue("@JK", cmbJK.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text.Trim());
                cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text.Trim());

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Data berhasil diupdate!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data tidak ditemukan!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




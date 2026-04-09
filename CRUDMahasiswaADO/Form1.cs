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


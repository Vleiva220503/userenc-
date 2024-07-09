using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LoginPermisos
{
    public partial class MostrarUsuarios : Form
    {
        private Conexion conexion;

        public MostrarUsuarios()
        {
            InitializeComponent();
            conexion = new Conexion();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            DataTable usuarios = conexion.MostrarTodosLosUsuarios();
            dataGridView1.DataSource = usuarios;

            // Configurar DataGridView para que no muestre las contraseñas en texto plano.
            if (dataGridView1.Columns.Contains("password"))
            {
                dataGridView1.Columns["password"].Visible = false;
            }

            // Configurar eventos de DataGridView.
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataRowView rowView = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
                if (rowView != null)
                {
                    DataRow row = rowView.Row;

                    // Mostrar la foto.
                    byte[] fotoBytes = row["foto"] as byte[];
                    if (fotoBytes != null && fotoBytes.Length > 0)
                    {
                        try
                        {
                            using (var ms = new System.IO.MemoryStream(fotoBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
                            pictureBox1.Image = null;
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
            }
        }

 
    }
}

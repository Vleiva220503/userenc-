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
using Konscious.Security.Cryptography;

namespace LoginPermisos
{
    public partial class Form1 : Form
    {
        private Conexion conexion;

        public Form1()
        {
            InitializeComponent();
            conexion = new Conexion();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; // Solo seleccionable
            // Add event handlers for text fields
            textBox2.TextChanged += NombreApellidoTextChanged;
            textBox3.TextChanged += NombreApellidoTextChanged;
            textBox4.TextChanged += NombreApellidoTextChanged;
            textBox5.TextChanged += NombreApellidoTextChanged;
            textBox6.TextChanged += CorreoTextChanged;
            //ocualtando password
            textBox7.UseSystemPasswordChar = true;
            //dimension de imagen
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            // cursor al inicio de los  maskedTextBox
            maskedTextBox1.Enter += MaskedTextBox_Enter;
            maskedTextBox2.Enter += MaskedTextBox_Enter;
            //mostrar password
            checkBoxShowPassword.CheckedChanged += TogglePasswordVisibility;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = textBox1.Text.Trim();
                string pnombre = textBox2.Text.Trim();
                string snombre = textBox3.Text.Trim();
                string papellido = textBox4.Text.Trim();
                string sapellido = textBox5.Text.Trim();
                string correo = textBox6.Text.Trim().ToLower();
                string password = textBox7.Text;
                string privilegios = comboBox1.SelectedItem?.ToString();
                byte[] foto = GetFotoBytes();
                string numeroTelefono = maskedTextBox1.Text.Trim();
                string telefonoEmergencia = maskedTextBox2.Text.Trim();

                if (string.IsNullOrEmpty(usuario))
                {
                    MessageBox.Show("El campo de usuario es obligatorio.");
                    textBox1.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(pnombre))
                {
                    MessageBox.Show("El primer nombre es obligatorio.");
                    textBox2.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(papellido))
                {
                    MessageBox.Show("El primer apellido es obligatorio.");
                    textBox4.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(correo))
                {
                    MessageBox.Show("El correo electrónico es obligatorio.");
                    textBox6.Focus();
                    return;
                }

                if (!correo.EndsWith("@gmail.com") && !correo.EndsWith("@outlook.com") && !correo.EndsWith("@hotmail.com"))
                {
                    MessageBox.Show("El correo electrónico debe ser de dominio @gmail.com, @outlook.com, o @hotmail.com.");
                    textBox6.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("La contraseña es obligatoria.");
                    textBox7.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(privilegios))
                {
                    MessageBox.Show("Debe seleccionar un privilegio.");
                    comboBox1.Focus();
                    return;
                }

                if (foto == null)
                {
                    MessageBox.Show("Debe seleccionar una foto.");
                    pictureBox1.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(numeroTelefono))
                {
                    MessageBox.Show("El número de teléfono es obligatorio.");
                    maskedTextBox1.Focus();
                    return;
                }

                if (numeroTelefono.Length != 8 || (numeroTelefono[0] != '5' && numeroTelefono[0] != '7' && numeroTelefono[0] != '8'))
                {
                    MessageBox.Show("El número de teléfono debe iniciar con 7, 8 o 5 y tener exactamente 8 caracteres.");
                    maskedTextBox1.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(telefonoEmergencia))
                {
                    MessageBox.Show("El teléfono de emergencia es obligatorio.");
                    maskedTextBox2.Focus();
                    return;
                }

                if (telefonoEmergencia.Length != 8 || (telefonoEmergencia[0] != '5' && telefonoEmergencia[0] != '7' && telefonoEmergencia[0] != '8'))
                {
                    MessageBox.Show("El teléfono de emergencia debe iniciar con 7, 8 o 5 y tener exactamente 8 caracteres.");
                    maskedTextBox2.Focus();
                    return;
                }

                if (privilegios != "admin" && privilegios != "cajero" && privilegios != "bodega")
                {
                    MessageBox.Show("Privilegios inválidos. Debe ser uno de los siguientes: admin, cajero, bodega.");
                    comboBox1.Focus();
                    return;
                }

                // Cifrar la contraseña con Argon2
                byte[] passwordHash = HashPassword(password);

                // Llamar al método AgregarUsuario de la clase Conexion
                conexion.AgregarUsuario(usuario, pnombre, snombre, papellido, sapellido, correo, passwordHash, privilegios, foto, numeroTelefono, telefonoEmergencia);

                MessageBox.Show("Usuario agregado exitosamente.");

                // Limpiar los campos después de guardar
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonSelectPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtener la ruta del archivo seleccionada
                    string filePath = openFileDialog.FileName;

                    // Cargar la imagen en el PictureBox
                    pictureBox1.Image = Image.FromFile(filePath);
                }
            }
        }

        private byte[] GetFotoBytes()
        {
            if (pictureBox1.Image == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private byte[] HashPassword(string password)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            // Configuración de Argon2
            argon2.Salt = Encoding.UTF8.GetBytes("somesalt"); // Debes usar un salt único y seguro
            argon2.DegreeOfParallelism = 8; // Número de hilos
            argon2.MemorySize = 1024 * 64; // Cantidad de memoria a usar en KB
            argon2.Iterations = 4; // Número de iteraciones

            return argon2.GetBytes(128); // Tamaño del hash en bytes
        }

        private void NombreApellidoTextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string originalText = textBox.Text;
            string processedText = ProcessName(originalText);

            if (originalText != processedText)
            {
                int selectionStart = textBox.SelectionStart;
                textBox.Text = processedText;
                textBox.SelectionStart = selectionStart; // maintain the cursor position
            }
        }

        private void CorreoTextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string originalText = textBox.Text;
            string processedText = originalText.ToLower().Replace(" ", "");

            if (originalText != processedText)
            {
                int selectionStart = textBox.SelectionStart;
                textBox.Text = processedText;
                textBox.SelectionStart = selectionStart; // maintain the cursor position
            }
        }

        private string ProcessName(string name)
        {
            string[] words = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join("", words);
        }

        // Método para limpiar los campos del formulario
        private void LimpiarCampos()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            comboBox1.SelectedIndex = -1;
            maskedTextBox1.Clear();
            maskedTextBox2.Clear();
            pictureBox1.Image = null; // Limpiar la foto
            textBox1.Focus();
        }

        private void MaskedTextBox_Enter(object sender, EventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null)
            {
                maskedTextBox.SelectionStart = 0;
            }
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            textBox7.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }
    }
}

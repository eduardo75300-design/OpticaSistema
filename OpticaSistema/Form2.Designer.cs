namespace OpticaSistema
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblUsuario = new Label();
            txtContrasena = new TextBox();
            txtUsuario = new TextBox();
            btnIngresar = new Button();
            lblContraseña = new Label();
            lblLogin = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            textBox2 = new TextBox();
            label2 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            label4 = new Label();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(46, 51);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(56, 15);
            lblUsuario.TabIndex = 11;
            lblUsuario.Text = "Nombres";
            lblUsuario.Click += lblUsuario_Click;
            // 
            // txtContrasena
            // 
            txtContrasena.Location = new Point(46, 122);
            txtContrasena.Margin = new Padding(3, 2, 3, 2);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.Size = new Size(204, 23);
            txtContrasena.TabIndex = 10;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(46, 68);
            txtUsuario.Margin = new Padding(3, 2, 3, 2);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(204, 23);
            txtUsuario.TabIndex = 9;
            txtUsuario.TextChanged += txtUsuario_TextChanged;
            // 
            // btnIngresar
            // 
            btnIngresar.Location = new Point(576, 278);
            btnIngresar.Margin = new Padding(3, 2, 3, 2);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(82, 22);
            btnIngresar.TabIndex = 8;
            btnIngresar.Text = "INGRESAR";
            btnIngresar.UseVisualStyleBackColor = true;
            // 
            // lblContraseña
            // 
            lblContraseña.AutoSize = true;
            lblContraseña.Location = new Point(46, 105);
            lblContraseña.Name = "lblContraseña";
            lblContraseña.Size = new Size(56, 15);
            lblContraseña.TabIndex = 7;
            lblContraseña.Text = "Apellidos";
            lblContraseña.Click += lblContraseña_Click;
            // 
            // lblLogin
            // 
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(46, 24);
            lblLogin.Name = "lblLogin";
            lblLogin.Size = new Size(60, 15);
            lblLogin.TabIndex = 6;
            lblLogin.Text = "PACIENTE";
            lblLogin.Click += lblLogin_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(46, 228);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(334, 23);
            textBox1.TabIndex = 13;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(46, 211);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 12;
            label1.Text = "Direccion";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(46, 289);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(204, 23);
            textBox2.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 272);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 14;
            label2.Text = "Telefono";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(46, 174);
            textBox3.Margin = new Padding(3, 2, 3, 2);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(204, 23);
            textBox3.TabIndex = 17;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(46, 157);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 16;
            label3.Text = "Correo";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(46, 329);
            label4.Name = "label4";
            label4.Size = new Size(68, 15);
            label4.TabIndex = 18;
            label4.Text = "Estado Civil";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(46, 347);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(204, 23);
            comboBox1.TabIndex = 20;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBox1);
            Controls.Add(label4);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(lblUsuario);
            Controls.Add(txtContrasena);
            Controls.Add(txtUsuario);
            Controls.Add(btnIngresar);
            Controls.Add(lblContraseña);
            Controls.Add(lblLogin);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUsuario;
        private TextBox txtContrasena;
        private TextBox txtUsuario;
        private Button btnIngresar;
        private Label lblContraseña;
        private Label lblLogin;
        private TextBox textBox1;
        private Label label1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox3;
        private Label label3;
        private Label label4;
        private ComboBox comboBox1;
    }
}
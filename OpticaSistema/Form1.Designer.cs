namespace OpticaSistema
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            btnIngresar = new Button();
            txtUsuario = new TextBox();
            txtContrasena = new TextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(651, 77);
            label1.Name = "label1";
            label1.Size = new Size(51, 20);
            label1.TabIndex = 0;
            label1.Text = "LOGIN";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(665, 252);
            label2.Name = "label2";
            label2.Size = new Size(50, 20);
            label2.TabIndex = 1;
            label2.Text = "label2";
            // 
            // btnIngresar
            // 
            btnIngresar.Location = new Point(641, 322);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(94, 29);
            btnIngresar.TabIndex = 2;
            btnIngresar.Text = "button1";
            btnIngresar.UseVisualStyleBackColor = true;
            btnIngresar.Click += btnIngresar_Click;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(627, 200);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(125, 27);
            txtUsuario.TabIndex = 3;
            txtUsuario.TextChanged += txtUsuario_TextChanged;
            // 
            // txtContrasena
            // 
            txtContrasena.Location = new Point(627, 275);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.Size = new Size(125, 27);
            txtContrasena.TabIndex = 4;
            txtContrasena.TextChanged += textBox2_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(665, 167);
            label3.Name = "label3";
            label3.Size = new Size(50, 20);
            label3.TabIndex = 5;
            label3.Text = "label3";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label3);
            Controls.Add(txtContrasena);
            Controls.Add(txtUsuario);
            Controls.Add(btnIngresar);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button btnIngresar;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Label label3;
    }
}

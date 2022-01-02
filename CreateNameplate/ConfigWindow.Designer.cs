
namespace CreateNameplate
{
    partial class ConfigWindow
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
            this.labelVWS = new System.Windows.Forms.Label();
            this.labelProjectName = new System.Windows.Forms.Label();
            this.textBoxAAS = new System.Windows.Forms.TextBox();
            this.textBoxProjectName = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelVWS
            // 
            this.labelVWS.AutoSize = true;
            this.labelVWS.Location = new System.Drawing.Point(50, 45);
            this.labelVWS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVWS.Name = "labelVWS";
            this.labelVWS.Size = new System.Drawing.Size(238, 20);
            this.labelVWS.TabIndex = 0;
            this.labelVWS.Text = "REST-Endpoint of AASX-Server";
            // 
            // labelProjectName
            // 
            this.labelProjectName.AutoSize = true;
            this.labelProjectName.Location = new System.Drawing.Point(54, 122);
            this.labelProjectName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProjectName.Name = "labelProjectName";
            this.labelProjectName.Size = new System.Drawing.Size(105, 20);
            this.labelProjectName.TabIndex = 1;
            this.labelProjectName.Text = "Project-Name";
            // 
            // textBoxAAS
            // 
            this.textBoxAAS.Location = new System.Drawing.Point(303, 42);
            this.textBoxAAS.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxAAS.Name = "textBoxAAS";
            this.textBoxAAS.Size = new System.Drawing.Size(353, 26);
            this.textBoxAAS.TabIndex = 2;
            this.textBoxAAS.Text = "http://localhost:51310";
            // 
            // textBoxProjectName
            // 
            this.textBoxProjectName.Location = new System.Drawing.Point(303, 119);
            this.textBoxProjectName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxProjectName.Name = "textBoxProjectName";
            this.textBoxProjectName.Size = new System.Drawing.Size(353, 26);
            this.textBoxProjectName.TabIndex = 3;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(545, 191);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(112, 35);
            this.buttonSend.TabIndex = 5;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.ButtonSend_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(58, 191);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 35);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ConfigWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 264);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxProjectName);
            this.Controls.Add(this.textBoxAAS);
            this.Controls.Add(this.labelProjectName);
            this.Controls.Add(this.labelVWS);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ConfigWindow";
            this.Text = "Konfigurationseingabe";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelVWS;
        private System.Windows.Forms.Label labelProjectName;
        private System.Windows.Forms.TextBox textBoxAAS;
        private System.Windows.Forms.TextBox textBoxProjectName;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button button1;
    }
}
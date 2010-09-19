namespace Eimu
{
    partial class StartDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_RomPath = new System.Windows.Forms.TextBox();
            this.button_FileBrowse = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_CPUModeRecompiler = new System.Windows.Forms.RadioButton();
            this.radioButton_CPUModeInterpreter = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton_GraphicsXna = new System.Windows.Forms.RadioButton();
            this.radioButton_GraphicsDirect3d = new System.Windows.Forms.RadioButton();
            this.radioButton_GraphicsOpengl = new System.Windows.Forms.RadioButton();
            this.radioButton_GraphicsDrawing = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_AudioXna = new System.Windows.Forms.RadioButton();
            this.radioButton_AudioDirectSound = new System.Windows.Forms.RadioButton();
            this.radioButton_AudioOpenal = new System.Windows.Forms.RadioButton();
            this.radioButton_AudioBeep = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButton_InputXna = new System.Windows.Forms.RadioButton();
            this.radioButton_InputDirectinput = new System.Windows.Forms.RadioButton();
            this.radioButton_InputSdl = new System.Windows.Forms.RadioButton();
            this.radioButton_InputWinForms = new System.Windows.Forms.RadioButton();
            this.button_RunProgram = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_RomPath);
            this.groupBox1.Controls.Add(this.button_FileBrowse);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ROM File";
            // 
            // textBox_RomPath
            // 
            this.textBox_RomPath.Location = new System.Drawing.Point(7, 19);
            this.textBox_RomPath.Name = "textBox_RomPath";
            this.textBox_RomPath.ReadOnly = true;
            this.textBox_RomPath.Size = new System.Drawing.Size(208, 20);
            this.textBox_RomPath.TabIndex = 1;
            // 
            // button_FileBrowse
            // 
            this.button_FileBrowse.Location = new System.Drawing.Point(221, 16);
            this.button_FileBrowse.Name = "button_FileBrowse";
            this.button_FileBrowse.Size = new System.Drawing.Size(75, 23);
            this.button_FileBrowse.TabIndex = 0;
            this.button_FileBrowse.Text = "Browse...";
            this.button_FileBrowse.UseVisualStyleBackColor = true;
            this.button_FileBrowse.Click += new System.EventHandler(this.button_FileBrowse_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_CPUModeRecompiler);
            this.groupBox2.Controls.Add(this.radioButton_CPUModeInterpreter);
            this.groupBox2.Location = new System.Drawing.Point(12, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 64);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CPU Emulation Mode";
            // 
            // radioButton_CPUModeRecompiler
            // 
            this.radioButton_CPUModeRecompiler.AutoSize = true;
            this.radioButton_CPUModeRecompiler.Location = new System.Drawing.Point(7, 41);
            this.radioButton_CPUModeRecompiler.Name = "radioButton_CPUModeRecompiler";
            this.radioButton_CPUModeRecompiler.Size = new System.Drawing.Size(78, 17);
            this.radioButton_CPUModeRecompiler.TabIndex = 1;
            this.radioButton_CPUModeRecompiler.TabStop = true;
            this.radioButton_CPUModeRecompiler.Text = "Recompiler";
            this.radioButton_CPUModeRecompiler.UseVisualStyleBackColor = true;
            // 
            // radioButton_CPUModeInterpreter
            // 
            this.radioButton_CPUModeInterpreter.AutoSize = true;
            this.radioButton_CPUModeInterpreter.Checked = true;
            this.radioButton_CPUModeInterpreter.Location = new System.Drawing.Point(7, 20);
            this.radioButton_CPUModeInterpreter.Name = "radioButton_CPUModeInterpreter";
            this.radioButton_CPUModeInterpreter.Size = new System.Drawing.Size(73, 17);
            this.radioButton_CPUModeInterpreter.TabIndex = 0;
            this.radioButton_CPUModeInterpreter.TabStop = true;
            this.radioButton_CPUModeInterpreter.Text = "Interpreter";
            this.radioButton_CPUModeInterpreter.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton_GraphicsXna);
            this.groupBox3.Controls.Add(this.radioButton_GraphicsDirect3d);
            this.groupBox3.Controls.Add(this.radioButton_GraphicsOpengl);
            this.groupBox3.Controls.Add(this.radioButton_GraphicsDrawing);
            this.groupBox3.Location = new System.Drawing.Point(321, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(140, 118);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Graphics Backend";
            // 
            // radioButton_GraphicsXna
            // 
            this.radioButton_GraphicsXna.AutoSize = true;
            this.radioButton_GraphicsXna.Enabled = false;
            this.radioButton_GraphicsXna.Location = new System.Drawing.Point(7, 89);
            this.radioButton_GraphicsXna.Name = "radioButton_GraphicsXna";
            this.radioButton_GraphicsXna.Size = new System.Drawing.Size(47, 17);
            this.radioButton_GraphicsXna.TabIndex = 3;
            this.radioButton_GraphicsXna.Text = "XNA";
            this.radioButton_GraphicsXna.UseVisualStyleBackColor = true;
            // 
            // radioButton_GraphicsDirect3d
            // 
            this.radioButton_GraphicsDirect3d.AutoSize = true;
            this.radioButton_GraphicsDirect3d.Enabled = false;
            this.radioButton_GraphicsDirect3d.Location = new System.Drawing.Point(7, 66);
            this.radioButton_GraphicsDirect3d.Name = "radioButton_GraphicsDirect3d";
            this.radioButton_GraphicsDirect3d.Size = new System.Drawing.Size(67, 17);
            this.radioButton_GraphicsDirect3d.TabIndex = 2;
            this.radioButton_GraphicsDirect3d.Text = "Direct3D";
            this.radioButton_GraphicsDirect3d.UseVisualStyleBackColor = true;
            // 
            // radioButton_GraphicsOpengl
            // 
            this.radioButton_GraphicsOpengl.AutoSize = true;
            this.radioButton_GraphicsOpengl.Enabled = false;
            this.radioButton_GraphicsOpengl.Location = new System.Drawing.Point(7, 43);
            this.radioButton_GraphicsOpengl.Name = "radioButton_GraphicsOpengl";
            this.radioButton_GraphicsOpengl.Size = new System.Drawing.Size(65, 17);
            this.radioButton_GraphicsOpengl.TabIndex = 1;
            this.radioButton_GraphicsOpengl.Text = "OpenGL";
            this.radioButton_GraphicsOpengl.UseVisualStyleBackColor = true;
            // 
            // radioButton_GraphicsDrawing
            // 
            this.radioButton_GraphicsDrawing.AutoSize = true;
            this.radioButton_GraphicsDrawing.Checked = true;
            this.radioButton_GraphicsDrawing.Location = new System.Drawing.Point(7, 20);
            this.radioButton_GraphicsDrawing.Name = "radioButton_GraphicsDrawing";
            this.radioButton_GraphicsDrawing.Size = new System.Drawing.Size(101, 17);
            this.radioButton_GraphicsDrawing.TabIndex = 0;
            this.radioButton_GraphicsDrawing.TabStop = true;
            this.radioButton_GraphicsDrawing.Text = "System.Drawing";
            this.radioButton_GraphicsDrawing.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_AudioXna);
            this.groupBox4.Controls.Add(this.radioButton_AudioDirectSound);
            this.groupBox4.Controls.Add(this.radioButton_AudioOpenal);
            this.groupBox4.Controls.Add(this.radioButton_AudioBeep);
            this.groupBox4.Location = new System.Drawing.Point(12, 136);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(303, 118);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Audio Backend";
            // 
            // radioButton_AudioXna
            // 
            this.radioButton_AudioXna.AutoSize = true;
            this.radioButton_AudioXna.Enabled = false;
            this.radioButton_AudioXna.Location = new System.Drawing.Point(7, 89);
            this.radioButton_AudioXna.Name = "radioButton_AudioXna";
            this.radioButton_AudioXna.Size = new System.Drawing.Size(47, 17);
            this.radioButton_AudioXna.TabIndex = 3;
            this.radioButton_AudioXna.Text = "XNA";
            this.radioButton_AudioXna.UseVisualStyleBackColor = true;
            // 
            // radioButton_AudioDirectSound
            // 
            this.radioButton_AudioDirectSound.AutoSize = true;
            this.radioButton_AudioDirectSound.Enabled = false;
            this.radioButton_AudioDirectSound.Location = new System.Drawing.Point(7, 66);
            this.radioButton_AudioDirectSound.Name = "radioButton_AudioDirectSound";
            this.radioButton_AudioDirectSound.Size = new System.Drawing.Size(84, 17);
            this.radioButton_AudioDirectSound.TabIndex = 2;
            this.radioButton_AudioDirectSound.Text = "DirectSound";
            this.radioButton_AudioDirectSound.UseVisualStyleBackColor = true;
            // 
            // radioButton_AudioOpenal
            // 
            this.radioButton_AudioOpenal.AutoSize = true;
            this.radioButton_AudioOpenal.Enabled = false;
            this.radioButton_AudioOpenal.Location = new System.Drawing.Point(7, 43);
            this.radioButton_AudioOpenal.Name = "radioButton_AudioOpenal";
            this.radioButton_AudioOpenal.Size = new System.Drawing.Size(64, 17);
            this.radioButton_AudioOpenal.TabIndex = 1;
            this.radioButton_AudioOpenal.Text = "OpenAL";
            this.radioButton_AudioOpenal.UseVisualStyleBackColor = true;
            // 
            // radioButton_AudioBeep
            // 
            this.radioButton_AudioBeep.AutoSize = true;
            this.radioButton_AudioBeep.Checked = true;
            this.radioButton_AudioBeep.Location = new System.Drawing.Point(7, 20);
            this.radioButton_AudioBeep.Name = "radioButton_AudioBeep";
            this.radioButton_AudioBeep.Size = new System.Drawing.Size(83, 17);
            this.radioButton_AudioBeep.TabIndex = 0;
            this.radioButton_AudioBeep.TabStop = true;
            this.radioButton_AudioBeep.Text = "Kernel Beep";
            this.radioButton_AudioBeep.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButton_InputXna);
            this.groupBox5.Controls.Add(this.radioButton_InputDirectinput);
            this.groupBox5.Controls.Add(this.radioButton_InputSdl);
            this.groupBox5.Controls.Add(this.radioButton_InputWinForms);
            this.groupBox5.Location = new System.Drawing.Point(321, 136);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(140, 118);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Input Backend";
            // 
            // radioButton_InputXna
            // 
            this.radioButton_InputXna.AutoSize = true;
            this.radioButton_InputXna.Enabled = false;
            this.radioButton_InputXna.Location = new System.Drawing.Point(7, 89);
            this.radioButton_InputXna.Name = "radioButton_InputXna";
            this.radioButton_InputXna.Size = new System.Drawing.Size(47, 17);
            this.radioButton_InputXna.TabIndex = 3;
            this.radioButton_InputXna.Text = "XNA";
            this.radioButton_InputXna.UseVisualStyleBackColor = true;
            // 
            // radioButton_InputDirectinput
            // 
            this.radioButton_InputDirectinput.AutoSize = true;
            this.radioButton_InputDirectinput.Enabled = false;
            this.radioButton_InputDirectinput.Location = new System.Drawing.Point(7, 66);
            this.radioButton_InputDirectinput.Name = "radioButton_InputDirectinput";
            this.radioButton_InputDirectinput.Size = new System.Drawing.Size(77, 17);
            this.radioButton_InputDirectinput.TabIndex = 2;
            this.radioButton_InputDirectinput.Text = "DirectInput";
            this.radioButton_InputDirectinput.UseVisualStyleBackColor = true;
            // 
            // radioButton_InputSdl
            // 
            this.radioButton_InputSdl.AutoSize = true;
            this.radioButton_InputSdl.Enabled = false;
            this.radioButton_InputSdl.Location = new System.Drawing.Point(7, 43);
            this.radioButton_InputSdl.Name = "radioButton_InputSdl";
            this.radioButton_InputSdl.Size = new System.Drawing.Size(46, 17);
            this.radioButton_InputSdl.TabIndex = 1;
            this.radioButton_InputSdl.Text = "SDL";
            this.radioButton_InputSdl.UseVisualStyleBackColor = true;
            // 
            // radioButton_InputWinForms
            // 
            this.radioButton_InputWinForms.AutoSize = true;
            this.radioButton_InputWinForms.Checked = true;
            this.radioButton_InputWinForms.Location = new System.Drawing.Point(7, 20);
            this.radioButton_InputWinForms.Name = "radioButton_InputWinForms";
            this.radioButton_InputWinForms.Size = new System.Drawing.Size(100, 17);
            this.radioButton_InputWinForms.TabIndex = 0;
            this.radioButton_InputWinForms.TabStop = true;
            this.radioButton_InputWinForms.Text = "Windows.Forms";
            this.radioButton_InputWinForms.UseVisualStyleBackColor = true;
            // 
            // button_RunProgram
            // 
            this.button_RunProgram.Location = new System.Drawing.Point(358, 261);
            this.button_RunProgram.Name = "button_RunProgram";
            this.button_RunProgram.Size = new System.Drawing.Size(103, 23);
            this.button_RunProgram.TabIndex = 7;
            this.button_RunProgram.Text = "Start Program";
            this.button_RunProgram.UseVisualStyleBackColor = true;
            this.button_RunProgram.Click += new System.EventHandler(this.button_RunProgram_Click);
            // 
            // StartDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 295);
            this.Controls.Add(this.button_RunProgram);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StartDialog";
            this.Text = "Eimu 1.0 - Chip 8 Emulator (By Omegadox)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_FileBrowse;
        private System.Windows.Forms.TextBox textBox_RomPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton_CPUModeRecompiler;
        private System.Windows.Forms.RadioButton radioButton_CPUModeInterpreter;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton_GraphicsXna;
        private System.Windows.Forms.RadioButton radioButton_GraphicsDirect3d;
        private System.Windows.Forms.RadioButton radioButton_GraphicsOpengl;
        private System.Windows.Forms.RadioButton radioButton_GraphicsDrawing;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton_AudioXna;
        private System.Windows.Forms.RadioButton radioButton_AudioDirectSound;
        private System.Windows.Forms.RadioButton radioButton_AudioOpenal;
        private System.Windows.Forms.RadioButton radioButton_AudioBeep;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButton_InputXna;
        private System.Windows.Forms.RadioButton radioButton_InputDirectinput;
        private System.Windows.Forms.RadioButton radioButton_InputSdl;
        private System.Windows.Forms.RadioButton radioButton_InputWinForms;
        private System.Windows.Forms.Button button_RunProgram;
    }
}
﻿namespace Eimu
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox_SelectedInput = new System.Windows.Forms.ComboBox();
            this.button_RunProgram = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox_SelectedAudio = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.comboBox_SelectedGraphics = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_RomPath);
            this.groupBox1.Controls.Add(this.button_FileBrowse);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 64);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ROM File";
            // 
            // textBox_RomPath
            // 
            this.textBox_RomPath.Location = new System.Drawing.Point(7, 17);
            this.textBox_RomPath.Name = "textBox_RomPath";
            this.textBox_RomPath.ReadOnly = true;
            this.textBox_RomPath.Size = new System.Drawing.Size(289, 20);
            this.textBox_RomPath.TabIndex = 1;
            // 
            // button_FileBrowse
            // 
            this.button_FileBrowse.Location = new System.Drawing.Point(7, 38);
            this.button_FileBrowse.Name = "button_FileBrowse";
            this.button_FileBrowse.Size = new System.Drawing.Size(79, 23);
            this.button_FileBrowse.TabIndex = 0;
            this.button_FileBrowse.Text = "Browse...";
            this.button_FileBrowse.UseVisualStyleBackColor = true;
            this.button_FileBrowse.Click += new System.EventHandler(this.button_FileBrowse_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_CPUModeRecompiler);
            this.groupBox2.Controls.Add(this.radioButton_CPUModeInterpreter);
            this.groupBox2.Location = new System.Drawing.Point(321, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(136, 64);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.comboBox_SelectedInput);
            this.groupBox4.Location = new System.Drawing.Point(13, 83);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(444, 53);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input Backend";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(361, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 22);
            this.button2.TabIndex = 2;
            this.button2.Text = "Config...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox_SelectedInput
            // 
            this.comboBox_SelectedInput.FormattingEnabled = true;
            this.comboBox_SelectedInput.Location = new System.Drawing.Point(7, 19);
            this.comboBox_SelectedInput.Name = "comboBox_SelectedInput";
            this.comboBox_SelectedInput.Size = new System.Drawing.Size(265, 21);
            this.comboBox_SelectedInput.TabIndex = 0;
            // 
            // button_RunProgram
            // 
            this.button_RunProgram.Enabled = false;
            this.button_RunProgram.Location = new System.Drawing.Point(374, 260);
            this.button_RunProgram.Name = "button_RunProgram";
            this.button_RunProgram.Size = new System.Drawing.Size(83, 23);
            this.button_RunProgram.TabIndex = 7;
            this.button_RunProgram.Text = "Run Program";
            this.button_RunProgram.UseVisualStyleBackColor = true;
            this.button_RunProgram.Click += new System.EventHandler(this.button_RunProgram_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.comboBox_SelectedAudio);
            this.groupBox3.Location = new System.Drawing.Point(13, 142);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 53);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Audio Backend";
            // 
            // comboBox_SelectedAudio
            // 
            this.comboBox_SelectedAudio.FormattingEnabled = true;
            this.comboBox_SelectedAudio.Location = new System.Drawing.Point(7, 19);
            this.comboBox_SelectedAudio.Name = "comboBox_SelectedAudio";
            this.comboBox_SelectedAudio.Size = new System.Drawing.Size(265, 21);
            this.comboBox_SelectedAudio.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button5);
            this.groupBox5.Controls.Add(this.comboBox_SelectedGraphics);
            this.groupBox5.Controls.Add(this.button6);
            this.groupBox5.Location = new System.Drawing.Point(13, 201);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(444, 53);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Graphics Backend";
            // 
            // comboBox_SelectedGraphics
            // 
            this.comboBox_SelectedGraphics.FormattingEnabled = true;
            this.comboBox_SelectedGraphics.Location = new System.Drawing.Point(6, 19);
            this.comboBox_SelectedGraphics.Name = "comboBox_SelectedGraphics";
            this.comboBox_SelectedGraphics.Size = new System.Drawing.Size(266, 21);
            this.comboBox_SelectedGraphics.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(278, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(77, 22);
            this.button3.TabIndex = 3;
            this.button3.Text = "About";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(278, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 22);
            this.button1.TabIndex = 5;
            this.button1.Text = "About";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(361, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(77, 22);
            this.button4.TabIndex = 4;
            this.button4.Text = "Config...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(278, 19);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(77, 22);
            this.button5.TabIndex = 7;
            this.button5.Text = "About";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(361, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(77, 22);
            this.button6.TabIndex = 6;
            this.button6.Text = "Config...";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // StartDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 288);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_RunProgram);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StartDialog";
            this.Text = "Eimu 1.0 - Chip 8 Emulator (By Omegadox)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_FileBrowse;
        private System.Windows.Forms.TextBox textBox_RomPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton_CPUModeRecompiler;
        private System.Windows.Forms.RadioButton radioButton_CPUModeInterpreter;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_RunProgram;
        private System.Windows.Forms.ComboBox comboBox_SelectedInput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_SelectedAudio;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBox_SelectedGraphics;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}
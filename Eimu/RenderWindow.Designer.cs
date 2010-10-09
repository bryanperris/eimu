namespace Eimu
{
    partial class RenderWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.proccesorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPURegistersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPUStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.projectSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_RenderContext = new Eimu.RenderPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.BackgroundImage = global::Eimu.Properties.Resources.menubar;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(554, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // proccesorToolStripMenuItem
            // 
            this.proccesorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopToolStripMenuItem,
            this.pauseToolStripMenuItem});
            this.proccesorToolStripMenuItem.Name = "proccesorToolStripMenuItem";
            this.proccesorToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.proccesorToolStripMenuItem.Text = "Machine";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.audioConfigToolStripMenuItem,
            this.graphicsConfigToolStripMenuItem,
            this.inputConfigToolStripMenuItem,
            this.toolStripSeparator2,
            this.enableToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.viewToolStripMenuItem.Text = "Config";
            // 
            // audioConfigToolStripMenuItem
            // 
            this.audioConfigToolStripMenuItem.Name = "audioConfigToolStripMenuItem";
            this.audioConfigToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.audioConfigToolStripMenuItem.Text = "Audio Config";
            this.audioConfigToolStripMenuItem.Click += new System.EventHandler(this.audioConfigToolStripMenuItem_Click);
            // 
            // graphicsConfigToolStripMenuItem
            // 
            this.graphicsConfigToolStripMenuItem.Name = "graphicsConfigToolStripMenuItem";
            this.graphicsConfigToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.graphicsConfigToolStripMenuItem.Text = "Graphics Config";
            this.graphicsConfigToolStripMenuItem.Click += new System.EventHandler(this.graphicsConfigToolStripMenuItem_Click);
            // 
            // inputConfigToolStripMenuItem
            // 
            this.inputConfigToolStripMenuItem.Name = "inputConfigToolStripMenuItem";
            this.inputConfigToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.inputConfigToolStripMenuItem.Text = "Input Config";
            this.inputConfigToolStripMenuItem.Click += new System.EventHandler(this.inputConfigToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // enableToolStripMenuItem
            // 
            this.enableToolStripMenuItem.Name = "enableToolStripMenuItem";
            this.enableToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.enableToolStripMenuItem.Text = "Enable VSync";
            this.enableToolStripMenuItem.Click += new System.EventHandler(this.enableToolStripMenuItem_Click);
            // 
            // debuggingToolStripMenuItem
            // 
            this.debuggingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cPURegistersToolStripMenuItem,
            this.cPUStackToolStripMenuItem,
            this.memoryToolStripMenuItem,
            this.codeViewToolStripMenuItem});
            this.debuggingToolStripMenuItem.Name = "debuggingToolStripMenuItem";
            this.debuggingToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.debuggingToolStripMenuItem.Text = "Debugging";
            // 
            // cPURegistersToolStripMenuItem
            // 
            this.cPURegistersToolStripMenuItem.Name = "cPURegistersToolStripMenuItem";
            this.cPURegistersToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.cPURegistersToolStripMenuItem.Text = "CPU Registers";
            // 
            // cPUStackToolStripMenuItem
            // 
            this.cPUStackToolStripMenuItem.Name = "cPUStackToolStripMenuItem";
            this.cPUStackToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.cPUStackToolStripMenuItem.Text = "CPU Stack";
            // 
            // memoryToolStripMenuItem
            // 
            this.memoryToolStripMenuItem.Name = "memoryToolStripMenuItem";
            this.memoryToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.memoryToolStripMenuItem.Text = "Memory";
            // 
            // codeViewToolStripMenuItem
            // 
            this.codeViewToolStripMenuItem.Name = "codeViewToolStripMenuItem";
            this.codeViewToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.codeViewToolStripMenuItem.Text = "Code View";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1,
            this.projectSiteToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // projectSiteToolStripMenuItem
            // 
            this.projectSiteToolStripMenuItem.Name = "projectSiteToolStripMenuItem";
            this.projectSiteToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.projectSiteToolStripMenuItem.Text = "Project Site";
            this.projectSiteToolStripMenuItem.Click += new System.EventHandler(this.projectSiteToolStripMenuItem_Click);
            // 
            // panel_RenderContext
            // 
            this.panel_RenderContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_RenderContext.Location = new System.Drawing.Point(0, 24);
            this.panel_RenderContext.Name = "panel_RenderContext";
            this.panel_RenderContext.Size = new System.Drawing.Size(554, 400);
            this.panel_RenderContext.TabIndex = 1;
            // 
            // RenderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 424);
            this.Controls.Add(this.panel_RenderContext);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RenderWindow";
            this.Text = "RenderWindow";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RenderWindow_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem proccesorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphicsConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debuggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cPURegistersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cPUStackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem memoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem projectSiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeViewToolStripMenuItem;
        private RenderPanel panel_RenderContext;
    }
}
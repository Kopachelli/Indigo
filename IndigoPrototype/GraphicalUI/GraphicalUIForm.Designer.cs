﻿namespace GraphicalUI
{
    partial class GrapgicalUIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrapgicalUIForm));
            this.modelControlPanel = new System.Windows.Forms.Panel();
            this.modelPauseButton = new System.Windows.Forms.Button();
            this.modelStartButton = new System.Windows.Forms.Button();
            this.mapInfoPanel = new System.Windows.Forms.Panel();
            this.mapInfoPanelScrollBar = new System.Windows.Forms.VScrollBar();
            this.mapPanel = new GraphicalUI.myDoubleBufferedPanel();
            this.modelControlPanel.SuspendLayout();
            this.mapInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelControlPanel
            // 
            this.modelControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.modelControlPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.modelControlPanel.Controls.Add(this.modelPauseButton);
            this.modelControlPanel.Controls.Add(this.modelStartButton);
            this.modelControlPanel.Location = new System.Drawing.Point(13, 469);
            this.modelControlPanel.Name = "modelControlPanel";
            this.modelControlPanel.Size = new System.Drawing.Size(759, 81);
            this.modelControlPanel.TabIndex = 1;
            // 
            // modelPauseButton
            // 
            this.modelPauseButton.Enabled = false;
            this.modelPauseButton.Location = new System.Drawing.Point(85, 4);
            this.modelPauseButton.Name = "modelPauseButton";
            this.modelPauseButton.Size = new System.Drawing.Size(75, 23);
            this.modelPauseButton.TabIndex = 1;
            this.modelPauseButton.Text = "Pause";
            this.modelPauseButton.UseVisualStyleBackColor = true;
            this.modelPauseButton.Click += new System.EventHandler(this.modelPauseButton_Click);
            // 
            // modelStartButton
            // 
            this.modelStartButton.Location = new System.Drawing.Point(3, 3);
            this.modelStartButton.Name = "modelStartButton";
            this.modelStartButton.Size = new System.Drawing.Size(75, 23);
            this.modelStartButton.TabIndex = 0;
            this.modelStartButton.Text = "Start";
            this.modelStartButton.UseVisualStyleBackColor = true;
            this.modelStartButton.Click += new System.EventHandler(this.modelStartButton_Click);
            // 
            // mapInfoPanel
            // 
            this.mapInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapInfoPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mapInfoPanel.Controls.Add(this.mapInfoPanelScrollBar);
            this.mapInfoPanel.Location = new System.Drawing.Point(569, 13);
            this.mapInfoPanel.Name = "mapInfoPanel";
            this.mapInfoPanel.Size = new System.Drawing.Size(203, 450);
            this.mapInfoPanel.TabIndex = 2;
            // 
            // mapInfoPanelScrollBar
            // 
            this.mapInfoPanelScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mapInfoPanelScrollBar.Location = new System.Drawing.Point(186, 0);
            this.mapInfoPanelScrollBar.Name = "mapInfoPanelScrollBar";
            this.mapInfoPanelScrollBar.Size = new System.Drawing.Size(17, 450);
            this.mapInfoPanelScrollBar.TabIndex = 0;
            // 
            // mapPanel
            // 
            this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mapPanel.Location = new System.Drawing.Point(13, 13);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(550, 450);
            this.mapPanel.TabIndex = 0;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            this.mapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseDown);
            this.mapPanel.MouseEnter += new System.EventHandler(this.mapPanel_MouseEnter);
            this.mapPanel.MouseLeave += new System.EventHandler(this.mapPanel_MouseLeave);
            this.mapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseMove);
            this.mapPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseUp);
            // 
            // GrapgicalUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.mapInfoPanel);
            this.Controls.Add(this.modelControlPanel);
            this.Controls.Add(this.mapPanel);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GrapgicalUIForm";
            this.Text = "Indigo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GrapgicalUIForm_FormClosing);
            this.Load += new System.EventHandler(this.GrapgicalUIForm_Load);
            this.modelControlPanel.ResumeLayout(false);
            this.mapInfoPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private myDoubleBufferedPanel mapPanel;
        private System.Windows.Forms.Panel modelControlPanel;
        private System.Windows.Forms.Panel mapInfoPanel;
        private System.Windows.Forms.VScrollBar mapInfoPanelScrollBar;
        private System.Windows.Forms.Button modelPauseButton;
        private System.Windows.Forms.Button modelStartButton;
    }
}


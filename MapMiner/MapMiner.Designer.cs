namespace MapMiner
{
    partial class MapMiner
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.map = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.stateNameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.map)).BeginInit();
            this.SuspendLayout();
            // 
            // map
            // 
            this.map.Dock = System.Windows.Forms.DockStyle.Top;
            this.map.Location = new System.Drawing.Point(0, 0);
            this.map.Name = "map";
            this.map.Size = new System.Drawing.Size(1658, 568);
            this.map.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.map.TabIndex = 0;
            this.map.TabStop = false;
            this.map.Click += new System.EventHandler(this.map_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 580);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "State :";
            // 
            // stateNameLabel
            // 
            this.stateNameLabel.AutoSize = true;
            this.stateNameLabel.Location = new System.Drawing.Point(92, 580);
            this.stateNameLabel.Name = "stateNameLabel";
            this.stateNameLabel.Size = new System.Drawing.Size(70, 25);
            this.stateNameLabel.TabIndex = 2;
            this.stateNameLabel.Text = "label2";
            // 
            // MapMiner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1658, 892);
            this.Controls.Add(this.stateNameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.map);
            this.Name = "MapMiner";
            this.Text = "Map Miner 1.0";
            ((System.ComponentModel.ISupportInitialize)(this.map)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox map;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label stateNameLabel;
    }
}


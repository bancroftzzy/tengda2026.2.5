namespace ActiveControl.Forms
{
    partial class Form_Calculate
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
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.gbOptMethod = new System.Windows.Forms.GroupBox();
            this.rbtZeroDisp = new System.Windows.Forms.RadioButton();
            this.tbForces = new System.Windows.Forms.TextBox();
            this.rbtManual = new System.Windows.Forms.RadioButton();
            this.rbtPartitionPSO = new System.Windows.Forms.RadioButton();
            this.rbtGlobalPSO = new System.Windows.Forms.RadioButton();
            this.rbtDirect = new System.Windows.Forms.RadioButton();
            this.btnStartOnce = new System.Windows.Forms.Button();
            this.gbOptMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnContinue
            // 
            this.btnContinue.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnContinue.Location = new System.Drawing.Point(209, 309);
            this.btnContinue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(160, 40);
            this.btnContinue.TabIndex = 51;
            this.btnContinue.Text = "继续";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStop.Location = new System.Drawing.Point(382, 308);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(160, 40);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "终止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPause.Location = new System.Drawing.Point(209, 309);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(160, 40);
            this.btnPause.TabIndex = 52;
            this.btnPause.Text = "暂停";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStartAll
            // 
            this.btnStartAll.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartAll.Location = new System.Drawing.Point(209, 309);
            this.btnStartAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(160, 40);
            this.btnStartAll.TabIndex = 7;
            this.btnStartAll.Text = "全部计算";
            this.btnStartAll.UseVisualStyleBackColor = true;
            this.btnStartAll.Click += new System.EventHandler(this.btnStartAll_Click);
            // 
            // gbOptMethod
            // 
            this.gbOptMethod.Controls.Add(this.rbtZeroDisp);
            this.gbOptMethod.Controls.Add(this.tbForces);
            this.gbOptMethod.Controls.Add(this.rbtManual);
            this.gbOptMethod.Controls.Add(this.rbtPartitionPSO);
            this.gbOptMethod.Controls.Add(this.rbtGlobalPSO);
            this.gbOptMethod.Controls.Add(this.rbtDirect);
            this.gbOptMethod.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gbOptMethod.Location = new System.Drawing.Point(36, 33);
            this.gbOptMethod.Name = "gbOptMethod";
            this.gbOptMethod.Size = new System.Drawing.Size(506, 251);
            this.gbOptMethod.TabIndex = 55;
            this.gbOptMethod.TabStop = false;
            this.gbOptMethod.Text = "优化方法";
            // 
            // rbtZeroDisp
            // 
            this.rbtZeroDisp.AutoSize = true;
            this.rbtZeroDisp.Location = new System.Drawing.Point(59, 135);
            this.rbtZeroDisp.Name = "rbtZeroDisp";
            this.rbtZeroDisp.Size = new System.Drawing.Size(169, 27);
            this.rbtZeroDisp.TabIndex = 3;
            this.rbtZeroDisp.TabStop = true;
            this.rbtZeroDisp.Text = "动态零位移法";
            this.rbtZeroDisp.UseVisualStyleBackColor = true;
            // 
            // tbForces
            // 
            this.tbForces.Location = new System.Drawing.Point(85, 202);
            this.tbForces.Name = "tbForces";
            this.tbForces.Size = new System.Drawing.Size(230, 34);
            this.tbForces.TabIndex = 5;
            // 
            // rbtManual
            // 
            this.rbtManual.AutoSize = true;
            this.rbtManual.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtManual.Location = new System.Drawing.Point(59, 168);
            this.rbtManual.Name = "rbtManual";
            this.rbtManual.Size = new System.Drawing.Size(253, 30);
            this.rbtManual.TabIndex = 4;
            this.rbtManual.TabStop = true;
            this.rbtManual.Text = "手动赋值（kN/m）：";
            this.rbtManual.UseVisualStyleBackColor = true;
            // 
            // rbtPartitionPSO
            // 
            this.rbtPartitionPSO.AutoSize = true;
            this.rbtPartitionPSO.Location = new System.Drawing.Point(59, 102);
            this.rbtPartitionPSO.Name = "rbtPartitionPSO";
            this.rbtPartitionPSO.Size = new System.Drawing.Size(192, 27);
            this.rbtPartitionPSO.TabIndex = 2;
            this.rbtPartitionPSO.TabStop = true;
            this.rbtPartitionPSO.Text = "分区粒子群算法";
            this.rbtPartitionPSO.UseVisualStyleBackColor = true;
            // 
            // rbtGlobalPSO
            // 
            this.rbtGlobalPSO.AutoSize = true;
            this.rbtGlobalPSO.Location = new System.Drawing.Point(59, 69);
            this.rbtGlobalPSO.Name = "rbtGlobalPSO";
            this.rbtGlobalPSO.Size = new System.Drawing.Size(192, 27);
            this.rbtGlobalPSO.TabIndex = 1;
            this.rbtGlobalPSO.TabStop = true;
            this.rbtGlobalPSO.Text = "全局粒子群算法";
            this.rbtGlobalPSO.UseVisualStyleBackColor = true;
            // 
            // rbtDirect
            // 
            this.rbtDirect.AutoSize = true;
            this.rbtDirect.Location = new System.Drawing.Point(59, 36);
            this.rbtDirect.Name = "rbtDirect";
            this.rbtDirect.Size = new System.Drawing.Size(123, 27);
            this.rbtDirect.TabIndex = 0;
            this.rbtDirect.TabStop = true;
            this.rbtDirect.Text = "直接计算";
            this.rbtDirect.UseVisualStyleBackColor = true;
            // 
            // btnStartOnce
            // 
            this.btnStartOnce.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartOnce.Location = new System.Drawing.Point(36, 309);
            this.btnStartOnce.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStartOnce.Name = "btnStartOnce";
            this.btnStartOnce.Size = new System.Drawing.Size(160, 40);
            this.btnStartOnce.TabIndex = 6;
            this.btnStartOnce.Text = "逐工况计算";
            this.btnStartOnce.UseVisualStyleBackColor = true;
            this.btnStartOnce.Click += new System.EventHandler(this.btnStartOnce_Click);
            // 
            // Form_Calculate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(576, 378);
            this.Controls.Add(this.btnStartOnce);
            this.Controls.Add(this.gbOptMethod);
            this.Controls.Add(this.btnStartAll);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnContinue);
            this.Name = "Form_Calculate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "计算控制";
            this.gbOptMethod.ResumeLayout(false);
            this.gbOptMethod.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStartAll;
        private System.Windows.Forms.GroupBox gbOptMethod;
        private System.Windows.Forms.TextBox tbForces;
        private System.Windows.Forms.RadioButton rbtManual;
        private System.Windows.Forms.RadioButton rbtPartitionPSO;
        private System.Windows.Forms.RadioButton rbtGlobalPSO;
        private System.Windows.Forms.RadioButton rbtDirect;
        private System.Windows.Forms.Button btnStartOnce;
        private System.Windows.Forms.RadioButton rbtZeroDisp;
    }
}
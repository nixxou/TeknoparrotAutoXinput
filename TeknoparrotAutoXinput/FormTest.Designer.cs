namespace TeknoparrotAutoXinput
{
	partial class FormTest
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
			gameManagementUserControl1 = new GameManagementUserControl();
			SuspendLayout();
			// 
			// gameManagementUserControl1
			// 
			gameManagementUserControl1.Location = new Point(162, 161);
			gameManagementUserControl1.Name = "gameManagementUserControl1";
			gameManagementUserControl1.Size = new Size(780, 530);
			gameManagementUserControl1.TabIndex = 0;
			// 
			// FormTest
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1010, 753);
			Controls.Add(gameManagementUserControl1);
			Name = "FormTest";
			Text = "FormTest";
			Shown += FormTest_Shown;
			ResumeLayout(false);
		}

		#endregion

		private GameManagementUserControl gameManagementUserControl1;
	}
}
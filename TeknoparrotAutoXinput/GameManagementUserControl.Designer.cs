namespace TeknoparrotAutoXinput
{
	partial class GameManagementUserControl
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

		#region Code généré par le Concepteur de composants

		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			tableLayoutPanel1 = new TableLayoutPanel();
			tabGameControl = new TabControl();
			tabGameList = new TabPage();
			objectListViewGameList = new BrightIdeasSoftware.ObjectListView();
			clm_name = new BrightIdeasSoftware.OLVColumn();
			tabGameAdd = new TabPage();
			panel1 = new Panel();
			txt_scangame = new Krypton.Toolkit.KryptonTextBox();
			kryptonLabel7 = new Krypton.Toolkit.KryptonLabel();
			btn_select_scangame = new Krypton.Toolkit.KryptonButton();
			tableLayoutPanel1.SuspendLayout();
			tabGameControl.SuspendLayout();
			tabGameList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)objectListViewGameList).BeginInit();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(tabGameControl, 0, 1);
			tableLayoutPanel1.Controls.Add(panel1, 0, 0);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new Size(685, 414);
			tableLayoutPanel1.TabIndex = 0;
			tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
			// 
			// tabGameControl
			// 
			tabGameControl.Controls.Add(tabGameList);
			tabGameControl.Controls.Add(tabGameAdd);
			tabGameControl.Dock = DockStyle.Fill;
			tabGameControl.Location = new Point(3, 73);
			tabGameControl.Name = "tabGameControl";
			tabGameControl.SelectedIndex = 0;
			tabGameControl.Size = new Size(679, 338);
			tabGameControl.TabIndex = 0;
			// 
			// tabGameList
			// 
			tabGameList.Controls.Add(objectListViewGameList);
			tabGameList.Location = new Point(4, 24);
			tabGameList.Name = "tabGameList";
			tabGameList.Padding = new Padding(3);
			tabGameList.Size = new Size(671, 310);
			tabGameList.TabIndex = 0;
			tabGameList.Text = "Game List";
			tabGameList.UseVisualStyleBackColor = true;
			// 
			// objectListViewGameList
			// 
			objectListViewGameList.Columns.AddRange(new ColumnHeader[] { clm_name });
			objectListViewGameList.Dock = DockStyle.Fill;
			objectListViewGameList.FullRowSelect = true;
			objectListViewGameList.Location = new Point(3, 3);
			objectListViewGameList.Name = "objectListViewGameList";
			objectListViewGameList.ShowGroups = false;
			objectListViewGameList.Size = new Size(665, 304);
			objectListViewGameList.TabIndex = 0;
			objectListViewGameList.TileSize = new Size(200, 16);
			objectListViewGameList.View = View.Details;
			objectListViewGameList.FormatCell += objectListViewGameList_FormatCell;
			objectListViewGameList.FormatRow += objectListViewGameList_FormatRow;
			// 
			// clm_name
			// 
			clm_name.AspectName = "GameName";
			clm_name.FillsFreeSpace = true;
			// 
			// tabGameAdd
			// 
			tabGameAdd.Location = new Point(4, 24);
			tabGameAdd.Name = "tabGameAdd";
			tabGameAdd.Padding = new Padding(3);
			tabGameAdd.Size = new Size(671, 310);
			tabGameAdd.TabIndex = 1;
			tabGameAdd.Text = "Add Games";
			tabGameAdd.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			panel1.Controls.Add(txt_scangame);
			panel1.Controls.Add(kryptonLabel7);
			panel1.Controls.Add(btn_select_scangame);
			panel1.Location = new Point(3, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(675, 64);
			panel1.TabIndex = 1;
			// 
			// txt_scangame
			// 
			txt_scangame.Enabled = false;
			txt_scangame.Location = new Point(107, 3);
			txt_scangame.Name = "txt_scangame";
			txt_scangame.Size = new Size(444, 23);
			txt_scangame.TabIndex = 51;
			// 
			// kryptonLabel7
			// 
			kryptonLabel7.Location = new Point(4, 3);
			kryptonLabel7.Name = "kryptonLabel7";
			kryptonLabel7.Size = new Size(86, 20);
			kryptonLabel7.TabIndex = 49;
			kryptonLabel7.Values.Text = "Game Folder :";
			// 
			// btn_select_scangame
			// 
			btn_select_scangame.Location = new Point(557, 3);
			btn_select_scangame.Name = "btn_select_scangame";
			btn_select_scangame.Size = new Size(70, 23);
			btn_select_scangame.TabIndex = 50;
			btn_select_scangame.Values.Text = "...";
			// 
			// GameManagementUserControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(tableLayoutPanel1);
			Name = "GameManagementUserControl";
			Size = new Size(685, 414);
			tableLayoutPanel1.ResumeLayout(false);
			tabGameControl.ResumeLayout(false);
			tabGameList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)objectListViewGameList).EndInit();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel tableLayoutPanel1;
		private TabControl tabGameControl;
		private TabPage tabGameList;
		private TabPage tabGameAdd;
		private Panel panel1;
		private Krypton.Toolkit.KryptonTextBox txt_scangame;
		private Krypton.Toolkit.KryptonLabel kryptonLabel7;
		private Krypton.Toolkit.KryptonButton btn_select_scangame;
		private BrightIdeasSoftware.ObjectListView objectListViewGameList;
		private BrightIdeasSoftware.OLVColumn clm_name;
	}
}

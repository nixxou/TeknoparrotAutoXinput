using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeknoparrotAutoXinput
{
	public partial class Wizard : KryptonForm
	{
		Region tabOriginalRegion;

		public Wizard()
		{
			InitializeComponent();
		}

		private void Wizard_Load(object sender, EventArgs e)
		{
			tabOriginalRegion = tabControl1.Region;
			tabControl1.Region = new Region(tabControl1.DisplayRectangle);
		}

		private void btn_next_Click(object sender, EventArgs e)
		{
			bool canMoveNext = false;
			switch (tabControl1.SelectedIndex)
			{
				case 0:
					canMoveNext = verifDataTab1();
					break;
				case 1:
					canMoveNext = verifDataTab2();
					break;
				// Ajoutez des cases pour les autres onglets si nécessaire
				default:
					canMoveNext = true;
					break;
			}
			
			// Vérifier si l'index actuel est inférieur à l'index maximal des onglets
			if (canMoveNext && tabControl1.SelectedIndex < tabControl1.TabCount - 1)
			{
				// Passer à l'onglet suivant
				tabControl1.SelectedIndex++;
			}
		}

		private void btn_previous_Click(object sender, EventArgs e)
		{
			if (tabControl1.SelectedIndex > 0)
			{
				// Revenir à l'onglet précédent
				tabControl1.SelectedIndex--;
			}
		}

		private bool verifDataTab1()
		{
			// Ajoutez ici la logique de vérification pour le Tab 1
			// Retourne true si les données sont valides, sinon false
			return true; // Remplacez par la logique réelle
		}

		private bool verifDataTab2()
		{
			// Ajoutez ici la logique de vérification pour le Tab 2
			return true; // Remplacez par la logique réelle
		}

	}
}

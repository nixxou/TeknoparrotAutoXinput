namespace TeknoparrotAutoXinput
{
	// Classe pour rediriger la sortie standard vers la nouvelle console
	public class ConsoleTextWriter : System.IO.TextWriter
	{
		public override void Write(char value)
		{
			// Écrivez la sortie standard ici (par exemple, redirigez-la vers la fenêtre de la console)
			// Vous pouvez également utiliser Console.WriteLine() pour écrire une ligne
		}

		public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
	}
}
#region Usings
using Altium.Utils;
using System.IO;
#endregion

namespace Altium.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class FileGenerator
	{
		#region Class properties
		/// <summary>
		/// 
		/// </summary>
		public string FileName { get; }
		/// <summary>
		/// 
		/// </summary>
		public long Size { get; }
		#endregion

		#region Class initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFileName"></param>
		/// <param name="size"></param>
		public FileGenerator( string sFileName, long size ) 
		{
			FileName = sFileName;
			Size = size;
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// 
		/// </summary>
		public void Create() 
		{
			var progress = new ProgressBar( $"Generating [{FileName}]: " );

			using var fs = new FileStream( FileName, FileMode.Create, FileAccess.Write, FileShare.None );
			fs.SetLength( Size );

			using var writer = new StreamWriter( fs );

			var total = Create( writer, progress );

			progress.Complete( ( ( long ) total ).ToSizeSuffix() );
		}
		/// <summary>
		/// 
		/// </summary>
		private double Create( StreamWriter writer, ProgressBar progress ) 
		{
			double total = 0;

			while( total < Size )
			{
				var lr = $"{TestFactory.GetRandomUInt()}.{TestFactory.GetRandomSentence()}";

				writer.WriteLine( lr );

				total += ( lr.Length );

				progress.Report( ( total ) / Size );
			}

			return total;
		}
		#endregion
	}
}

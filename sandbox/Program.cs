#region Usings
using Altium.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort.Repl
{
	class Program
	{
		#region Class constants
		/// <summary>
		/// 
		/// </summary>
		private const string KEY_NAME = "-n";
		#endregion

		#region Class public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static async Task Main( string [] args )
		{
			try
			{
				var fileName = ExtractParameters( args );

				await new FileSorter( fileName ).Run();
			}
			catch( Exception e )
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Error.WriteLine( e.Message );
				Console.ResetColor();
			}
			
    }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static string ExtractParameters( string [] p )
		{
			if( p.Length < 2 )
				throw new Exception( "Incorrect number of arguments" );

			var args = p.ToList();

			var indexName = args.IndexOf( KEY_NAME );

			if( indexName == -1 || indexName >= args.Count - 1 )
				throw new Exception( $"Failed to find file name (ex. {KEY_NAME} MyFile.txt )" );

			var fileName = args [ indexName + 1 ];

			if( !File.Exists( fileName ) )
				throw new Exception( $"File [{fileName}] does not exists" );

			return fileName;
		}
		#endregion
	}
}

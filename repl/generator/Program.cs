#region Usings
using Altium.Utils;
using System;
using System.Linq;
#endregion

namespace Altium.Generator.Repl
{
	class Program
	{
		#region Class constants
		/// <summary>
		/// 
		/// </summary>
		private const string KEY_SIZE = "-s";
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
		static void Main( string [] args )
		{
			try
			{
				var ( file, size ) = ExtractParameters( args );

				new FileGenerator( file, size ).Create();
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
		public static (string file, long size)  ExtractParameters( string [] p ) 
		{
			if( p.Length < 4 )
				throw new Exception( "Incorrect number of arguments" );
		
			var args = p.ToList();

			var indexName = args.IndexOf( KEY_NAME );

			if( indexName == -1 || indexName >= args.Count - 1 )
				throw new Exception( $"Failed to find file name (ex. {KEY_NAME} MyFile.txt )" );

			var fileName = args [ indexName + 1 ];

			var indexSize = args.IndexOf( KEY_SIZE );

			if( indexSize == -1 || indexSize >= args.Count - 1 )
				throw new Exception( $"Failed to find file size (ex. {KEY_SIZE} 100kb )" );

			if( Math.Abs( indexSize - indexName ) <= 1 )
				throw new Exception( $"Incorrect arguments" );

			var size = args [ indexSize + 1 ];
			var fileSize = size.FromSizeSuffix();

			return (fileName, ( long )fileSize);
		}
		#endregion
	}
}

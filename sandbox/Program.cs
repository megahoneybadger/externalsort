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
		#region Class public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static async Task Main( string [] args )
		{
			//new FileGenerator( "200mb.txt", 200 * 1024 * 1024L ).Create();

			await new FileSorter( "1gb.txt" ).Run();
    }
  
    #endregion
  }
}

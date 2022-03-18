#region Usings
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// 
	/// </summary>
	public class FileSorter
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private readonly Splitter _splitter;
    /// <summary>
    /// 
    /// </summary>
    private readonly Sorter _sorter;
    /// <summary>
    /// 
    /// </summary>
    private readonly Merger _merger;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public string SourceFileName { get; init; }
		/// <summary>
		/// 
		/// </summary>
		public string TargetFileName { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public CancellationToken CancellationToken { get; init; } = CancellationToken.None;
    /// <summary>
    /// 
    /// </summary>
    public Options Options { get; init; }
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public FileSorter( string source, string target = null, Options options = null ) 
		{
			SourceFileName = source;
      TargetFileName = target;

      if( string.IsNullOrEmpty( TargetFileName ) ) 
      {
        TargetFileName = $"{Path.GetFileNameWithoutExtension( source )}[sorted]{Path.GetExtension( source )}";
      }
			
      Options = options ?? new();

      _splitter = new Splitter( this );
      _sorter = new Sorter( this );
      _merger = new Merger( this );
    }
		#endregion

		#region Class public methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
      var sw = Stopwatch.StartNew();

			var splitResult = await _splitter.Run();

			var sortedFiles = await _sorter.Run( splitResult );

			await _merger.Run( sortedFiles );

			var time = TimeSpan.FromMilliseconds( sw.ElapsedMilliseconds );
      sw.Stop();
      Console.WriteLine( $"Total: {time.TotalSeconds}s" );
    }
    #endregion
  }
}

#region Usings
using Altium.Utils;
using Sorting.SedgewickSort;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// Sorts slice of chunks.
	/// </summary>
	internal class Sorter : BaseComponent
	{
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    private int InputBufferSize => Options.Sort.InputBufferSize;
    /// <summary>
    /// 
    /// </summary>
    private int OutputBufferSize => Options.Sort.OutputBufferSize;
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public Sorter( FileSorter parent ) : base( parent )
		{
      
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unsortedFiles"></param>
    /// <returns></returns>
    public async Task<IList<string>> Run( SplitterResult split )
    {
      var unsortedFiles = split.Files;
      var sortedFiles = new ConcurrentBag<string>();

      _progress = new( "Sorting: " );

      await unsortedFiles.ParallelForEachAsync( async ( item ) =>
      {
        sortedFiles.Add( await CreateSortedFile( item, split.MaxChunkRows ) );

        _progress.Report( ( ( double )sortedFiles.Count ) / ( ( double )unsortedFiles.Count ) );
      });

      _progress.Complete( $"{sortedFiles.Count} file(s)" );

      return sortedFiles.ToList();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unsortedFile"></param>
    private async Task<string> CreateSortedFile( string unsortedFile, long maxRows ) 
    {
      var location = Options.FileLocation;

      var sortedFilename = unsortedFile.Replace(
          Options.UnsortedFileExtension, Options.SortedFileExtension );

      var unsortedFilePath = Path.Combine( location, unsortedFile );

      var sortedFilePath = Path.Combine( location, sortedFilename );

      await SortFile( File.OpenRead( unsortedFilePath ), File.OpenWrite( sortedFilePath ), maxRows );

      File.Delete( unsortedFilePath );

      return sortedFilePath;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unsortedFile"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private async Task SortFile( Stream unsortedFile, Stream target, long maxRows )
    {
      using var reader = new StreamReader( unsortedFile, bufferSize: InputBufferSize );
      
      var rows = new List<string>( ( int )maxRows );

      while( !reader.EndOfStream )
      {
        rows.Add( await reader.ReadLineAsync());
      }

      // TODO: try alternative string algorithm (e.x. burst sort).
      rows.Sort( StringComparer.Ordinal );

      await using var writer = new StreamWriter( target, bufferSize: OutputBufferSize );

			for( int i = 0; i < rows.Count; ++i )
			{
				await writer.WriteLineAsync( rows [ i ] );
			}
		}
    #endregion
  }
}

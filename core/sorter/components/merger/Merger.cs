#region Usings
using Altium.Utils;
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
	/// Merges all sorted chunks into a single files.
	/// </summary>
	internal class Merger : BaseComponent
	{
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    private int FilesPerRun => Options.Merge.FilesPerRun;
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public Merger( FileSorter parent ) : base( parent )
		{
      
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public string TargetFilePath => GetFullPath( _parent.TargetFileName );
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unsortedFiles"></param>
    /// <returns></returns>
    public async Task Run( IList<string> sortedFiles )
    {
      _progress = new( "Merging:" );
      var fileCount = sortedFiles.Count;

      var mergedFiles = new ConcurrentBag<string>();
      var partitionIndex = 0;

      while( sortedFiles.Count > 1 )
      {
        mergedFiles.Clear();

        var mergers = sortedFiles
          .Split( FilesPerRun )
          .Select( x => new PartitionMerger( ++partitionIndex, x ) );

        var finalRun = ( mergers.Count() == 1 );

        await mergers.ParallelForEachAsync( async x =>
          mergedFiles.Add( await x.Merge( finalRun ) ) );

        sortedFiles = mergedFiles.ToArray();

        _progress.Report( ( double )( fileCount - sortedFiles.Count ) / fileCount );
      }
   
      _progress.Complete( "Done" );

      File.Move( GetFullPath( sortedFiles.First() ), TargetFilePath, true );
    }
    #endregion

    #region Class 'Path' methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private string GetFullPath( string filename ) =>
      Path.Combine( Environment.CurrentDirectory, filename );
    #endregion
  }
}

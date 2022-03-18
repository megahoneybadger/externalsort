#region Usings
using Altium.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// 
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

        await mergers.ParallelForEachAsync( async x => mergedFiles.Add( await x.Merge() ) );

        sortedFiles = mergedFiles.ToArray();

        _progress.Report( ( double )( fileCount - sortedFiles.Count ) / fileCount );
      }
   
      _progress.Complete( "Done" );

      Recover( GetFullPath( sortedFiles.First() ) );
    }
    /// <summary>
    /// 
    /// </summary>
    public void Recover( string finalFile ) 
    {
      _progress = new( "Finalization:" );

      using var stream = File.OpenRead( finalFile );
      using var reader = new StreamReader( stream, bufferSize: 256 * 1024 * 1024 );

      
      using var output = File.Open( TargetFilePath, FileMode.Create );
      using var writer = new StreamWriter( output, bufferSize: 256 * 1024 * 1024 );

      var byteCount = 0;
      var sb = new StringBuilder( 100 );
      
      var block = ( int )stream.Length / 10;
      var step = 0;

      while( !reader.EndOfStream )
      {
        var s = reader.ReadLine();

        var tail = s [ ( s.Length - 10 ).. ];
        var head = s [ ..( s.Length - 10 ) ];
        var n = int.Parse( tail );

        sb.Append( n );
        sb.Append( '.' );
        sb.Append( head );

        var res = sb.ToString();
        sb.Length = 0;

        byteCount += s.Length + 1;
        writer.WriteLine( res );

        if( byteCount > block ) 
        {
          ++step;
          _progress.Report( step / 10 );
          byteCount = 0;
        }
      }

      reader.Close();
      writer.Close();

      File.Delete( finalFile );

      _progress.Complete( "Done" );
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

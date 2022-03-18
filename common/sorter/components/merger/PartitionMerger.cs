﻿#region Usings
using Altium.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// 
	/// </summary>
	internal class PartitionMerger 
	{
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    private readonly IList<string> _files;
    /// <summary>
    /// 
    /// </summary>
    private readonly int _index;
    /// <summary>
    /// 
    /// </summary>
    private readonly StreamWriter _writer;
    /// <summary>
    /// 
    /// </summary>
    private IList<ChunkReader> _readers;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public string TargetFileName => GetOutputFileName( _index );
    /// <summary>
    /// 
    /// </summary>
    public string TargetFilePath => GetFullPath( TargetFileName );
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public PartitionMerger( int index, IList<string> files ) 
		{
      _files = files;
      _index = index;

      _readers = new List<ChunkReader>( _files.Count );
      //_values = new LogRow [ _files.Count ];

      _writer = new StreamWriter( File.OpenWrite( TargetFilePath ), bufferSize: 64 * 1024 * 1024 );
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose() 
    {
      _writer?.Close();

			for( var i = 0; i < _files.Count; i++ )
			{
				// RENAME BEFORE DELETION SINCE DELETION OF LARGE FILES CAN TAKE SOME TIME
				// WE DONT WANT TO CLASH WHEN WRITING NEW FILES.
				var temporaryFilename = $"{_files [ i ]}.removal";
				File.Move( GetFullPath( _files [ i ] ), GetFullPath( temporaryFilename ) );
				File.Delete( GetFullPath( temporaryFilename ) );
			}
		}
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<string> Merge() 
    {
      OpenReaders();

      while( _readers.Count > 0 ) 
      {
        var index = GetCurrentReader();
        var reader = _readers [ index ];

        var valueToWrite = reader.Current.ToString();

        _writer.WriteLine( valueToWrite.AsMemory() );

        reader.Next();

        if( reader.EndOfStream )
        {
          _readers.RemoveAt( index );
          reader.Dispose();
        }
      }

      Dispose();

      return await Task.FromResult( TargetFilePath );
    }

		/// <summary>
		/// /
		/// </summary>
		/// <param name="sortedFiles"></param>
		/// <returns></returns>
		private void OpenReaders()
		{
			var count = _files.Count;

			for( var i = 0; i < count; i++ )
			{
				var sortedFilePath = GetFullPath( _files [ i ] );
        var reader = new ChunkReader( sortedFilePath );

        if( reader.Next() ) 
        {
          _readers.Add( reader );
          //_values [ i ] = reader.Current;
        }
			}
		}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetCurrentReader() 
    {
			string min = null;
			var index = -1;

			for( var i = 0; i < _readers.Count; i++ )
			{
				var reader = _readers [ i ];

				if( min == null || 0 > StringComparer.Ordinal.Compare( reader.Current, min ) )
				{
					min = reader.Current;
					index = i;
				}
			}

			return index;
		}
    #endregion

    #region Class 'Path' methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chunkCounter"></param>
    /// <returns></returns>
    private string GetOutputFileName( int chunkCounter ) =>
      $"{chunkCounter}.{Options.UnsortedFileExtension}.{Options.TempFileExtension}";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private string GetFullPath( string filename ) =>
      Path.Combine( Environment.CurrentDirectory, filename );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputFileName"></param>
    /// <returns></returns>
    private string GetFullPathWithoutTemp( string outputFileName ) =>
      GetFullPath( outputFileName.Replace( Options.TempFileExtension, string.Empty ) );
    #endregion
  }

  

}
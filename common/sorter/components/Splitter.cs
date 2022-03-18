#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// 
	/// </summary>
	internal class Splitter : BaseComponent
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private long _currentFileIndex = 0L;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    private string FileName => _parent.SourceFileName;
    /// <summary>
    /// 
    /// </summary>
    private int FileSize => _parent.Options.Split.FileSize;
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public Splitter( FileSorter parent ) : base( parent )
		{
      
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<SplitterResult> Run()
		{
      var files = new List<string>();
      var maxChunkRowCount = 0;
      double readBytes = 0;

      _progress = new( "Splitting: " );

			await using var stream = File.OpenRead( FileName );

			using var reader = new StreamReader( stream, bufferSize: 256 * 1024 * 1024 );

			while( !reader.EndOfStream )
			{
				var (buffer, rowCount, byteCount) = ReadChunk( reader );
        
        var unsortedFileName = await WriteChunk( buffer );

				maxChunkRowCount = Math.Max( maxChunkRowCount, rowCount );

				files.Add( unsortedFileName );

        readBytes += byteCount;
        _progress.Report( ( double )readBytes / stream.Length );
      }

			_progress.Complete( $"{files.Count} file(s)" );

			return new( files, maxChunkRowCount );
		}
    /// <summary>
    /// 
    /// </summary>
    private ( byte[] buffer, int rowCount, int byteCount )  ReadChunk( StreamReader reader )
    {
      var rowCount = 0;
      var byteCount = 0;

      var sb = new StringBuilder( FileSize );

      while( !reader.EndOfStream && byteCount < FileSize )
      {
        var s = reader.ReadLine();

        var index = s.IndexOf( '.' );

        if( index == -1 )
          continue;

        var number = s [ ..index ];
        var text = s [ ( index + 1 ).. ];

        var len = sb.Length;
        sb.Append( text );
        sb.Append( number.PadLeft( 10, '0' ) );
        sb.Append( '\n' );

        byteCount += sb.Length - len;
        ++rowCount;
      }

      var buffer = Encoding.ASCII.GetBytes( sb.ToString() );

      return ( buffer, rowCount, byteCount);
    }
    /// <summary>
    /// 
    /// </summary>
    private async Task<string> WriteChunk( byte [] buffer )
    {
			var fileName = $"{++_currentFileIndex}.{Options.UnsortedFileExtension}";

			await using var unsortedFile = File.Create(
				Path.Combine( _parent.Options.FileLocation, fileName ) );

			await unsortedFile.WriteAsync(
				buffer.AsMemory( 0, buffer.Length ), _parent.CancellationToken );

			return fileName;
		}
    #endregion
  }

  internal record SplitterResult( IReadOnlyCollection<string> Files, long MaxChunkRows );
}

#region Usings
using System.IO;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// Reads lines by line from a sorted chunk.
	/// </summary>
	internal class ChunkReader 
	{
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    private readonly string _path;
    /// <summary>
    /// 
    /// </summary>
    private readonly StreamReader _reader;
    /// <summary>
    /// 
    /// </summary>
    private string _current;
    /// <summary>
    /// 
    /// </summary>
    private bool _isEnd;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public string Current => _current;
    /// <summary>
    /// 
    /// </summary>
    public bool EndOfStream => _isEnd;
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public ChunkReader( string path ) 
		{
      _path = path;
      _reader = new StreamReader( path );
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose() => _reader?.Close();
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Next() 
    {
      if( EndOfStream )
        return false;

      if( _reader.Peek() < 0 )
      {
        _isEnd = true;
        return false;
      }

      var s = _reader.ReadLine();

      _current = s;

      return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
		public override string ToString()
		{
      if( _isEnd )
        return "(eos)";

      return ( null == _current ) ? string.Empty : _current.ToString();
		}
		#endregion
	}
}

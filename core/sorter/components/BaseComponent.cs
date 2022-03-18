#region Usings
using Altium.Utils;
using System.Threading;
#endregion

namespace Altium.Sort
{
	/// <summary>
	/// 
	/// </summary>
	internal abstract class BaseComponent
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected FileSorter _parent;
    /// <summary>
    /// 
    /// </summary>
    protected ProgressBar _progress;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    protected Options Options => _parent.Options;
    /// <summary>
    /// 
    /// </summary>
    public CancellationToken CancellationToken => _parent.CancellationToken;
    #endregion

    #region Class initialization
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="size"></param>
    public BaseComponent( FileSorter parent ) 
		{
      _parent = parent;
    }
    #endregion
  }
}

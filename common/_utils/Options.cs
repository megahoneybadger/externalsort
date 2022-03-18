#region Usings
using System;
using System.Collections.Generic;
#endregion

namespace Altium.Sort
{
  /// <summary>
  /// 
  /// </summary>
  public class Options
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const string UnsortedFileExtension = "unsorted";
    /// <summary>
    /// 
    /// </summary>
    public const string SortedFileExtension = "sorted";
    /// <summary>
    /// 
    /// </summary>
    public const string TempFileExtension = "tmp";
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public string FileLocation { get; init; } = Environment.CurrentDirectory;
    /// <summary>
    /// 
    /// </summary>
    public Splitting Split { get; init; } = new();
    /// <summary>
    /// 
    /// </summary>
    public Sorting Sort { get; init; } = new();
    /// <summary>
    /// 
    /// </summary>
    public Merging Merge { get; init; } = new();

    #endregion

    #region Class internal structs
    /// <summary>
    /// 
    /// </summary>
    public class Splitting
    {
      #region Class properties
      /// <summary>
      /// Size of unsorted file (chunk) (in bytes)
      /// </summary>
      public int FileSize { get; init; } = 128 * 1024 * 1024; //64 * 1024 * 1024;
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class Sorting
    {
      #region Class properties
      /// <summary>
      /// 
      /// </summary>
      public IComparer<string> Comparer { get; init; } = Comparer<string>.Default;
      /// <summary>
      /// /
      /// </summary>
      public int InputBufferSize { get; init; } = 65536;
      /// <summary>
      /// 
      /// </summary>
      public int OutputBufferSize { get; init; } = 65536;
      #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class Merging
    {
      #region Class properties
      /// <summary>
      /// How many files we will process per run
      /// </summary>
      public int FilesPerRun { get; init; } = 10;
      /// <summary>
      /// Buffer size (in bytes) for input StreamReaders
      /// </summary>
      public int InputBufferSize { get; init; } = 65536;
      /// <summary>
      /// Buffer size (in bytes) for output StreamWriter
      /// </summary>
      public int OutputBufferSize { get; init; } = 65536;
      /// <summary>
      /// 
      /// </summary>
      public IProgress<double> ProgressHandler { get; init; } = null!;
      #endregion
    }
    #endregion
  }
}
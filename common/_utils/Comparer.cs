#region Usings
using System;
using System.Collections.Generic;
#endregion

namespace Altium
{
  /// <summary>
  /// 
  /// </summary>
  public class RadixSorter 
  {
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arr"></param>
    public static void Sort( List<string> arr ) => Sort( arr, 0, arr.Count - 1, 0 );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="start"></param>
    /// <param name="last"></param>
    /// <param name="character_index"></param>
    private static void Sort( List<string> s, int start, int last, int character_index )
    {
      // base condition if no further index possible.
      if( start >= last )
        return;

      // first making a start pointer for dividing the
      // list from start to start_pointer.
      int start_pointer = start;

      // last_pointer and last are the boundaries for the
      // third list.
      int last_pointer = last;

      // taking the ascii value of the pivot at the index
      // given.
      int char_ascii_value_pivot
          = s [ start ] [ character_index ];

      int pointer = start + 1;

      // starting a pointer to scan the whole array to
      // sort.
      while( pointer <= last )
      {

        // ASCII value of char at the position of all
        // the strings to compare with that of the pivot
        // char.
        int char_ascii_value_element
            = s [ pointer ] [ character_index ];

        // if the element has char less than pivot than
        // swapping it with the top element and
        // incrementing the top boundary of the first
        // list.
        if( char_ascii_value_pivot
            > char_ascii_value_element )
        {
          Swap( s, start_pointer, pointer );
          start_pointer++;

          // incrementing the pointer to check for
          // next string.
          pointer++;
        }
        else

            // if found larger character than it is
            // replaced by the element at last_pointer
            // and lower boundary is raised by
            // decrementing it.
            if( char_ascii_value_pivot
                < char_ascii_value_element )
        {
          Swap( s, last_pointer, pointer );
          last_pointer--;
          pointer++;
        }

        // if character is the same as that of the pivot
        // then no need to swap and move the pointer on
        else
        {
          pointer++;
        }
      }

      // now performing same sort on the first list
      // bounded by start and start_pointer with same
      // character_index
      Sort( s, start, start_pointer - 1, character_index );

      // if we have more character left in the pivot
      // element than recall quicksort on the second list
      // bounded by  start_pointer and last_pointer and
      // next character_index.
      if( char_ascii_value_pivot >= 0 )

        Sort( s, start_pointer, last_pointer,
            character_index + 1 );

      // lastly the third list with boundaries as
      // last_pointer and last.
      Sort( s, last_pointer + 1, last, character_index );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private static void Swap( List<string> s, int x, int y )
    {
      string tmp = s [ x ];
      s [ x ] = s [ y ];
      s [ y ] = tmp;
    }
    #endregion
  }

  public class MSDSorter
  {
    private static int BITS_PER_BYTE = 8;
    private static int BITS_PER_INT = 32;   // each Java int is 32 bits 
    private static int R = 256;   // extended ASCII alphabet size
    private static int CUTOFF = 15;   // cutoff to insertion sort

    // do not instantiate
    private MSDSorter() { }

    /**
      * Rearranges the array of extended ASCII strings in ascending order.
      *
      * @param a the array to be sorted
      */
    public static void Sort( String [] a )
    {
      int n = a.Length;
      String [] aux = new String [ n ];
      sort( a, 0, n - 1, 0, aux );
    }

    // return dth character of s, -1 if d = length of string
    private static int charAt( String s, int d )
    {
      
      if( d == s.Length ) return -1;
      return s[ d ];
    }

    // sort from a[lo] to a[hi], starting at the dth character
    private static void sort( String [] a, int lo, int hi, int d, String [] aux )
    {

      // cutoff to insertion sort for small subarrays
      if( hi <= lo + CUTOFF )
      {
        insertion( a, lo, hi, d );
        return;
      }

      // compute frequency counts
      int [] count = new int [ R + 2 ];
      for( int i = lo; i <= hi; i++ )
      {
        int c = charAt( a [ i ], d );
        count [ c + 2 ]++;
      }

      // transform counts to indicies
      for( int r = 0; r < R + 1; r++ )
        count [ r + 1 ] += count [ r ];

      // distribute
      for( int i = lo; i <= hi; i++ )
      {
        int c = charAt( a [ i ], d );
        aux [ count [ c + 1 ]++ ] = a [ i ];
      }

      // copy back
      for( int i = lo; i <= hi; i++ )
        a [ i ] = aux [ i - lo ];


      // recursively sort for each character (excludes sentinel -1)
      for( int r = 0; r < R; r++ )
        sort( a, lo + count [ r ], lo + count [ r + 1 ] - 1, d + 1, aux );
    }


    // insertion sort a[lo..hi], starting at dth character
    private static void insertion( String [] a, int lo, int hi, int d )
    {
      for( int i = lo; i <= hi; i++ )
        for( int j = i; j > lo && less( a [ j ], a [ j - 1 ], d ); j-- )
          exch( a, j, j - 1 );
    }

    // exchange a[i] and a[j]
    private static void exch( String [] a, int i, int j )
    {
      String temp = a [ i ];
      a [ i ] = a [ j ];
      a [ j ] = temp;
    }

    // is v less than w, starting at character d
    private static bool less( String v, String w, int d )
    {
      // assert v.substring(0, d).equals(w.substring(0, d));
      for( int i = d; i < Math.Min( v.Length, w.Length ); i++ )
      {
        if( v[ i ] < w[ i ] ) return true;
        if( v[ i ] > w[ i ] ) return false;
      }
      return v.Length < w.Length;
    }


    /**
      * Rearranges the array of 32-bit integers in ascending order.
      * Currently assumes that the integers are nonnegative.
      *
      * @param a the array to be sorted
      */
    public static void Sort( int [] a )
    {
      int n = a.Length;
      int [] aux = new int [ n ];
      sort( a, 0, n - 1, 0, aux );
    }

    // MSD sort from a[lo] to a[hi], starting at the dth byte
    private static void sort( int [] a, int lo, int hi, int d, int [] aux )
    {

      // cutoff to insertion sort for small subarrays
      if( hi <= lo + CUTOFF )
      {
        insertion( a, lo, hi );
        return;
      }

      // compute frequency counts (need R = 256)
      int [] count = new int [ R + 1 ];
      int mask = R - 1;   // 0xFF;
      int shift = BITS_PER_INT - BITS_PER_BYTE * d - BITS_PER_BYTE;
      for( int i = lo; i <= hi; i++ )
      {
        int c = ( a [ i ] >> shift ) & mask;
        count [ c + 1 ]++;
      }

      // transform counts to indicies
      for( int r = 0; r < R; r++ )
        count [ r + 1 ] += count [ r ];

      // for most significant byte, 0x80-0xFF comes before 0x00-0x7F
      if( d == 0 )
      {
        int shift1 = count [ R ] - count [ R / 2 ];
        int shift2 = count [ R / 2 ];
        count [ R ] = shift1 + count [ 1 ];   // to simplify recursive calls later
        for( int r = 0; r < R / 2; r++ )
          count [ r ] += shift1;
        for( int r = R / 2; r < R; r++ )
          count [ r ] -= shift2;
      }

      // distribute
      for( int i = lo; i <= hi; i++ )
      {
        int c = ( a [ i ] >> shift ) & mask;
        aux [ count [ c ]++ ] = a [ i ];
      }

      // copy back
      for( int i = lo; i <= hi; i++ )
        a [ i ] = aux [ i - lo ];

      // no more bits
      if( d == 3 ) return;

      // special case for most significant byte
      if( d == 0 && count [ R / 2 ] > 0 )
        sort( a, lo, lo + count [ R / 2 ] - 1, d + 1, aux );

      // special case for other bytes
      if( d != 0 && count [ 0 ] > 0 )
        sort( a, lo, lo + count [ 0 ] - 1, d + 1, aux );

      // recursively sort for each character
      // (could skip r = R/2 for d = 0 and skip r = R for d > 0)
      for( int r = 0; r < R; r++ )
        if( count [ r + 1 ] > count [ r ] )
          sort( a, lo + count [ r ], lo + count [ r + 1 ] - 1, d + 1, aux );
    }

    // insertion sort a[lo..hi]
    private static void insertion( int [] a, int lo, int hi )
    {
      for( int i = lo; i <= hi; i++ )
        for( int j = i; j > lo && a [ j ] < a [ j - 1 ]; j-- )
          exch( a, j, j - 1 );
    }

    // exchange a[i] and a[j]
    private static void exch( int [] a, int i, int j )
    {
      int temp = a [ i ];
      a [ i ] = a [ j ];
      a [ j ] = temp;
    }


   
  }
}
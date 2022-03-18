#region Usings
using System;
using System.Collections.Generic;
#endregion

namespace Altium.Utils
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class PriorityQueue<K, T> where K : IComparable<K>
  {
    private int _size = 0;
    private readonly SortedDictionary<K, Queue<T>> _dict;

    public int Count => _size;
    public bool IsEmpty => ( Count == 0 );

    public PriorityQueue( bool desc = true )
    {
      var comp = desc ?
        Comparer<K>.Create( ( x, y ) => x.CompareTo( y ) ) :
        Comparer<K>.Create( ( x, y ) => y.CompareTo( x ) );

      _dict = new SortedDictionary<K, Queue<T>>( comp );
    }

    public PriorityQueue( IComparer<K> comp )
    {
      _dict = new SortedDictionary<K, Queue<T>>( comp );
    }

    public void Enqueue( K prio, T item = default )
    {
      if( null == item )
        return;

      _dict.TryGetValue( prio, out Queue<T> q );
      q ??= new Queue<T>();
      _dict [ prio ] = q;

      q.Enqueue( item );
      _size++;
    }

    public T Peek()
    {
      if( IsEmpty )
        return default;

      foreach( var q in _dict.Values )
      {
        if( q.Count > 0 )
          return q.Peek();
      }

      return default; // not supposed to reach here.
    }

    public T Dequeue()
    {
      if( IsEmpty )
        return default;

      foreach( var p in _dict )
      {
        var q = p.Value;

        // we use a sorted dictionary
        if( q.Count > 0 )
        {
          _size--;
          var res = q.Dequeue();

          if( q.Count == 0 )
          {
            _dict.Remove( p.Key );
          }

          return res;
        }
      }

      return default; // not supposed to reach here.
    }

    public (K key, T value) DequeueWithKey()
    {
      if( IsEmpty )
        return default;

      foreach( var p in _dict )
      {
        var q = p.Value;

        // we use a sorted dictionary
        if( q.Count > 0 )
        {
          _size--;
          var res = q.Dequeue();

          if( q.Count == 0 )
          {
            _dict.Remove( p.Key );
          }

          return (p.Key, res);
        }
      }

      return default; // not supposed to reach here.
    }

    public bool ContainsKey( K prio ) => _dict.ContainsKey( prio );


    public T Dequeue( K prio )
    {
      _dict.TryGetValue( prio, out Queue<T> q );

      if( null == q || 0 == q.Count )
        return default;

      _size--;
      return q.Dequeue();
    }


  }
}



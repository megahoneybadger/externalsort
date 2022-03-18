#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Diagnostics;
#endregion

namespace Altium.Utils
{
	/// <summary>
	/// 
	/// </summary>
	internal static class TestFactory
	{
		#region Class members
		/// <summary>
		/// 
		/// </summary>
		private static Random _random;
		/// <summary>
		/// 
		/// </summary>
		private static List<string> _suffixes;
		
		#endregion

		#region Class initialization
	/// <summary>
	/// 
	/// </summary>
	static TestFactory() 
		{
			_random = new();
			_suffixes = new ();

			var set = new HashSet<string>();
			var sw = Stopwatch.StartNew();

			while( _suffixes.Count < 1000 ) 
			{
				var next = string.Empty;

				do
				{
					next = GetRandomString( 3 );
				}
				while( set.Contains( next ) );

				set.Add( next );
				_suffixes.Add( next );
			}

			sw.Stop();
		}

		#endregion

		#region Class utility methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetRandomString( int length )
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string( Enumerable.Repeat( chars, length )
				.Select( s => s [ _random.Next( s.Length ) ] ).ToArray() );
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int GetRandomUShort( ushort max ) => _random.Next( 0, max );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
		public static uint GetRandomUInt( int max = Int32.MaxValue ) => ( uint )_random.Next( 0, max );
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static bool GetRandomBoolean() => _random.Next( 0, 10 ) > 5;
		
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomEnumValue<T>()
		{
			var v = Enum.GetValues( typeof( T ) );
			return ( T )v.GetValue( _random.Next( v.Length ) );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static object GetRandomEnumValue( Type t )
		{
			var v = Enum.GetValues( t );
			return v.GetValue( _random.Next( v.Length ) );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static string GetUid( int size = 10 )
		{
			char [] chars =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

			byte [] data = new byte [ size ];
			var result = new StringBuilder( size );

			using( var crypto = new RNGCryptoServiceProvider() )
			{
				crypto.GetBytes( data );
			}

			foreach( byte b in data )
			{
				result.Append( chars [ b % ( chars.Length ) ] );
			}

			return result.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string [] GetNouns()
		{
			return new string []
			{
				"haircut"
			, "truck"
			, "history"
			, "side"
			, "horse"
			, "trucks"
			, "cable"
			, "country"
			, "steam"
			, "nut"
			, "wave"
			, "verse"
			, "babies"
			, "plate"
			, "plastic"
			, "scarecrow"
			, "quicksand"
			, "rain"
			, "appliance"
			, "burst"
			, "thunder"
			, "grandfather"
			, "balance"
			, "key"
			, "match"
			, "adjustment"
			, "wire"
			, "skin"
			, "ring"
			, "cow"
			, "orange"
			, "tin"
			, "ice"
			, "grandmother"
			, "cup"
			, "duck"
			, "sticks"
			, "trousers"
			, "silk"
			, "furniture"
			, "cars"
			, "question"
			, "downtown"
			, "car"
			, "direction"
			, "time"
			, "boy"
			, "laugh"
			, "tub"
			, "addition"
			, "mist"
			, "daughter"
			, "slave"
			, "sack"
			, "chin"
			, "crook"
			, "box"
			, "spy"
			, "song"
			, "ship"
			, "vegetable"
			, "cemetery"
			, "oranges"
			, "hobbies"
			, "quiet"
			, "string"
			, "scene"
			, "cart"
			, "chickens"
			, "calendar"
			, "pets"
			, "smash"
			, "card"
			, "soup"
			, "tree"
			, "exchange"
			, "money"
			, "planes"
			, "fog"
			, "change"
			, "distribution"
			, "head"
			, "blade"
			, "bee"
			, "pie"
			, "blood"
			, "marble"
			, "houses"
			, "wing"
			, "crate"
			, "gate"
			, "day"
			, "twig"
			, "feeling"
			, "representative"
			, "zebra"
			, "ball"
			, "field"
			, "lock"
			, "frame"
			};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string [] GetLatinPhrases()
		{
			return new []
			{
				"Veni, vidi, vici",
				"Alea iacta est",
				"Carpe diem",
				"Cogito, ergo sum",
				"In vino verita",
				"Carthago delenda est",
				"Acta non verba",
				"Et tu Brute",
			};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetRandomSentence() => $"{GetRandomNoun()}-{GetRandomLatinPhrase()}";

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetRandomNoun( bool addPrefix = true )
		{
			var arr = GetNouns();

			var n = arr [ _random.Next( 0, arr.Length ) ];

			var s = _suffixes [ _random.Next( 0, _suffixes.Count ) ];

			return addPrefix ? $"{n}-{s}" : n;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetRandomLatinPhrase()
		{
			var arr = GetLatinPhrases();

			var n = arr [ _random.Next( 0, arr.Length ) ];

			return n;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> SelectRandomObjects<T>( IList<T> list, int count = 0 )
		{
			count = ( 0 == count ) ? GetRandomUShort( 5 ) : count;

			if( count > list.Count )
				return list.ToList();

			var res = new List<T>();

			while( count > 0 )
			{
				while( true )
				{
					int index = GetRandomUShort( ( ushort )list.Count() );
					var cand = list [ index ];

					if( !res.Contains( cand ) )
					{
						res.Add( cand );
						break;
					}
				}

				--count;

				if( res.Count() == list.Count() )
					break;
			}

			return res;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T SelectRandomObject<T>( IList<T> list )
		{
			return SelectRandomObjects<T>( list, 1 ).FirstOrDefault();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T SelectRandomObject<T>( IList<T> list, Func<T, bool> pred )
			where T : class
		{
			var count = 0;

			while( count < list.Count )
			{
				var obj = SelectRandomObject( list );

				if( pred.Invoke( obj ) )
					return obj;

				++count;
			}

			return null;
		}
		
		
		
		#endregion
	}
}

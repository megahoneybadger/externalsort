#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Altium.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public static class Extensions
	{
		#region Class members
		/// <summary>
		/// 
		/// </summary>
		private static readonly string [] SizeSuffixes =
			{ "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		#endregion

		#region Class 'Collection' methods
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		public static List<List<T>> Split<T>( this IList<T> source, int chunkSize )
		{
			return source
					.Select( ( x, i ) => new { Index = i, Value = x } )
					.GroupBy( x => x.Index / chunkSize )
					.Select( x => x.Select( v => v.Value ).ToList() )
					.ToList();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="asyncAction"></param>
		/// <param name="maxDegreeOfParallelism"></param>
		/// <returns></returns>
		public static async Task ParallelForEachAsync<T>( this IEnumerable<T> source,
			Func<T, Task> asyncAction, int maxDegreeOfParallelism = -1 )
		{
			if( maxDegreeOfParallelism == -1 )
			{
				maxDegreeOfParallelism = Environment.ProcessorCount;
			}

			var throttler = new SemaphoreSlim( initialCount: maxDegreeOfParallelism );

			var tasks = source.Select( async item =>
			{
				await throttler.WaitAsync();

				try
				{
					await asyncAction( item ).ConfigureAwait( false );
				}
				finally
				{
					throttler.Release();
				}
			} );

			await Task.WhenAll( tasks );
		}
		#endregion

		#region Class 'Size Suffix' methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="decimalPlaces"></param>
		/// <returns></returns>
		public static string ToSizeSuffix( this long value, int decimalPlaces = 1 )
		{
			if( decimalPlaces < 0 ) { throw new ArgumentOutOfRangeException( "decimalPlaces" ); }
			if( value < 0 ) { return "-" + ToSizeSuffix( -value, decimalPlaces ); }
			if( value == 0 ) { return string.Format( "{0:n" + decimalPlaces + "} bytes", 0 ); }

			// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
			int mag = ( int )Math.Log( value, 1024 );

			// 1L << (mag * 10) == 2 ^ (10 * mag) 
			// [i.e. the number of bytes in the unit corresponding to mag]
			decimal adjustedSize = ( decimal )value / ( 1L << ( mag * 10 ) );

			// make adjustment when the value is large enough that
			// it would round up to 1000 or more
			if( Math.Round( adjustedSize, decimalPlaces ) >= 1000 )
			{
				mag += 1;
				adjustedSize /= 1024;
			}

			return string.Format( "{0:n" + decimalPlaces + "} {1}",
					adjustedSize,
					SizeSuffixes [ mag ] );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="decimalPlaces"></param>
		/// <returns></returns>
		public static double FromSizeSuffix( this string value, int kb_value = 1024 )
		{
			// Remove leading and trailing spaces.
			value = value.Trim();

			try
			{
				// Find the last non-alphabetic character.
				int ext_start = 0;
				for( int i = value.Length - 1; i >= 0; i-- )
				{
					// Stop if we find something other than a letter.
					if( !char.IsLetter( value, i ) )
					{
						ext_start = i + 1;
						break;
					}
				}

				// Get the numeric part.
				double number = double.Parse( value.Substring( 0, ext_start ) );

				// Get the extension.
				string suffix;
				if( ext_start < value.Length )
				{
					suffix = value.Substring( ext_start ).Trim().ToUpper();
					if( suffix == "BYTES" ) suffix = "bytes";
				}
				else
				{
					suffix = "bytes";
				}

				// Find the extension in the list.
				int suffix_index = -1;
				for( int i = 0; i < SizeSuffixes.Length; i++ )
				{
					if( SizeSuffixes [ i ] == suffix )
					{
						suffix_index = i;
						break;
					}
				}
				if( suffix_index < 0 )
					throw new FormatException( "Unknown size extension " + suffix + "." );

				// Return the result.
				return Math.Round( number * Math.Pow( kb_value, suffix_index ) );
			}
			catch( Exception ex )
			{
				throw new FormatException( "Invalid size format", ex );
			}
		}
		#endregion
	}
}

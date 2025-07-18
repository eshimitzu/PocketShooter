﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetUsageDetectorNamespace.Extras
{
	public class EmptyEnumerator<T> : IEnumerable<T>, IEnumerator<T>
	{
		public T Current { get { return default( T ); } }
		object IEnumerator.Current { get { return Current; } }

		public void Dispose() { }
		public void Reset() { }

		public bool MoveNext()
		{
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}

	public class ObjectToSearchEnumerator : IEnumerable<Object>, IEnumerator<Object>
	{
		public Object Current
		{
			get
			{
				if( subAssetIndex < 0 )
					return source[index].obj;

				return source[index].subAssets[subAssetIndex].subAsset;
			}
		}

		object IEnumerator.Current { get { return Current; } }

		private List<ObjectToSearch> source;
		private int index;
		private int subAssetIndex;

		public ObjectToSearchEnumerator( List<ObjectToSearch> source )
		{
			this.source = source;
			Reset();
		}

		public void Dispose()
		{
			source = null;
		}

		public bool MoveNext()
		{
			if( subAssetIndex < -1 )
			{
				subAssetIndex = -1;

				if( ++index >= source.Count )
					return false;

				// Skip folder assets in the enumeration, AssetUsageDetector expands encountered folders automatically
				// and we don't want that to happen as source[index].subAssets already contains the folder's contents
				if( !source[index].obj.IsFolder() )
					return true;
			}

			List<ObjectToSearch.SubAsset> subAssets = source[index].subAssets;
			if( subAssets != null )
			{
				while( ++subAssetIndex < subAssets.Count && !subAssets[subAssetIndex].shouldSearch )
					continue;

				if( subAssetIndex < subAssets.Count )
					return true;
			}

			subAssetIndex = -2;
			return MoveNext();
		}

		public void Reset()
		{
			index = -1;
			subAssetIndex = -2;
		}

		public IEnumerator<Object> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}
}
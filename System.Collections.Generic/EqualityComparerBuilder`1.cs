namespace System.Collections.Generic
{
	public static partial class EqualityComparerBuilder<T>
	{
		private class ComparerIntl : IEqualityComparer<T>
		{
			private readonly Func<T, int> _getHashCodeFunc;
			private readonly Func<T, T, bool> _equalsFunc;

			public ComparerIntl(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
			{
				_getHashCodeFunc = getHashCodeFunc;
				_equalsFunc = equalsFunc;
			}

			public bool Equals(T x, T y)
			{
				return _equalsFunc(x, y);
			}

			public int GetHashCode(T obj)
			{
				return _getHashCodeFunc(obj);
			}
		}

		public static IEqualityComparer<T> Create(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
		{
			if (getHashCodeFunc == null)
				throw new ArgumentNullException("getHashCodeFunc");
			if (equalsFunc == null)
				throw new ArgumentNullException("equalsFunc");

			return new ComparerIntl(getHashCodeFunc, equalsFunc);
		}
	}
}

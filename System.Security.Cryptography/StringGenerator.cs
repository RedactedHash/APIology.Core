namespace System.Security.Cryptography
{
	using System.Text;

	public static class StringGenerator
	{
		public static string Create(int length = 16)
		{
			if (length < 1)
				throw new ArgumentOutOfRangeException(nameof(length), "Value must be positive and greater than zero.");
			
            using (var rng = RandomNumberGenerator.Create())
			{
				var b = new byte[1];
				var s = new byte[length];

				for (var i = 0; i < length; i++)
				{
					rng.GetBytes(b);
					if (b[0] >= 248) {
						// prevent bias in character selection.
						// (by ensuring the value is within an even multiple
						// of the number of allowed characters (62)
						// otherwise the RNG will bias toward low-range characters)
						i--;
						continue;
					}

					var c = b[0] % 62;

					if (c < 10) {
						s[i] = (byte)(c + 0x30); // addend for UTF8 0-10 character range.
						continue;
					}

					if (c < 36) {
						s[i] = (byte)(c + 0x37); // addend for UTF8 A-Z character range.
						continue;
					}

					s[i] = (byte)(c + 0x3D); // addend for the UTF8 a-z character range.
				}

				return Encoding.UTF8.GetString(s);
			}
		}

		public static string Create(int length, char[] charset)
		{
			if (length < 1)
				throw new ArgumentOutOfRangeException(nameof(length), "Value must be positive and greater than zero.");

			if (string.IsNullOrEmpty(new string(charset)))
				throw new ArgumentOutOfRangeException(nameof(length), "character set cannot be empty or null.");

			using (var rng = RandomNumberGenerator.Create())
			{
				var limit = charset.Length * (255 / charset.Length);

				var b = new byte[1];
				var s = new char[length];

				for (var i = 0; i < length; i++)
				{
					rng.GetBytes(b);
					if (b[0] >= limit)
					{
						i--;
						continue;
					}

					s[i] = charset[b[0] % charset.Length];
				}

				return new string(s);
			}
		}
	}
}

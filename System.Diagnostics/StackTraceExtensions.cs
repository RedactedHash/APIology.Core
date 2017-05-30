#if !NETSTANDARD13
namespace System.Diagnostics
{
	using Linq;
	using Reflection;
	using Runtime.CompilerServices;
	using Runtime.InteropServices;

	public static class StackTraceExtensions
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetCaller(this StackTrace trace)
		{
			var originFrame = trace.GetFrames()?
				.Reverse()
				.Where(frame => ApplicationGuid.Value == GetGuid(frame.GetMethod().DeclaringType?.GetTypeInfo().Assembly))
				.LastOrDefault();

			if (originFrame == null)
				return "UnknownCaller";

			var parentMethod = originFrame.GetMethod();
			var declaringType = parentMethod?.DeclaringType;
			var parentClass = declaringType?.FullName
				.Replace($@"{declaringType.GetTypeInfo().Assembly.FullName.Split(',')[0]}.", "");
			var methodName = $@"{parentClass}.{parentMethod?.Name}";

			return methodName != "."
				? methodName
				: null;
		}

		private static Lazy<Guid> ApplicationGuid => new Lazy<Guid>(() => GetGuid(Assembly.GetEntryAssembly()));

		private static Guid GetGuid(Assembly assembly)
		{
			var attribute = assembly?.GetCustomAttributes<GuidAttribute>().FirstOrDefault();

			return !ReferenceEquals(attribute, null)
				? Guid.Parse(attribute.Value)
				: Guid.Empty;
		}
	}
}
#endif
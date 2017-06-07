using System.Collections.Generic;
using System.Linq;
using OmniTag.Models.Base;

namespace OmniTag.Models.Utility
{
	public static class ModelListExtensions
	{
		/// <summary>
		/// Checks whether one collection of IModelEntities contains the same models as another.
		/// </summary>
		public static bool IsEqualTo<T>(this IEnumerable<T> src, IEnumerable<T> other) where T : IModelEntity
		{
			if (src.Count() != other.Count())
				return false;

			Dictionary<int, int> dict = new Dictionary<int, int>();

			foreach (var model in src)
			{
				if (!dict.ContainsKey(model.Id))
					dict.Add(model.Id, 1);
				else
					dict[model.Id]++;
			}

			foreach (var model in other)
			{
				if (!dict.ContainsKey(model.Id))
					return false;
				
				dict[model.Id]--;
			}

			return dict.All(kvp => kvp.Value == 0);
		}
	}
}

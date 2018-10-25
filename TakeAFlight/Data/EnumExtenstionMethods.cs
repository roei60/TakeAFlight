using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Data
{
	public static class EnumExtenstionMethoods
	{
		/// <summary>
		/// this function receive enum and return list items for display on html select
		/// </summary>
		/// <param name="enumObj"></param>
		/// <returns></returns>


		public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
			where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			var values = from TEnum e in Enum.GetValues(typeof(TEnum))
						 select new { Id = e, Name = e.ToString() };
			return new SelectList(values, "Id", "Name", enumObj);
		}

	}
}

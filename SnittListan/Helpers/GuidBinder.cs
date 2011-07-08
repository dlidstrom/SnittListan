﻿using System;
using System.Web.Mvc;

namespace SnittListan.Helpers
{
	public class GuidBinder : DefaultModelBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			Guid guid;
			if (value != null && Guid.TryParse(value.AttemptedValue, out guid))
				return guid;

			return Guid.Empty;
		}
	}
}
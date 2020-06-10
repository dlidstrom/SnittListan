﻿namespace Snittlistan.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;

    public class GuidBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null && Guid.TryParse(value.AttemptedValue, out Guid guid))
                return guid;

            return Guid.Empty;
        }
    }
}
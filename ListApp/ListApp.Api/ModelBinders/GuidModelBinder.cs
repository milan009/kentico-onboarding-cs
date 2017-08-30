using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace ListApp.Api.ModelBinders
{
    public class GuidModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string key = value.RawValue.ToString();

            if (!Guid.TryParse(key, out Guid guid))
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, "Cannot convert given ID to a GUID! Invalid format!");
                return false;
            }

            bindingContext.Model = guid;
            return true;
        }
    }
}
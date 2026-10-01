using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return dictionary.SelectMany(m => m.Value?.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
        }
        public static string GetErrorMessageString(this ModelStateDictionary dictionary)
        {
            var errors = dictionary.SelectMany(m => m.Value?.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            var message = string.Join("<br/>", errors);
            return message;
        }
    }
}

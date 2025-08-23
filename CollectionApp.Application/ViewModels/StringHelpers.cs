using System;
using System.Collections.Generic;

namespace CollectionApp.Application.ViewModels
{
    public static class StringHelpers
    {
        public static string[] JoinNonEmpty(params string?[] values)
        {
            if (values is null || values.Length == 0) return Array.Empty<string>();
            var list = new List<string>(values.Length);
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    list.Add(v!.Trim());
                }
            }
            return list.ToArray();
        }
    }
}


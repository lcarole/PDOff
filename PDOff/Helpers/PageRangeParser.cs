using System;
using System.Collections.Generic;
using System.Linq;

namespace PDOff.Helpers;

public static class PageRangeParser
{
    public static List<int> Parse(string range, int? totalPages = null)
    {
        var pages = new HashSet<int>();

        foreach (var part in range.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.Contains('-'))
            {
                var bounds = part.Split('-', 2);
                if (int.TryParse(bounds[0].Trim(), out int start) && int.TryParse(bounds[1].Trim(), out int end))
                {
                    if (start > end) (start, end) = (end, start);
                    start = Math.Max(1, start);
                    if (totalPages.HasValue) end = Math.Min(totalPages.Value, end);
                    for (int i = start; i <= end; i++)
                        pages.Add(i);
                }
            }
            else if (int.TryParse(part, out int page) && page >= 1)
            {
                if (!totalPages.HasValue || page <= totalPages.Value)
                    pages.Add(page);
            }
        }

        return pages.OrderBy(p => p).ToList();
    }
}

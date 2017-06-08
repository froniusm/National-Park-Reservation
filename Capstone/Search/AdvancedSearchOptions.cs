using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.Search
{
    public class AdvancedSearchOptions
    {
        public int MaxOccupancy { get; set; }
        public bool NeedsAccessibility { get; set; }
        public bool NeedsUtilityHookup { get; set; }
        public int RequiredRVLength { get; set; }
    }
}

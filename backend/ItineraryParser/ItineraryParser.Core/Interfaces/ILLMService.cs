using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItineraryParser.Core.Interfaces;

public interface ILLMService
{
    Task<string> ExtractAsync(string prompt);
}


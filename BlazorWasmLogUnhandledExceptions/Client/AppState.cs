using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWasmLogUnhandledExceptions.Client
{
    public static class AppState
    {
        public static string User { get; set; } = "John Doe";
        public static string UserCompany { get; set; } = "ACME International";
    }
}

// See https://aka.ms/new-console-template for more information
using System.Globalization;

Console.WriteLine("Hello, World!");



Console.WriteLine($"CurrentCulture: {CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"Invariant? {AppContext.GetData("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT")}");
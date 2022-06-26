using System.Reflection;
using Spectre.Console;

namespace App.Helpers;

public interface IRandomHelper
{
    Spinner RandomSpinner();
}

public class RandomHelper : IRandomHelper
{
    public Spinner RandomSpinner()
    {
        var values = typeof(Spinner.Known)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.PropertyType == typeof(Spinner))
            .Select(x => (Spinner)x.GetValue(null))
            .ToArray();

        var index = Random.Shared.Next(values.Length);
        var value = (Spinner)values.GetValue(index);
        return value;
    }
}
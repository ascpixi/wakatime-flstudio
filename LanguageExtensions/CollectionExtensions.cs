using System.Collections.ObjectModel;

namespace Ascpixi.Wakatime.FLStudio.LanguageExtensions;

public static class CollectionExtensions
{
    public static void Add<T>(this Collection<T> self, T value, bool condition)
    {
        if (condition) {
            self.Add(value);
        }
    }

    public static void Add<T>(this Collection<T> self, ReadOnlySpan<T> values, bool condition)
    {
        if (!condition)
            return;
        
        foreach (var value in values) {
            self.Add(value);
        }
    }
}
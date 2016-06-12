using SharpDocs.Documentation;

namespace SharpDocs.Formatters
{
    public interface IFormatter
    {
        string Extension { get; }

        string Render(Assembly tree);
    }
}
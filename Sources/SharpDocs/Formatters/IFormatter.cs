namespace SharpDocs.Formatters
{
    using Documentation;

    /// <summary>
    /// Generates a document from an assembly tree.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Extension of files of content.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Renders the final document content from an assembly tree.
        /// </summary>
        /// <param name="tree">The assembly tree.</param>
        /// <returns>The content of the document.</returns>
        string Render(Assembly tree);
    }
}
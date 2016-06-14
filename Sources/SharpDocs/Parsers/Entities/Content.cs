namespace SharpDocs.Parsers.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// A documentation formatted text content.
    /// </summary>
    public class Content : List<Content.IContentNode>
    {
        /// <summary>
        /// A content formatted text node.
        /// </summary>
        public interface IContentNode
        {

        }

        /// <summary>
        /// A text node.
        /// </summary>
        public class Text : IContentNode
        {
            /// <summary>
            /// The text content.
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// A new paragraph containing children nodes (mapped from "<para>").
        /// </summary>
        public class Paragraph :  IContentNode
        {
            /// <summary>
            /// All the children nodes.
            /// </summary>
            public Content Content { get; set; }
        }

        /// <summary>
        /// An inline code sample (mapped from "<c>").
        /// </summary>
        public class InlineCode : IContentNode
        {
            /// <summary>
            /// The code content.
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// A code block (mapped from "<code>").
        /// </summary>
        public class Code : IContentNode // <code>
        {
            public string Value { get; set; }
        }

        /// <summary>
        /// A reference to another type (mapped from "<see>").
        /// </summary>
        public class See : IContentNode 
        {
            public string Reference { get; set; }
        }

        /// <summary>
        /// A reference to a parameter (mapped from "<paramref>").
        /// </summary>
        public class ParameterReference : IContentNode
        {
            public string Value { get; set; }
        }

        /// <summary>
        /// A reference to a generic type parameter (mapped from "<typeparamref>").
        /// </summary>
        public class TypeParameterReference : IContentNode
        {
            public string Value { get; set; }
        }

        /// <summary>
        /// A list node (mapped from "<list>").
        /// </summary>
        public class List : IContentNode
        {
            /// <summary>
            /// The header from the list (mapped from "<listheader>").
            /// </summary>
            public Item Header { get; set; }

            public string Type { get; set; }
            
            public IEnumerable<Item> Items { get; set; }

            /// <summary>
            /// A list item node (mapped from "<item>").
            /// </summary>
            public class Item
            {
                public Content Value { get; set; }
            }
        }

    }
}

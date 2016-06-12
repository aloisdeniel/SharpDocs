namespace SharpDocs.Parsers.Entities
{
    using System.Collections.Generic;

    public class Content : List<Content.IContentNode>
    {
        public interface IContentNode
        {

        }

        public class Text : IContentNode
        {
            public string Value { get; set; }
        }

        public class Paragraph :  IContentNode
        {
            public Content Content { get; set; }
        }

        public class InlineCode : IContentNode // <c>
        {
            public string Value { get; set; }
        }
        
        public class Code : IContentNode // <code>
        {
            public string Value { get; set; }
        }

        public class See : IContentNode // <see cref="">
        {
            public string Reference { get; set; }
        }

        public class ParameterReference : IContentNode // <paramref>
        {
            public string Value { get; set; }
        }
        
        public class TypeParameterReference : IContentNode // <typeparamref>
        {
            public string Value { get; set; }
        }

        public class List : IContentNode // <list>
        {
            public Item Header { get; set; } // <listheader>

            public string Type { get; set; }
            
            public IEnumerable<Item> Items { get; set; }

            public class Item // <item>
            {
                public Content Value { get; set; }
            }
        }

    }
}

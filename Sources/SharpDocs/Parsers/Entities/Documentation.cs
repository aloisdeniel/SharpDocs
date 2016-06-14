namespace SharpDocs.Parsers.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents all the documentation referenced in an XML generated document file.
    /// </summary>
    public class Documentation
    {
        /// <summary>
        /// The assembly description.
        /// </summary>
        public Member Assembly { get; set; }

        /// <summary>
        /// All the members.
        /// </summary>
        public IEnumerable<Member> Members { get; set; }
    }
}

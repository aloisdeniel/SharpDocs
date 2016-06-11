namespace SharpDocs.Samples
{
    /// <summary>
    /// An example abstract class.
    /// </summary>
    public abstract class AbstractExample
    {
        /// <summary>
        /// Indicates whether the instance is cool or not.
        /// </summary>
        public bool IsCool { get; set; }

        /// <summary>
        /// An example abstract property.
        /// </summary>
        public abstract bool IsAbstract { get; }

        /// <summary>
        /// An example abstract method;
        /// </summary>
        public abstract void TestAbstract();
    }
}

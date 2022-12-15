namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which indicates that changes to this member should cause validation of another member's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class TriggerValidationAttribute : Attribute
    {
        /// <summary>
        /// The members which should be validated when this member is changed, in the order that validation should occur.
        /// </summary>
        public string[] memberNames;
        /// <summary>
        /// Indicates that changes to this member should cause validation of another member's value.
        /// </summary>
        public TriggerValidationAttribute(string memberName) => this.memberNames = new string[] { memberName };
        public TriggerValidationAttribute(params string[] memberNames) => this.memberNames = memberNames;
    }
}

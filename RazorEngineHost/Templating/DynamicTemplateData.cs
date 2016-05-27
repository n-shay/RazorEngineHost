namespace RazorEngineHost.Templating
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// Defines a dynamic template data.
    /// </summary>
    [Serializable]
    public class DynamicTemplateData : DynamicObject
    {
        #region Fields

        private readonly IDictionary<string, object> dict =
            new Dictionary<string, object>();

        #endregion

        /// <summary>
        /// Create a new DynamicTemplateData.
        /// </summary>
        public DynamicTemplateData()
        {
        }

        /// <summary>
        /// Create a new DynamicTemplateData by copying the given dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        public DynamicTemplateData(IDictionary<string, object> dictionary)
            : this()
        {
            this.AddDictionary(dictionary);
        }

        /// <summary>
        /// Create a copy of the given DynamicTemplateData.
        /// </summary>
        /// <param name="viewbag"></param>
        public DynamicTemplateData(DynamicTemplateData viewbag)
            : this(viewbag.dict)
        {
        }
        
        #region Methods

        /// <summary>
        /// Adds the given dictionary to the current DynamicTemplateData instance.
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddDictionary(IDictionary<string, object> dictionary)
        {
            foreach (var item in dictionary)
            {
                this.dict.Add(item.Key, item.Value);
            }
        }
        
        /// <summary>
        /// Adds a single value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AddValue(string propertyName, object value)
        {
            this.dict.Add(propertyName, value);
        }

        /// <summary>
        /// Gets the set of dynamic member names.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{String}"/>.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.dict.Keys;
        }

        /// <summary>
        /// Attempts to read a dynamic member from the object.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="result">The result instance.</param>
        /// <returns>True, always.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.dict.ContainsKey(binder.Name)
                ? this.dict[binder.Name]
                : null;

            return true;
        }

        /// <summary>
        /// Attempts to set a value on the object.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True, always.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (this.dict.ContainsKey(binder.Name))
                this.dict[binder.Name] = value;
            else
                this.dict.Add(binder.Name, value);

            return true;
        }

        #endregion
    }
}
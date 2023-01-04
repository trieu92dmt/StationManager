using System.ComponentModel;
using System.Resources;

namespace ISD.Core.Attributes
{
    public class LocalizationAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        private readonly bool _isIgnore;
        public LocalizationAttribute(string resourceKey, Type resourceType, bool isIgnore = false)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
            _isIgnore = isIgnore;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);
                return string.IsNullOrEmpty(displayName) ? string.Format("[[{0}]]", _resourceKey) : displayName;
            }
        }

        public bool IsIgnore
        {
            get
            {
                return _isIgnore;
            }
        }
    }
}

using System;
using System.Linq;

namespace CompSorting.Settings
{
    internal class CustomSerializedFieldOptionsAttribute : BaseSerializedTypeOptionsAttribute
    {
        public override SerializedType[] GetOptions(SerializedType serializedType)
        {
            var _currentTypes = CompSortingRepository.GetTypes();

            var allTypes = ComponentDatabase.GetAllTypes();

            return allTypes.Where(s =>
            {
                if (s == serializedType)
                    return true;

                if (_currentTypes.Any(t => t == s))
                    return false;

                return true;
            })
                .Select(s => new SerializedType(s))
                .ToArray();
        }

        public static Type FirstUnselected()
        {
            var currentTypes = CompSortingRepository.GetTypes();

            var allTypes = ComponentDatabase.GetAllTypes();
            var unselectedType = allTypes.Where(s => !currentTypes.Any(t => t.AssemblyQualifiedName == s.AssemblyQualifiedName)).FirstOrDefault();

            return unselectedType;
        }
    }
}
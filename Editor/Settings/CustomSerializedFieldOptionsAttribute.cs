using System;
using System.Linq;

namespace CompSorting.Settings
{
    internal class CustomSerializedFieldOptionsAttribute : BaseSerializedTypeOptionsAttribute
    {
        public override string[] GetOptions(SerializedType serializedType)
        {
            var currentTypes = CompSortingRepository.GetTypes();

            var allTypes = ComponentDatabase.GetAllTypes();
            allTypes = allTypes.Where(s => !currentTypes.Any(t => t.Name == s.Name)).ToList();

            return allTypes.Select(s => s.Name).ToArray();
        }

        public static Type FirstUnselected()
        {
            var currentTypes = CompSortingRepository.GetTypes();

            var allTypes = ComponentDatabase.GetAllTypes();
            var unselectedType = allTypes.Where(s => !currentTypes.Any(t => t.Name == s.Name)).FirstOrDefault();

            return unselectedType;
        }
    }
}
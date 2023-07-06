using System.Linq;

namespace CompSorting
{
    public class AllSerializedTypeOptionsAttribute : BaseSerializedTypeOptionsAttribute
    {
        static AllSerializedTypeOptionsAttribute()
        {
        }

        public override string[] GetOptions(SerializedType serializedType)
        {
            var types = ComponentDatabase.GetAllTypes();
            var options = types.Select(s => s.Name).ToArray();
            return options;
        }
    }
}
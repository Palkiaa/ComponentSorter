using System.Linq;

namespace CompSorting
{
    public class AllSerializedTypeOptionsAttribute : BaseSerializedTypeOptionsAttribute
    {
        static AllSerializedTypeOptionsAttribute()
        {
        }

        public override SerializedType[] GetOptions(SerializedType serializedType)
        {
            var types = ComponentDatabase.GetAllTypes();
            var options = types.Select(s => new SerializedType(s)).ToArray();
            return options;
        }
    }
}
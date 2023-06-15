using PropertyBinder.ChangeListener;

namespace PropertyBinder.Model
{
    /// <summary>
    /// Class that has two properties and id and a name
    /// </summary>
    internal sealed class ItemDto : Trackable
    {     

        public ItemDto(int id, string name)
        {
            Id = id;
            Name = name;           
        }
        public int Id { get; set; }       
        public string Name { get; set; }
    }
}

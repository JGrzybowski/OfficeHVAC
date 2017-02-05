namespace OfficeHVAC.Models
{
    public interface IRange<T>
    {
        T Min { get; set; }
        T Max { get; set; }
        bool Contains(T item, bool open = true);
    }
}

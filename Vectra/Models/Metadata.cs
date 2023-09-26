namespace Vectra.Models
{
    public class Metadata : Dictionary<string, object>
    {
        public Metadata() : base()
        {
        }

        public Metadata(Dictionary<string, object> dictionary) : base(dictionary)
        {
        }

        public Metadata Clone()
        {
            return new Metadata(this);
        }
    }
}

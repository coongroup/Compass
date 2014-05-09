namespace Compass.Coondornator
{
    public class OmssaParameter
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public OmssaParameter(string name, string value = "")
        {
            Name = name.ToLower();
            Value = value;
        }

        public override string ToString()
        {
            if(string.IsNullOrWhiteSpace(Value))
            {
                return string.Format("-{0}", Name);
            }
            return string.Format("-{0} {1}", Name, Value);
        }
    }
}

namespace Phosphinator
{
    public class Fragment
    {
        private FragmentType fragmentType;

        public FragmentType FragmentType
        {
            get { return fragmentType; }
            set { fragmentType = value; }
        }

        private int number;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private double mass;

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Fragment(FragmentType fragmentType, int number, double mass)
        {
            this.fragmentType = fragmentType;
            this.number = number;
            this.mass = mass;
        }

        public override string ToString()
        {
            return fragmentType.ToString() + number.ToString();
        }
    }
}

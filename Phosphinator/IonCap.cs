namespace Phosphinator
{
    public class IonCap
    {
        private double mass;

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public IonCap(double mass)
        {
            this.mass = mass;
        }
    }
}

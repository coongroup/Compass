using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coon.Compass.Procyon
{
    public class ComparisonLine : INotifyPropertyChanged
    {
        public BindingList<string> Numerator { get; set; }
        public BindingList<string> Denominator { get; set; }
        public bool TestSignificance { get; set; }

        public ComparisonLine(BindingList<string> numerator, BindingList<string> denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            TestSignificance = true;
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

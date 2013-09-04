using System.Drawing;
using System.Windows.Forms;

namespace Compass.ProteinHoarder
{
    public partial class OutputFileHeaderForm : UserControl
    {
        public OutputFileHeaderForm()
        {
            InitializeComponent();
            LoadPossibleColumns();
        }

        int itemIndex = 0;

        public void LoadPossibleColumns()
        {
            checkedListBox1.AllowDrop = true;
            checkedListBox1.MouseDown += checkedListBox1_MouseDown;
            checkedListBox1.DragOver += checkedListBox1_DragOver;
            checkedListBox1.DragDrop += checkedListBox1_DragDrop;           

            checkedListBox1.Items.Clear();
            checkedListBox1.Items.Add("Protein Group #");
            checkedListBox1.Items.Add("Sequence Coverage");
            checkedListBox1.Items.Add("# Quantifiable PSMs");
            checkedListBox1.Items.Add("# Quantifiable Peptides");   
        }

  

        void checkedListBox1_DragDrop(object sender, DragEventArgs e)
        {
            CheckedListBox clbSender = sender as CheckedListBox;            
            int newIndex = clbSender.IndexFromPoint(clbSender.PointToClient(new Point(e.X, e.Y)));
            if (newIndex >= 0)
            {
                string temp = (string)clbSender.Items[newIndex];
                clbSender.Items[newIndex] = clbSender.Items[itemIndex];
                clbSender.Items[itemIndex] = temp;               
            }            
        }

        void checkedListBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.String)))
            {
                e.Effect = DragDropEffects.Move;              
            }           
        }

        void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            CheckedListBox clb = sender as CheckedListBox;
            itemIndex = clb.IndexFromPoint(e.X, e.Y);
            if (itemIndex >= 0 & e.Button == MouseButtons.Left)
            {
                clb.DoDragDrop(clb.Items[itemIndex], DragDropEffects.Move);
            }
        }
    }

}


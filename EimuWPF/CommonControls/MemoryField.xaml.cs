using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Eimu.CommonControls
{
    /// <summary>
    /// Interaction logic for MemoryField.xaml
    /// </summary>
    public partial class MemoryField : UserControl
    {
        public MemoryField()
        {
            InitializeComponent();
        }

        public void MarkInvalid()
        {
            Color col = Color.FromRgb(255, 100, 100);
            this.m_MemoryValueA.Background = new SolidColorBrush(col);
            this.m_MemoryValueB.Background = new SolidColorBrush(col);
        }

        public void MarkValid()
        {
            Color col = Color.FromArgb(255, 255, 255, 255);
            this.m_MemoryValueA.Background = new SolidColorBrush(col);
            this.m_MemoryValueB.Background = new SolidColorBrush(col);
        }

        public ushort Address
        {
            get { return ushort.Parse(this.m_MemoryAddress.Text); }
            set { this.m_MemoryAddress.Text = value.ToString("X4"); }
        }

        public byte ValueA
        {
            get { return byte.Parse(this.m_MemoryValueA.Text); }
            set { this.m_MemoryValueA.Text = value.ToString("X2"); }
        }

        public byte ValueB
        {
            get { return byte.Parse(this.m_MemoryValueB.Text); }
            set { this.m_MemoryValueB.Text = value.ToString("X2"); }
        }
    }
}

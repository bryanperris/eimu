using System;
using System.Collections.Generic;
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
using Eimu.Core;
using System.Threading;

namespace Eimu.CommonControls
{
	/// <summary>
	/// Interaction logic for MemoryViewer.xaml
	/// </summary>
	public partial class MemoryViewer : UserControl
	{
		List<MemoryField> m_Fields;
		Memory m_Memory;

		public MemoryViewer()
		{
			this.InitializeComponent();
			m_StackPanel_Fields.Loaded += new RoutedEventHandler(m_StackPanel_Fields_Loaded);
		}

		void m_StackPanel_Fields_Loaded(object sender, RoutedEventArgs e)
		{
			GenerateFields();

			if (m_Memory != null)
			{
				UpdateFields();
			}
		}

		private void GenerateFields()
		{
			m_Fields = new List<MemoryField>();
			m_StackPanel_Fields.Children.Clear();
			double size = this.Height;
			
			while (size > 0)
			{
				MemoryField f = new MemoryField();

				if ((size -= f.Height) > 0)
				{
					m_Fields.Add(f);
					m_StackPanel_Fields.Children.Add(f);
				}
			}
		}

		public void UpdateFields()
		{
			if (m_Fields == null)
				return;

			int offset = (int)m_ScrollBar_MemScroll.Value;

			for (int i = 0; i < m_Fields.Count; i++)
			{
				m_Fields[i].Address = (ushort)offset;

				if (offset < (m_Memory.Size - 1))
				{
					
					m_Fields[i].ValueA = m_Memory[offset];
					m_Fields[i].ValueB = m_Memory[offset + 1];
					m_Fields[i].MarkValid();
				}
				else
				{
					m_Fields[i].ValueA = 0;
					m_Fields[i].ValueB = 0;
					m_Fields[i].MarkInvalid();
				}

				offset += 2;
			}

		}

		public void SetMemory(Memory memory)
		{
			m_Memory = memory;
			m_ScrollBar_MemScroll.Value = 0;
			m_ScrollBar_MemScroll.Minimum = 0;
			m_ScrollBar_MemScroll.Maximum = (double)memory.Size;
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
		}

		private void m_ScrollBar_MemScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateFields();
		}

		public bool GotoAddress(ushort address)
		{
			if (address > m_ScrollBar_MemScroll.Maximum)
			{
				return false;
			}
			else
			{
				m_ScrollBar_MemScroll.Value = (double)address;
				UpdateFields();
				return true;
			}
		}

		public int FieldCount
		{
			get { return this.m_Fields.Count; }
		}

		public ushort CurrentAddress
		{
			get
			{
				if (m_Fields.Count > 0)
				{
					return m_Fields[0].Address;
				}
				else
					return 0;
			}
		}
	}
}
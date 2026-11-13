using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

internal class LoopLinkItem : INotifyPropertyChanged
{
	public delegate bool IsDuplicateDelegate(string linkName);

	public IsDuplicateDelegate IsDuplicate;

	private string _linkName = string.Empty;

	public string Name
	{
		get
		{
			return _linkName;
		}
		set
		{
			string iDPattern = new LinkAutoNumberingAttribute("", "").IDPattern;
			if (Regex.Match(value, iDPattern).Success || string.IsNullOrEmpty(value))
			{
				if (IsDuplicate != null && !IsDuplicate(value))
				{
					_linkName = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("LoopLinkItem"));
					}
				}
				else
				{
					MessageBox.Show(Resource.Message_CreateLoop_LinkNameDumplicated, Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
			else
			{
				MessageBox.Show(Resource.Message_Name_Link, Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;
}

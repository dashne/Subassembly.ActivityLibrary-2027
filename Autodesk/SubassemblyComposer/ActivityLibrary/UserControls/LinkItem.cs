using System.ComponentModel;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

internal class LinkItem : INotifyPropertyChanged
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
			if (GeometryNamePatterns.IsLinkName(value) || GeometryNamePatterns.IsLoopName(value) || GeometryNamePatterns.IsLoopLinkName(value) || string.IsNullOrEmpty(value))
			{
				if (IsDuplicate != null && !IsDuplicate(value))
				{
					_linkName = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("LinkItem"));
					}
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

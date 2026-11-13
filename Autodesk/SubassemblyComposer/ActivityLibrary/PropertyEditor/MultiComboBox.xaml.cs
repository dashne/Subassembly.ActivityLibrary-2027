using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

public partial class MultiComboBox : UserControl, INotifyPropertyChanged, IComponentConnector, IStyleConnector
{
	private enum CodeInsertMode
	{
		AppendDelimiterBefore,
		AppendDelimiterAfter,
		AppendDelimiterBothEnd,
		ReplaceLastCode,
		End
	}

	public enum ListBoxItemType
	{
		Codes,
		Variables,
		Parameters
	}

	public class ListBoxItem : INotifyPropertyChanged
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public ListBoxItemType Type { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ListBoxItem(string name, ListBoxItemType itemType)
		{
			Name = name;
			Type = itemType;
		}

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}

	private const char LiteralStartSymbol = '"';

	private const char LiteralEndSymbol = '"';

	private const char DelimiterSymbol = ',';

	private const char SpaceChar = ' ';

	private readonly RoutedEventHandler EditableTextBox_HandledMouseDownEventHandler;

	private ObservableCollection<ListBoxItem> _itemsList = new ObservableCollection<ListBoxItem>();

	public static readonly DependencyProperty CodeItemsProperty = DependencyProperty.Register("CodeItems", typeof(object), typeof(MultiComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CodeItemsPropertyChangedCallback));

	public static readonly DependencyProperty SelectedItemsStringProperty = DependencyProperty.Register("SelectedItemsString", typeof(string), typeof(MultiComboBox), new FrameworkPropertyMetadata(null, SelectedItemsStringPropertyChangedCallback));

	private string _text = string.Empty;

	public static readonly DependencyProperty PromptProperty = DependencyProperty.Register("Prompt", typeof(string), typeof(MultiComboBox), new FrameworkPropertyMetadata());

	private bool DisableListBoxSelectionChanged { get; set; }

	public ObservableCollection<ListBoxItem> ItemsList
	{
		get
		{
			return _itemsList;
		}
		set
		{
			_itemsList = value;
			OnPropertyChanged("ItemsList");
		}
	}

	public ObservableCollection<CodeItem> CodeItems
	{
		get
		{
			return (ObservableCollection<CodeItem>)GetValue(CodeItemsProperty);
		}
		set
		{
			SetValue(CodeItemsProperty, value);
		}
	}

	public string SelectedItemsString
	{
		get
		{
			return (string)GetValue(SelectedItemsStringProperty);
		}
		set
		{
			SetValue(SelectedItemsStringProperty, value);
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			OnPropertyChanged("Text");
		}
	}

	public string Prompt
	{
		get
		{
			return (string)GetValue(PromptProperty);
		}
		set
		{
			SetValue(PromptProperty, value);
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public MultiComboBox()
	{
		InitializeComponent();
		DisableListBoxSelectionChanged = false;
		EditableTextBox_HandledMouseDownEventHandler = EditableTextBox_HandledMouseDown;
	}

	private static void CodeItemsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		MultiComboBox multiComboBox = dependencyObject as MultiComboBox;
		INotifyCollectionChanged notifyCollectionChanged = e.OldValue as INotifyCollectionChanged;
		INotifyCollectionChanged notifyCollectionChanged2 = e.NewValue as INotifyCollectionChanged;
		if (notifyCollectionChanged != null)
		{
			notifyCollectionChanged.CollectionChanged -= multiComboBox.OnCodeItemsCollectionChanged;
		}
		if (notifyCollectionChanged2 != null)
		{
			notifyCollectionChanged2.CollectionChanged += multiComboBox.OnCodeItemsCollectionChanged;
		}
	}

	private void OnCodeItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		updateItemsList();
	}

	private static void SelectedItemsStringPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		if (dependencyObject is MultiComboBox multiComboBox)
		{
			multiComboBox.Text = multiComboBox.SelectedItemsString;
		}
	}

	protected void OnPropertyChanged([CallerMemberName] string name = null)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}

	private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		DisableListBoxSelectionChanged = true;
		filterItemsList();
		updateSelectedItems();
		DisableListBoxSelectionChanged = false;
	}

	private void EditableTextBox_HandledMouseDown(object sender, RoutedEventArgs e)
	{
		if (sender is TextBox textBox)
		{
			if (textBox.Text.Length > 1 && textBox.Text.Last() != ',' && textBox.CaretIndex == textBox.Text.Length)
			{
				int caretIndex = ((textBox.CaretIndex == textBox.Text.Length) ? (textBox.Text.Length + 1) : textBox.CaretIndex);
				textBox.Text += ",";
				textBox.CaretIndex = caretIndex;
			}
			else if (textBox.Text.Length > 1 && textBox.Text.Last() == ',' && textBox.CaretIndex != textBox.Text.Length)
			{
				int caretIndex2 = textBox.CaretIndex;
				textBox.Text = textBox.Text.TrimEnd(',');
				textBox.CaretIndex = caretIndex2;
			}
		}
	}

	private void EditableTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		TextBox textBox = sender as TextBox;
		ScrollViewer scrollViewer = FindAncestor<ScrollViewer>(this);
		Window window = FindAncestor<Window>(this);
		if (textBox != null)
		{
			DisableListBoxSelectionChanged = true;
			Text = Text.DeleteRepeatCommas();
			Text = Text.Trim(',');
			DisableListBoxSelectionChanged = false;
			((CollectionView)CollectionViewSource.GetDefaultView((CheckableCombo.Template.FindName("CodeListBox", CheckableCombo) as ListBox).ItemsSource)).GroupDescriptions.Clear();
			SelectedItemsString = Text;
			FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
			textBox.RemoveHandler(UIElement.MouseDownEvent, EditableTextBox_HandledMouseDownEventHandler);
			if (scrollViewer != null)
			{
				scrollViewer.ScrollChanged -= OnParentScrollViewerScrollChanged;
			}
			if (window != null)
			{
				window.LocationChanged -= OnMainWindowPositionChanged;
				window.SizeChanged -= OnMainWindowPositionChanged;
			}
		}
	}

	private void EditableTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		if (sender is TextBox textBox)
		{
			updateItemsList();
			DisableListBoxSelectionChanged = true;
			updateSelectedItems();
			DisableListBoxSelectionChanged = false;
			CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView((CheckableCombo.Template.FindName("CodeListBox", CheckableCombo) as ListBox).ItemsSource);
			PropertyGroupDescription item = new PropertyGroupDescription("Type");
			collectionView.GroupDescriptions.Add(item);
			textBox.AddHandler(UIElement.MouseDownEvent, EditableTextBox_HandledMouseDownEventHandler, handledEventsToo: true);
			ScrollViewer scrollViewer = FindAncestor<ScrollViewer>(this);
			if (scrollViewer != null)
			{
				scrollViewer.ScrollChanged += OnParentScrollViewerScrollChanged;
			}
			Window window = FindAncestor<Window>(this);
			if (window != null)
			{
				window.LocationChanged += OnMainWindowPositionChanged;
				window.SizeChanged += OnMainWindowPositionChanged;
			}
		}
	}

	private void OnParentScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (sender is ScrollViewer { ComputedVerticalScrollBarVisibility: Visibility.Visible } scrollViewer && !IsMouseOver && (e.VerticalChange != 0.0 || e.ViewportHeightChange != 0.0 || e.ViewportWidthChange != 0.0))
		{
			FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
			Keyboard.ClearFocus();
			scrollViewer.ScrollChanged -= OnParentScrollViewerScrollChanged;
		}
	}

	private void OnMainWindowPositionChanged(object sender, EventArgs e)
	{
		if (sender is Window window)
		{
			FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
			Keyboard.ClearFocus();
			window.LocationChanged -= OnMainWindowPositionChanged;
			window.SizeChanged -= OnMainWindowPositionChanged;
		}
	}

	private void CodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (DisableListBoxSelectionChanged)
		{
			return;
		}
		DisableListBoxSelectionChanged = true;
		ListBox listBox = sender as ListBox;
		HashSet<string> hashSet = new HashSet<string>(Text.Split(',').ToList());
		HashSet<string> hashSet2 = new HashSet<string>(from item3 in listBox.SelectedItems.Cast<ListBoxItem>().ToList()
			select convertFullCodeName(item3));
		foreach (ListBoxItem item3 in (IEnumerable)listBox.Items)
		{
			string item2 = convertFullCodeName(item3);
			if (hashSet2.Contains(item2) && !hashSet.Contains(item2))
			{
				insertCodeAtCaret(item3);
				break;
			}
			if (!hashSet2.Contains(item2) && hashSet.Contains(item2))
			{
				removeCodeFromText(item3);
				break;
			}
		}
		DisableListBoxSelectionChanged = false;
	}

	private CodeInsertMode isableToInsertCode()
	{
		int textboxCaretIndex = getTextboxCaretIndex();
		if (!string.IsNullOrEmpty(getLastTypingCode(Text)))
		{
			return CodeInsertMode.ReplaceLastCode;
		}
		if (Text.Length == 0 || textboxCaretIndex == 0)
		{
			return CodeInsertMode.AppendDelimiterAfter;
		}
		if (textboxCaretIndex == Text.Length)
		{
			if (Text[textboxCaretIndex - 1] == ',')
			{
				return CodeInsertMode.AppendDelimiterAfter;
			}
			return CodeInsertMode.AppendDelimiterBothEnd;
		}
		if (Text[textboxCaretIndex - 1] == ',')
		{
			return CodeInsertMode.AppendDelimiterAfter;
		}
		if (Text[textboxCaretIndex] == ',')
		{
			return CodeInsertMode.AppendDelimiterBefore;
		}
		return CodeInsertMode.End;
	}

	private void insertCodeAtCaret(ListBoxItem item)
	{
		string text = convertFullCodeName(item);
		int textboxCaretIndex = getTextboxCaretIndex();
		string text2 = string.Empty;
		string text3;
		int textboxCaretIndex2;
		switch (isableToInsertCode())
		{
		case CodeInsertMode.AppendDelimiterBefore:
			text2 = "," + text;
			text3 = Text.Insert(textboxCaretIndex, text2);
			textboxCaretIndex2 = textboxCaretIndex + text2.Length;
			break;
		case CodeInsertMode.AppendDelimiterAfter:
			text2 = text + ",";
			text3 = Text.Insert(textboxCaretIndex, text2);
			textboxCaretIndex2 = textboxCaretIndex + text2.Length;
			break;
		case CodeInsertMode.AppendDelimiterBothEnd:
			text2 = "," + text + ",";
			text3 = Text.Insert(textboxCaretIndex, text2);
			textboxCaretIndex2 = textboxCaretIndex + text2.Length;
			break;
		case CodeInsertMode.ReplaceLastCode:
		{
			text2 = text + ",";
			string text4 = Text.Split(',').Last();
			text3 = Text.Remove(Text.Length - text4.Length) + text2;
			textboxCaretIndex2 = Text.Length - text4.Length + text2.Length;
			break;
		}
		case CodeInsertMode.End:
			if (Text.Last() != ',')
			{
				text2 += ",";
			}
			text2 = text2 + text + ",";
			text3 = Text.Insert(Text.Length, text2);
			textboxCaretIndex2 = Text.Length + text2.Length;
			break;
		default:
			text3 = Text;
			textboxCaretIndex2 = 0;
			break;
		}
		Text = text3;
		setTextboxCaretIndex(textboxCaretIndex2);
	}

	private void setTextboxCaretIndex(int index)
	{
		if (CheckableCombo.Template.FindName("EditableTextBox", CheckableCombo) is TextBox textBox)
		{
			if (index > textBox.Text.Length)
			{
				textBox.CaretIndex = textBox.Text.Length;
			}
			else if (index < 0)
			{
				textBox.CaretIndex = 0;
			}
			else
			{
				textBox.CaretIndex = index;
			}
		}
	}

	private int getTextboxCaretIndex()
	{
		if (!(CheckableCombo.Template.FindName("EditableTextBox", CheckableCombo) is TextBox textBox))
		{
			return -1;
		}
		return textBox.CaretIndex;
	}

	private void removeCodeFromText(ListBoxItem item)
	{
		string text = convertFullCodeName(item);
		if (text.Contains("+"))
		{
			text = text.Insert(text.IndexOf("+"), "\\");
		}
		Match match = new Regex("((^\\s*)|(,\\s*))" + text + "((\\s*,)|(\\s*$))").Match(Text);
		if (!match.Success)
		{
			return;
		}
		string text2 = match.Value.Trim(',');
		int num = ((Text[match.Index] == ',') ? (match.Index + 1) : match.Index);
		int num2 = getTextboxCaretIndex();
		string text3 = Text.Remove(num, text2.Length);
		if (num <= num2)
		{
			if (num2 > num && num2 <= num + text2.Length - 1)
			{
				num2 = num;
			}
			else if (num2 > num + text2.Length - 1)
			{
				num2 -= text2.Length;
			}
		}
		string lastTypingCode = getLastTypingCode(text3);
		if (!string.IsNullOrEmpty(lastTypingCode))
		{
			text3 = text3.Remove(text3.Length - lastTypingCode.Length);
			num2 -= lastTypingCode.Length;
		}
		string text4 = text3.Substring(0, num2);
		num2 -= text4.Length - text4.DeleteRepeatCommas().Length;
		text3 = text3.DeleteRepeatCommas();
		num2 -= text3.Length - text3.TrimStart(',').Length;
		text3 = text3.TrimStart(',');
		Text = text3;
		setTextboxCaretIndex(num2);
	}

	private string convertFullCodeName(ListBoxItem item)
	{
		if (item.Type == ListBoxItemType.Codes)
		{
			return "\"" + item.Name + "\"";
		}
		return item.Name;
	}

	private void updateItemsList()
	{
		ItemsList.Clear();
		addToItemsList(CodeItems.Select((CodeItem codeitem) => codeitem.Name).ToList(), ListBoxItemType.Codes);
		addToItemsList(getInputParameter(), ListBoxItemType.Parameters);
		addToItemsList(getVariables(), ListBoxItemType.Variables);
		void addToItemsList(List<string> targetList, ListBoxItemType type)
		{
			foreach (string target in targetList)
			{
				ItemsList.Add(new ListBoxItem(target, type));
			}
		}
	}

	private void updateSelectedItems()
	{
		if (Text == null)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		string[] array = Text.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (!string.IsNullOrEmpty(text))
			{
				bool num = text.Length > 1 && text.StartsWith('"'.ToString()) && text.EndsWith('"'.ToString());
				bool flag = text.Length > 0 && (!text.StartsWith('"'.ToString()) || !text.EndsWith('"'.ToString()));
				if (num | flag)
				{
					hashSet.Add(text);
				}
			}
		}
		ListBox listBox = CheckableCombo.Template.FindName("CodeListBox", CheckableCombo) as ListBox;
		listBox.SelectedItems.Clear();
		foreach (ListBoxItem items in ItemsList)
		{
			if (hashSet.Contains(convertFullCodeName(items)))
			{
				listBox.SelectedItems.Add(items);
			}
		}
	}

	private void filterItemsList()
	{
		CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView((CheckableCombo.Template.FindName("CodeListBox", CheckableCombo) as ListBox).ItemsSource);
		if (collectionView != null)
		{
			string filterText = getLastTypingCode(Text);
			collectionView.Filter = (object item) => string.IsNullOrEmpty(filterText) || ((ListBoxItem)item).Name.ToLower().Contains(filterText.ToLower());
			collectionView.Refresh();
		}
	}

	private string getLastTypingCode(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string text2 = text.Split(',').Last().Trim();
		bool num = text2.Length > 1 && text2.StartsWith('"'.ToString()) && text2.EndsWith('"'.ToString());
		bool flag = text2.Length > 0 && (getInputParameter().Contains(text2) || getVariables().Contains(text2));
		if (num | flag)
		{
			return null;
		}
		return text2.TrimStart('"');
	}

	private List<string> getInputParameter()
	{
		return ActivityDesigntime.Instance.GetInputParameterNames<string>().ToList();
	}

	private List<string> getVariables()
	{
		return ActivityDesigntime.Instance.GetAllUserDefinedVariableNames(typeof(string)).ToList();
	}

	private static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
	{
		if (obj != null)
		{
			DependencyObject dependencyObject = obj;
			do
			{
				dependencyObject = GetParent(dependencyObject);
				if (dependencyObject is T)
				{
					return dependencyObject as T;
				}
			}
			while (dependencyObject != null);
		}
		return null;
	}

	private static DependencyObject GetParent(DependencyObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ContentElement)
		{
			DependencyObject parent = ContentOperations.GetParent(obj as ContentElement);
			if (parent != null)
			{
				return parent;
			}
			if (obj is FrameworkContentElement)
			{
				return (obj as FrameworkContentElement).Parent;
			}
			return null;
		}
		return VisualTreeHelper.GetParent(obj);
	}
}

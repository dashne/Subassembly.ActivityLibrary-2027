using System;
using System.Activities;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class ExpressionDialog : Window, IComponentConnector
{
	private Argument _oldPropertyValue;

	private ModelProperty _modelItemProperty;

	private static double storedWidth = -1.0;

	private static double storedHeight;

	private static double storedLeft;

	private static double storedTop;

	private ExpressionDialog(ModelItem modelItem, string propertyName, Argument argumentValue, Type argumentType)
	{
		InitializeComponent();
		WindowStartupLocation = WindowStartupLocation.CenterScreen;
		WindowStyle = WindowStyle.ToolWindow;
		Binding binding = new Binding(".")
		{
			Source = modelItem
		};
		VBExpressionBox.SetBinding(ExpressionTextBox.OwnerActivityProperty, binding);
		Binding binding2 = new Binding(propertyName)
		{
			Source = modelItem,
			Mode = BindingMode.TwoWay,
			Converter = new ArgumentToExpressionConverter(),
			ConverterParameter = ArgumentDirection.In
		};
		VBExpressionBox.SetBinding(ExpressionTextBox.ExpressionProperty, binding2);
		VBExpressionBox.UseLocationExpression = false;
		if (argumentValue is OutArgument || argumentValue is InOutArgument)
		{
			VBExpressionBox.UseLocationExpression = true;
		}
		VBExpressionBox.ExpressionType = argumentType;
		_oldPropertyValue = argumentValue;
		_modelItemProperty = modelItem.Properties[propertyName];
	}

	public static void ShowDialog(ModelItem modelItem, string propertyName, Argument argumentValue, Type argumentType)
	{
		new ExpressionDialog(modelItem, propertyName, argumentValue, argumentType).InnerShowDialog();
	}

	private void InnerShowDialog()
	{
		if (storedWidth >= 0.0)
		{
			Width = storedWidth;
			Height = storedHeight;
			Left = storedLeft;
			Top = storedTop;
			WindowStartupLocation = WindowStartupLocation.Manual;
		}
		else
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		try
		{
			ShowDialog();
		}
		finally
		{
			storedHeight = Height;
			storedLeft = Left;
			storedTop = Top;
			storedWidth = Width;
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		if (e.OriginalSource is Button button && button == ButtonOK)
		{
			DialogResult = true;
		}
		else
		{
			DialogResult = false;
			_modelItemProperty.SetValue(_oldPropertyValue);
		}
		Close();
	}

	private void OnHelp(object sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo($"https://www.autodesk.com/civil3d-subassembly-composer-{Civil3DInfo.ReleaseYear}-enu?contextId=IDH_EXPRESSION_EDITOR")
		{
			UseShellExecute = true
		});
	}

	private void Window_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			DialogResult = false;
			_modelItemProperty.SetValue(_oldPropertyValue);
			Close();
		}
	}
}

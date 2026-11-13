namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class ActivityLibraryAppCallbacks
{
	public static ComputeOffsetLinks ComputeOffsetLinksCallback { get; set; }

	public static ComputeLoopLinks ComputeLoopLinksCallback { get; set; }

	public static RefreshSelectionData RefreshSelectionDataCallback { get; set; }

	public static RenameGeometryId RenameGeometryIdCallback { get; set; }

	public static InvokeTaskDlg InvokeTaskDlgCallback { get; set; }
}

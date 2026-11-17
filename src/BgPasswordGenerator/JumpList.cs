using System.Runtime.InteropServices;

namespace BgPasswordGenerator;

/// <summary>
/// Adds tasks to the taskbar button's right-click menu. WinRT's JumpList API needs package identity,
/// which this unpackaged app doesn't have, so this uses the shell's ICustomDestinationList directly.
/// The list follows the process's AppUserModelID, which Velopack sets to match its shortcuts.
/// </summary>
internal static class JumpList
{
    private static readonly (string Arguments, string Title, string Description)[] Tasks =
    [
        (StartupCommands.CopyPasswordArgument, "Copy new password",
            "Copies a new 18-character password without opening the window. It's cleared from the clipboard after 30 seconds."),
        (StartupCommands.CopyUuidArgument, "Copy new UUID", "Copies a new random UUID without opening the window."),
        (StartupCommands.OpenHashArgument, "Hash or encode text", "Opens the Hash / Base64 tab."),
    ];

    private static readonly Guid DestinationListClsid = new("77f10cf0-3db5-4966-b520-b7c54fd35ed6");
    private static readonly Guid EnumerableObjectCollectionClsid = new("2d3468c1-36a7-43b6-ac24-d3f02fd9607a");
    private static readonly Guid ShellLinkClsid = new("00021401-0000-0000-c000-000000000046");
    private static readonly PropertyKey TitleKey = new(new Guid("f29f85e0-4ff9-1068-ab91-08002b27b3d9"), 2);

    public static void Register()
    {
        var exe = Environment.ProcessPath;
        if (exe is null)
        {
            return;
        }

        try
        {
            var list = Create<ICustomDestinationList>(DestinationListClsid);
            var objectArrayId = typeof(IObjectArray).GUID;
            list.BeginList(out _, ref objectArrayId, out _);

            var tasks = Create<IObjectCollection>(EnumerableObjectCollectionClsid);
            foreach (var (arguments, title, description) in Tasks)
            {
                tasks.AddObject(CreateTask(exe, arguments, title, description));
            }
            list.AddUserTasks((IObjectArray)tasks);
            list.CommitList();
        }
        catch (COMException)
        {
            // The menu is a convenience; the app works the same without it
        }
    }

    private static IShellLinkW CreateTask(string exe, string arguments, string title, string description)
    {
        var link = Create<IShellLinkW>(ShellLinkClsid);
        link.SetPath(exe);
        link.SetArguments(arguments);
        link.SetDescription(description);
        link.SetIconLocation(exe, 0);

        var properties = (IPropertyStore)link;
        var key = TitleKey;
        var value = new PropVariant { VarType = VarTypeLpwstr, Value = Marshal.StringToCoTaskMemUni(title) };
        try
        {
            properties.SetValue(ref key, ref value);
            properties.Commit();
        }
        finally
        {
            PropVariantClear(ref value);
        }
        return link;
    }

    private static T Create<T>(Guid clsid) =>
        (T)Activator.CreateInstance(Type.GetTypeFromCLSID(clsid, throwOnError: true)!)!;

    private const ushort VarTypeLpwstr = 31;

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct PropertyKey(Guid FormatId, uint PropertyId);

    [StructLayout(LayoutKind.Sequential)]
    private struct PropVariant
    {
        public ushort VarType;
        private ushort _reserved1, _reserved2, _reserved3;
        public nint Value;
        private nint _padding;
    }

    [DllImport("ole32.dll")]
    private static extern int PropVariantClear(ref PropVariant value);

    // Interface members are declared in vtable order, including ones this file never calls

    [ComImport, Guid("6332debf-87b5-4670-90c0-5e57b408a49e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ICustomDestinationList
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string appId);
        void BeginList(out uint minSlots, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object removed);
        void AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string category, IObjectArray items);
        void AppendKnownCategory(int category);
        void AddUserTasks(IObjectArray tasks);
        void CommitList();
        void GetRemovedDestinations(ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object removed);
        void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string appId);
        void AbortList();
    }

    [ComImport, Guid("92ca9dcd-5622-4bba-a805-5e9f541bd8c9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IObjectArray
    {
        void GetCount(out uint count);
        void GetAt(uint index, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object item);
    }

    [ComImport, Guid("5632b1a4-e38a-400a-928a-d4cd63230295"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IObjectCollection
    {
        void GetCount(out uint count);
        void GetAt(uint index, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object item);
        void AddObject([MarshalAs(UnmanagedType.Interface)] object item);
        void AddFromArray(IObjectArray source);
        void RemoveObjectAt(uint index);
        void Clear();
    }

    [ComImport, Guid("000214f9-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellLinkW
    {
        void GetPath(nint file, int maxChars, nint findData, uint flags);
        void GetIDList(out nint idList);
        void SetIDList(nint idList);
        void GetDescription(nint name, int maxChars);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string name);
        void GetWorkingDirectory(nint directory, int maxChars);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string directory);
        void GetArguments(nint arguments, int maxChars);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string arguments);
        void GetHotkey(out short hotkey);
        void SetHotkey(short hotkey);
        void GetShowCmd(out int showCommand);
        void SetShowCmd(int showCommand);
        void GetIconLocation(nint iconPath, int maxChars, out int iconIndex);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string iconPath, int iconIndex);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string relativePath, uint reserved);
        void Resolve(nint window, uint flags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string file);
    }

    [ComImport, Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPropertyStore
    {
        void GetCount(out uint count);
        void GetAt(uint index, out PropertyKey key);
        void GetValue(ref PropertyKey key, out PropVariant value);
        void SetValue(ref PropertyKey key, ref PropVariant value);
        void Commit();
    }
}

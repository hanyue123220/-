
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Hikvision.WinForms
{
    public enum HikvisionStreamType : uint
    {
        Main = 0,
        Sub = 1,
        Third = 2,
        Fourth = 3
    }

    /// <summary>
    /// A small, thread-safe wrapper around Hikvision HCNetSDK for WinForms.
    /// The native HCNetSDK.dll and its HCNetSDKCom directory must be deployed
    /// beside the application (or supplied through sdkDirectory).
    /// </summary>
    public sealed class HikvisionCameraClient : IDisposable
    {
        private readonly object _syncRoot = new object();
        private readonly HCNetSDK.ExceptionCallback _exceptionCallback;
        private bool _sdkInitialized;
        private bool _disposed;
        private int _userId = -1;
        private int _previewHandle = -1;
        private bool _recording;

        public HikvisionCameraClient()
        {
            _exceptionCallback = OnSdkException;
        }

        /// <summary>Raised from an SDK callback thread. Keep handlers short.</summary>
       // public event Action<string>? Log;
        public event Action<string> Log;

        public bool IsInitialized
        {
            get { lock (_syncRoot) return _sdkInitialized; }
        }

        public bool IsLoggedIn
        {
            get { lock (_syncRoot) return _userId >= 0; }
        }

        public bool IsPreviewing
        {
            get { lock (_syncRoot) return _previewHandle >= 0; }
        }

        public bool IsRecording
        {
            get { lock (_syncRoot) return _recording; }
        }

        public int UserId
        {
            get { lock (_syncRoot) return _userId; }
        }

        /// <summary>
        /// Initializes HCNetSDK. Call this once before Login.
        /// </summary>
        //public void Initialize(string? sdkDirectory = null)
        public void Initialize(string sdkDirectory = null)
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                if (_sdkInitialized)
                    return;

                if (!string.IsNullOrWhiteSpace(sdkDirectory))
                {
                    string fullPath = Path.GetFullPath(sdkDirectory);
                    if (!Directory.Exists(fullPath))
                        throw new DirectoryNotFoundException("Hikvision SDK directory was not found: " + fullPath);

                    if (!NativeMethods.SetDllDirectory(fullPath))
                        throw new InvalidOperationException("Unable to set the Hikvision SDK directory. Win32 error: " + Marshal.GetLastWin32Error());
                }

                if (!HCNetSDK.NET_DVR_Init())
                    ThrowLastError("NET_DVR_Init");

                _sdkInitialized = true;
                HCNetSDK.NET_DVR_SetConnectTime(5000, 3);
                HCNetSDK.NET_DVR_SetReconnect(10000, 1);

                // Registering an exception callback makes disconnects and reconnects observable.
                if (!HCNetSDK.NET_DVR_SetExceptionCallBack_V30(0, IntPtr.Zero, _exceptionCallback, IntPtr.Zero))
                    WriteLog("NET_DVR_SetExceptionCallBack_V30 failed: " + HCNetSDK.NET_DVR_GetLastError());

                WriteLog("HCNetSDK initialized.");
            }
        }

        /// <summary>Logs in synchronously and returns the HCNetSDK user id.</summary>
        public int Login(string address, ushort port, string userName, string password)
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                EnsureInitialized();
                if (_userId >= 0)
                    throw new InvalidOperationException("A camera is already logged in.");
                if (string.IsNullOrWhiteSpace(address))
                    throw new ArgumentException("Camera address is required.", nameof(address));
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentException("Camera user name is required.", nameof(userName));
                if (password == null)
                    throw new ArgumentNullException(nameof(password));

                var loginInfo = new HCNetSDK.UserLoginInfo
                {
                    DeviceAddress = address,
                    UseTransport = 0,
                    Port = port,
                    UserName = userName,
                    Password = password,
                    LoginResultCallback = IntPtr.Zero,
                    UserData = IntPtr.Zero,
                    UseAsyncLogin = 0,
                    ProxyType = 0,
                    UseUtcTime = 0,
                    LoginMode = 0,
                    Https = 0,
                    Reserved = new byte[2]
                };
                var deviceInfo = HCNetSDK.DeviceInfoV40.Create();
                _userId = HCNetSDK.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
                if (_userId < 0)
                {
                    int error = HCNetSDK.NET_DVR_GetLastError();
                    _userId = -1;
                    throw new HikvisionException("NET_DVR_Login_V40 failed.", error);
                }

                WriteLog("Camera logged in: " + address + ":" + port);
                return _userId;
            }
        }

        /// <summary>
        /// Starts hardware-decoded preview in a WinForms control such as PictureBox.Panel.Handle.
        /// The control must already have a native handle (IsHandleCreated == true).
        /// </summary>
        public int StartPreview(IntPtr windowHandle, int channel = 1, HikvisionStreamType streamType = HikvisionStreamType.Main, bool blocked = true)
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                EnsureLoggedIn();
                if (windowHandle == IntPtr.Zero)
                    throw new ArgumentException("A valid WinForms window handle is required.", nameof(windowHandle));
                if (_previewHandle >= 0)
                    throw new InvalidOperationException("Preview is already running.");
                if (channel < 1)
                    throw new ArgumentOutOfRangeException(nameof(channel));

                var previewInfo = new HCNetSDK.PreviewInfo
                {
                    Channel = channel,
                    StreamType = (uint)streamType,
                    LinkMode = 0,
                    PreviewWindow = windowHandle,
                    Blocked = blocked ? 1 : 0,
                    DisplayBufferCount = 15,
                    ProtocolType = 0,
                    Reserved1 = 0,
                    Reserved2 = 0,
                    Reserved3 = 0
                };

                // A null real-data callback lets HCNetSDK render directly to hWnd.
                _previewHandle = HCNetSDK.NET_DVR_RealPlay_V40(_userId, ref previewInfo, null, IntPtr.Zero);
                if (_previewHandle < 0)
                {
                    int error = HCNetSDK.NET_DVR_GetLastError();
                    _previewHandle = -1;
                    throw new HikvisionException("NET_DVR_RealPlay_V40 failed.", error);
                }

                WriteLog("Preview started on channel " + channel + ".");
                return _previewHandle;
            }
        }

        public void StopPreview()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                StopPreviewCore(true);
            }
        }

        /// <summary>Captures a JPEG through the device channel, whether preview is running or not.</summary>
        public void CaptureJpeg(string filePath, int channel = 1, ushort pictureSize = 0, ushort pictureQuality = 2)
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                EnsureLoggedIn();
                if (channel < 1)
                    throw new ArgumentOutOfRangeException(nameof(channel));
                if (pictureQuality > 2)
                    throw new ArgumentOutOfRangeException(nameof(pictureQuality), "Hikvision JPEG quality is 0, 1, or 2.");

                string fullPath = PrepareOutputPath(filePath, ".jpg");
                var jpegParameter = new HCNetSDK.JpegParameter
                {
                    PictureSize = pictureSize,
                    PictureQuality = pictureQuality
                };

                if (!HCNetSDK.NET_DVR_CaptureJPEGPicture(_userId, channel, ref jpegParameter, fullPath))
                    ThrowLastError("NET_DVR_CaptureJPEGPicture");

                WriteLog("JPEG captured: " + fullPath);
            }
        }

        public void StartRecording(string filePath)
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                EnsurePreviewing();
                if (_recording)
                    throw new InvalidOperationException("Recording is already running.");

                string fullPath = PrepareOutputPath(filePath, ".mp4");
                if (!HCNetSDK.NET_DVR_SaveRealData(_previewHandle, fullPath))
                    ThrowLastError("NET_DVR_SaveRealData");

                _recording = true;
                WriteLog("Recording started: " + fullPath);
            }
        }

        public void StopRecording()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                StopRecordingCore(true);
            }
        }

        public void Logout()
        {
            lock (_syncRoot)
            {
                ThrowIfDisposed();
                StopRecordingCore(false);
                StopPreviewCore(false);

                if (_userId >= 0)
                {
                    if (!HCNetSDK.NET_DVR_Logout(_userId))
                        WriteLog("NET_DVR_Logout failed: " + HCNetSDK.NET_DVR_GetLastError());
                    else
                        WriteLog("Camera logged out.");
                    _userId = -1;
                }
            }
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                    return;

                try
                {
                    StopRecordingCore(false);
                    StopPreviewCore(false);
                    if (_userId >= 0)
                    {
                        HCNetSDK.NET_DVR_Logout(_userId);
                        _userId = -1;
                    }
                    if (_sdkInitialized)
                    {
                        HCNetSDK.NET_DVR_Cleanup();
                        _sdkInitialized = false;
                    }
                }
                finally
                {
                    _disposed = true;
                }
            }
            GC.SuppressFinalize(this);
        }

        private void StopPreviewCore(bool throwOnError)
        {
            if (_previewHandle < 0)
                return;

            bool success = HCNetSDK.NET_DVR_StopRealPlay(_previewHandle);
            int error = success ? 0 : HCNetSDK.NET_DVR_GetLastError();
            _previewHandle = -1;
            _recording = false;
            if (!success && throwOnError)
                throw new HikvisionException("NET_DVR_StopRealPlay failed.", error);
            if (!success)
                WriteLog("NET_DVR_StopRealPlay failed: " + error);
        }

        private void StopRecordingCore(bool throwOnError)
        {
            if (!_recording || _previewHandle < 0)
                return;

            bool success = HCNetSDK.NET_DVR_StopSaveRealData(_previewHandle);
            int error = success ? 0 : HCNetSDK.NET_DVR_GetLastError();
            _recording = false;
            if (!success && throwOnError)
                throw new HikvisionException("NET_DVR_StopSaveRealData failed.", error);
            if (!success)
                WriteLog("NET_DVR_StopSaveRealData failed: " + error);
        }

        private static string PrepareOutputPath(string filePath, string defaultExtension)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Output file path is required.", nameof(filePath));

            string fullPath = Path.GetFullPath(filePath);
            //string? directory = Path.GetDirectoryName(fullPath);
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            if (string.IsNullOrEmpty(Path.GetExtension(fullPath)))
                fullPath += defaultExtension;
            return fullPath;
        }

        private void EnsureInitialized()
        {
            if (!_sdkInitialized)
                throw new InvalidOperationException("Call Initialize before using the camera.");
        }

        private void EnsureLoggedIn()
        {
            EnsureInitialized();
            if (_userId < 0)
                throw new InvalidOperationException("Call Login before using the camera.");
        }

        private void EnsurePreviewing()
        {
            EnsureLoggedIn();
            if (_previewHandle < 0)
                throw new InvalidOperationException("Call StartPreview before recording.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(HikvisionCameraClient));
        }

        private void ThrowLastError(string operation)
        {
            throw new HikvisionException(operation + " failed.", HCNetSDK.NET_DVR_GetLastError());
        }

        private void OnSdkException(uint exceptionType, int userId, int handle, IntPtr userData)
        {
            WriteLog("HCNetSDK exception. type=" + exceptionType + ", userId=" + userId + ", handle=" + handle);
        }

        private void WriteLog(string message)
        {
            try
            {
                Log.Invoke(message);
            }
            catch
            {
                // A consumer's log handler must not crash the SDK callback thread.
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetDllDirectory(string lpPathName);
        }
    }

    public sealed class HikvisionException : Exception
    {
        public HikvisionException(string message, int errorCode)
            : base(message + " SDK error code: " + errorCode)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; }
    }

    internal static class HCNetSDK
    {
        private const string DllName = "HCNetSDK";
        private const int SerialNumberLength = 48;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ExceptionCallback(uint exceptionType, int userId, int handle, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RealDataCallback(int realHandle, uint dataType, IntPtr buffer, uint bufferSize, IntPtr userData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct UserLoginInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
            public string DeviceAddress;
            public byte UseTransport;
            public ushort Port;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string UserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Password;
            public IntPtr LoginResultCallback;
            public IntPtr UserData;
            public byte UseAsyncLogin;
            public byte ProxyType;
            public byte UseUtcTime;
            public byte LoginMode;
            public byte Https;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceInfoV30
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SerialNumberLength)]
            public byte[] SerialNumber;
            public byte AlarmInPortCount;
            public byte AlarmOutPortCount;
            public byte DiskCount;
            public byte DvrType;
            public byte ChannelCount;
            public byte StartChannel;
            public byte AudioChannelCount;
            public byte IpChannelCount;
            public byte ZeroChannelCount;
            public byte MainProtocol;
            public byte SubProtocol;
            public byte Support;
            public byte Support1;
            public byte Support2;
            public ushort DeviceType;
            public byte Support3;
            public byte MultiStreamProtocol;
            public byte StartDigitalChannel;
            public byte StartDigitalTalkChannel;
            public byte HighDigitalChannelCount;
            public byte Support4;
            public byte LanguageType;
            public byte VoiceInChannelCount;
            public byte StartVoiceInChannel;
            public byte Support5;
            public byte Support6;
            public byte MirrorChannelCount;
            public ushort StartMirrorChannel;
            public byte Support7;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceInfoV40
        {
            public DeviceInfoV30 DeviceInfo;
            public byte SupportLock;
            public byte RetryLoginTime;
            public byte PasswordLevel;
            public byte ProxyType;
            public uint SurplusLockTime;
            public byte CharacterEncodeType;
            public byte SupportDevice5;
            public byte LoginMode;
            public byte Reserved1;
            public uint OemCode;
            public uint SerialNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Reserved2;

            public static DeviceInfoV40 Create()
            {
                var value = new DeviceInfoV40
                {
                    DeviceInfo = new DeviceInfoV30
                    {
                        SerialNumber = new byte[SerialNumberLength],
                        Reserved = new byte[2]
                    },
                    Reserved2 = new byte[20]
                };
                return value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PreviewInfo
        {
            public int Channel;
            public uint StreamType;
            public uint LinkMode;
            public IntPtr PreviewWindow;
            public int Blocked;
            public uint DisplayBufferCount;
            public byte ProtocolType;
            public byte Reserved1;
            public byte Reserved2;
            public byte Reserved3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JpegParameter
        {
            public ushort PictureSize;
            public ushort PictureQuality;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_Init();

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_Cleanup();

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_SetConnectTime(uint waitTime, byte tryTimes);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_SetReconnect(uint interval, int enable);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int NET_DVR_Login_V40(ref UserLoginInfo loginInfo, ref DeviceInfoV40 deviceInfo);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_Logout(int userId);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_SetExceptionCallBack_V30(uint message, IntPtr windowHandle, ExceptionCallback callback, IntPtr userData);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int NET_DVR_RealPlay_V40(int userId, ref PreviewInfo previewInfo, RealDataCallback? callback, IntPtr userData);
        public static extern int NET_DVR_RealPlay_V40(int userId, ref PreviewInfo previewInfo, RealDataCallback callback, IntPtr userData);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_StopRealPlay(int previewHandle);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_CaptureJPEGPicture(int userId, int channel, ref JpegParameter jpegParameter, string fileName);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_SaveRealData(int previewHandle, string fileName);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool NET_DVR_StopSaveRealData(int previewHandle);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int NET_DVR_GetLastError();
    }
}
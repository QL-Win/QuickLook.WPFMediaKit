using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WPFMediaKit.DirectShow.MediaPlayers
{
    /// <summary>
    /// Codecs supported in the LAV Video configuration
    /// </summary>
    /// <remarks>
    /// Codecs not listed here cannot be turned off. You can request codecs to be added to this list, if you wish.
    /// </remarks>
    internal enum LAVVideoCodec
    {
        Codec_H264,
        Codec_VC1,
        Codec_MPEG1,
        Codec_MPEG2,
        Codec_MPEG4,
        Codec_MSMPEG4,
        Codec_VP8,
        Codec_WMV3,
        Codec_WMV12,
        Codec_MJPEG,
        Codec_Theora,
        Codec_FLV1,
        Codec_VP6,
        Codec_SVQ,
        Codec_H261,
        Codec_H263,
        Codec_Indeo,
        Codec_TSCC,
        Codec_Fraps,
        Codec_HuffYUV,
        Codec_QTRle,
        Codec_DV,
        Codec_Bink,
        Codec_Smacker,
        Codec_RV12,
        Codec_RV34,
        Codec_Lagarith,
        Codec_Cinepak,
        Codec_Camstudio,
        Codec_QPEG,
        Codec_ZLIB,
        Codec_QTRpza,
        Codec_PNG,
        Codec_MSRLE,
        Codec_ProRes,
        Codec_UtVideo,
        Codec_Dirac,
        Codec_DNxHD,
        Codec_MSVideo1,
        Codec_8BPS,
        Codec_LOCO,
        Codec_ZMBV,
        Codec_VCR1,
        Codec_Snow,
        Codec_FFV1,
        Codec_v210,
        Codec_JPEG2000,
        Codec_VMNC,
        Codec_FLIC,
        Codec_G2M,
        Codec_ICOD,
        Codec_THP,
        Codec_HEVC,
        Codec_VP9,
        Codec_TrueMotion,
        Codec_VP7,
        Codec_H264MVC,
    }

    /// <summary>
    /// Codecs with hardware acceleration
    /// </summary>
    internal enum LAVVideoHWCodec
    {
        HWCodec_H264 = 0,
        HWCodec_VC1 = 1,
        HWCodec_MPEG2 = 3,
        HWCodec_MPEG4 = 4,
        HWCodec_MPEG2DVD = 5,
        HWCodec_HEVC = 6,
        HWCodec_VP9 = 7,
        HWCodec_H264MVC = 8,
        HWCodec_AV1 = 9,
    }

    /// <summary>
    /// Flags for HW Resolution support
    /// </summary>
    [Flags]
    internal enum LAVHWResFlag
    {
        LAVHWResFlag_SD = 0x0001,
        LAVHWResFlag_HD = 0x0002,
        LAVHWResFlag_UHD = 0x0004,
    }

    /// <summary>
    /// Type of hardware accelerations
    /// </summary>
    internal enum LAVHWAccel
    {
        HWAccel_None = 0,
        HWAccel_CUDA = 1,
        HWAccel_QuickSync = 2,
        HWAccel_DXVA2 = 3,
        HWAccel_DXVA2CopyBack = HWAccel_DXVA2,
        HWAccel_DXVA2Native = 4,
        HWAccel_D3D11 = 5,
    }

    /// <summary>
    /// Deinterlace algorithms offered by the hardware decoders
    /// </summary>
    internal enum LAVHWDeintModes
    {
        HWDeintMode_Weave = 0,
        HWDeintMode_BOB = 1,
        HWDeintMode_Hardware = 2
    }

    /// <summary>
    /// Software deinterlacing algorithms
    /// </summary>
    internal enum LAVSWDeintModes
    {
        SWDeintMode_None = 0,
        SWDeintMode_YADIF = 1,
        SWDeintMode_W3FDIF_Simple = 2,
        SWDeintMode_W3FDIF_Complex = 3,
        SWDeintMode_BWDIF = 4,
    }

    /// <summary>
    /// Deinterlacing processing mode
    /// </summary>
    internal enum LAVDeintMode
    {
        DeintMode_Auto = 0,
        DeintMode_Aggressive = 1,
        DeintMode_Force = 2,
        DeintMode_Disable = 3,
    }

    /// <summary>
    /// Type of deinterlacing to perform
    /// </summary>
    /// <remarks>
    /// Note: Weave will always use FramePer2Field
    /// </remarks>
    internal enum LAVDeintOutput
    {
        /// <summary>
        /// FramePerField re-constructs one frame from every field, resulting in 50/60 fps.
        /// </summary>
        DeintOutput_FramePerField = 0,

        /// <summary>
        /// FramePer2Field re-constructs one frame from every 2 fields, resulting in 25/30 fps.
        /// </summary>
        DeintOutput_FramePer2Field = 1,
    }

    /// <summary>
    /// Control the field order of the deinterlacer
    /// </summary>
    internal enum LAVDeintFieldOrder
    {
        DeintFieldOrder_Auto = 0,
        DeintFieldOrder_TopFieldFirst = 1,
        DeintFieldOrder_BottomFieldFirst = 2,
    }

    /// <summary>
    /// Supported output pixel formats
    /// </summary>
    internal enum LAVOutPixFmts
    {
        LAVOutPixFmt_None = -1,

        /// <summary>
        /// 4:2:0, 8bit, planar
        /// </summary>
        LAVOutPixFmt_YV12,

        /// <summary>
        /// 4:2:0, 8bit, Y planar, U/V packed
        /// </summary>
        LAVOutPixFmt_NV12,

        /// <summary>
        /// 4:2:2, 8bit, packed
        /// </summary>
        LAVOutPixFmt_YUY2,

        /// <summary>
        /// 4:2:2, 8bit, packed
        /// </summary>
        LAVOutPixFmt_UYVY,

        /// <summary>
        /// 4:4:4, 8bit, packed
        /// </summary>
        LAVOutPixFmt_AYUV,

        /// <summary>
        /// 4:2:0, 10bit, Y planar, U/V packed
        /// </summary>
        LAVOutPixFmt_P010,

        /// <summary>
        /// 4:2:2, 10bit, Y planar, U/V packed
        /// </summary>
        LAVOutPixFmt_P210,

        /// <summary>
        /// 4:4:4, 10bit, packed
        /// </summary>
        LAVOutPixFmt_Y410,

        /// <summary>
        /// 4:2:0, 16bit, Y planar, U/V packed
        /// </summary>
        LAVOutPixFmt_P016,

        /// <summary>
        /// 4:2:2, 16bit, Y planar, U/V packed
        /// </summary>
        LAVOutPixFmt_P216,

        /// <summary>
        /// 4:4:4, 16bit, packed
        /// </summary>
        LAVOutPixFmt_Y416,

        /// <summary>
        /// 32-bit RGB (BGRA)
        /// </summary>
        LAVOutPixFmt_RGB32,

        /// <summary>
        /// 24-bit RGB (BGR)
        /// </summary>
        LAVOutPixFmt_RGB24,

        /// <summary>
        /// 4:2:2, 10bit, packed
        /// </summary>
        LAVOutPixFmt_v210,

        /// <summary>
        /// 4:4:4, 10bit, packed
        /// </summary>
        LAVOutPixFmt_v410,

        /// <summary>
        /// 4:2:2, 8-bit, planar
        /// </summary>
        LAVOutPixFmt_YV16,

        /// <summary>
        /// 4:4:4, 8-bit, planar
        /// </summary>
        LAVOutPixFmt_YV24,

        /// <summary>
        /// 48-bit RGB (16-bit per pixel, BGR)
        /// </summary>
        LAVOutPixFmt_RGB48,
    }

    /// <summary>
    /// Dithering mode
    /// </summary>
    internal enum LAVDitherMode
    {
        LAVDither_Ordered = 0,
        LAVDither_Random = 1,
    }

    /// <summary>
    /// LAV Video configuration interface
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("FA40D6E9-4D38-4761-ADD2-71A9EC5FD32F")]
    internal interface ILAVVideoSettings
    {
        [PreserveSig]
        int SetRuntimeConfig([MarshalAs(UnmanagedType.Bool)] bool bRuntimeConfig);

        [PreserveSig]
        bool GetFormatConfiguration(LAVVideoCodec vCodec);

        [PreserveSig]
        int SetFormatConfiguration(LAVVideoCodec vCodec, [MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        int SetNumThreads(uint dwNum);

        [PreserveSig]
        uint GetNumThreads();

        [PreserveSig]
        int SetStreamAR(uint bStreamAR);

        [PreserveSig]
        uint GetStreamAR();

        [PreserveSig]
        bool GetPixelFormat(LAVOutPixFmts pixFmt);

        [PreserveSig]
        int SetPixelFormat(LAVOutPixFmts pixFmt, [MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        int SetRGBOutputRange(uint dwRange);

        [PreserveSig]
        uint GetRGBOutputRange();

        [PreserveSig]
        int SetDeintFieldOrder(LAVDeintFieldOrder fieldOrder);

        [PreserveSig]
        LAVDeintFieldOrder GetDeintFieldOrder();

        [Obsolete("Use SetDeinterlacingMode", false)]
        [PreserveSig]
        int SetDeintAggressive([MarshalAs(UnmanagedType.Bool)] bool bAggressive);

        [Obsolete("Use GetDeinterlacingMode", false)]
        [PreserveSig]
        bool GetDeintAggressive();

        [Obsolete("Use SetDeinterlacingMode", false)]
        [PreserveSig]
        int SetDeintForce([MarshalAs(UnmanagedType.Bool)] bool bForce);

        [Obsolete("Use GetDeinterlacingMode", false)]
        [PreserveSig]
        bool GetDeintForce();

        [PreserveSig]
        uint CheckHWAccelSupport(LAVHWAccel hwAccel);

        [PreserveSig]
        int SetHWAccel(LAVHWAccel hwAccel);

        [PreserveSig]
        LAVHWAccel GetHWAccel();

        [PreserveSig]
        int SetHWAccelCodec(LAVVideoHWCodec hwAccelCodec, [MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetHWAccelCodec(LAVVideoHWCodec hwAccelCodec);

        [PreserveSig]
        int SetHWAccelDeintMode(LAVHWDeintModes deintMode);

        [PreserveSig]
        LAVHWDeintModes GetHWAccelDeintMode();

        [PreserveSig]
        int SetHWAccelDeintOutput(LAVDeintOutput deintOutput);

        [PreserveSig]
        LAVDeintOutput GetHWAccelDeintOutput();

        [Obsolete("HQ deint is always used when available depending on platform and codec", false)]
        [PreserveSig]
        int SetHWAccelDeintHQ([MarshalAs(UnmanagedType.Bool)] bool bHQ);

        [Obsolete("HQ deint is always used when available depending on platform and codec", false)]
        [PreserveSig]
        bool GetHWAccelDeintHQ();

        [PreserveSig]
        int SetSWDeintMode(LAVSWDeintModes deintMode);

        [PreserveSig]
        LAVSWDeintModes GetSWDeintMode();

        [PreserveSig]
        int SetSWDeintOutput(LAVDeintOutput deintOutput);

        [PreserveSig]
        LAVDeintOutput GetSWDeintOutput();

        [Obsolete("Use SetDeinterlacingMode", false)]
        [PreserveSig]
        int SetDeintTreatAsProgressive([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [Obsolete("Use GetDeinterlacingMode", false)]
        [PreserveSig]
        bool GetDeintTreatAsProgressive();

        [PreserveSig]
        int SetDitherMode(LAVDitherMode ditherMode);

        [PreserveSig]
        LAVDitherMode GetDitherMode();

        [PreserveSig]
        int SetUseMSWMV9Decoder([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetUseMSWMV9Decoder();

        [PreserveSig]
        int SetDVDVideoSupport([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetDVDVideoSupport();

        [PreserveSig]
        int SetHWAccelResolutionFlags([In] LAVHWResFlag dwFlags);

        [PreserveSig]
        LAVHWResFlag GetHWAccelResolutionFlags();

        [PreserveSig]
        int SetTrayIcon([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetTrayIcon();

        [PreserveSig]
        int SetDeinterlacingMode(LAVDeintMode deintMode);

        [PreserveSig]
        LAVDeintMode GetDeinterlacingMode();

        [PreserveSig]
        int SetGPUDeviceIndex(uint dwDevice);

        [PreserveSig]
        int GetHWAccelNumDevices(LAVHWAccel hwAccel);

        [PreserveSig]
        int GetHWAccelDeviceInfo(LAVHWAccel hwAccel, uint dwIndex, [Out, MarshalAs(UnmanagedType.BStr)] out string pstrDeviceName, [Out] out uint pdwDeviceIdentifier);

        [PreserveSig]
        int GetHWAccelDeviceIndex(LAVHWAccel hwAccel, [Out] out uint pdwDeviceIdentifier);

        [PreserveSig]
        int SetHWAccelDeviceIndex(LAVHWAccel hwAccel, uint dwIndex, int dwDeviceIdentifier);

        [PreserveSig]
        int SetH264MVCDecodingOverride([In, MarshalAs(UnmanagedType.Bool)] bool bEnabled);
    }
}
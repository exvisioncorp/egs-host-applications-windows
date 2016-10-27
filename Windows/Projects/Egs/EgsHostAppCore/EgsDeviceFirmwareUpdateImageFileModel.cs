namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Diagnostics;

    internal class EgsDeviceFirmwareUpdateImageFileModel
    {
        public byte ImageFileFormatRevision { get; private set; }
        public string AsciiDescription { get; private set; }
        public FirmwareUpdateProtocolRevisionKinds ProtocolRevision { get; private set; }
        public string DebugMessageAboutProtocolRevision { get; private set; }
        public Version FirmwareVersion { get; private set; }
        public byte PayloadHeaderFormatRevision { get; private set; }
        /// <summary>
        /// 3: App1
        /// </summary>
        public byte ImageType { get; private set; }
        /// <summary>
        /// ChipSet (0=MA2100, 1=MA2150), added from payloadHeaderFormatRevision=0x01 ??
        /// </summary>
        public byte ChipSet { get; private set; }
        public Egs.PropertyTypes.HardwareTypeDetail HardwareType { get; private set; }
        public byte[] LoadedImageAsByteArray { get; private set; }
        public uint PayloadLength { get; private set; }
        public uint CrcWord { get; private set; }
        public uint PayloadOffset { get; private set; }

        public EgsDeviceFirmwareUpdateImageFileModel(string filePath)
        {
            using (var fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int readSize;
                int remain = (int)fstream.Length;
                int bufPos = 0;
                LoadedImageAsByteArray = new byte[fstream.Length];

                //-- read file information (first 256 bytes) --
                readSize = fstream.Read(LoadedImageAsByteArray, bufPos, Math.Min(256, remain));
                bufPos += readSize;
                remain -= readSize;

                // NOTE: It is unnecessary to check the ascii description
                AsciiDescription = Encoding.ASCII.GetString(LoadedImageAsByteArray, 1, 126);
                Debug.WriteLine("AsciiDescription:" + AsciiDescription);


                var firmwareUpdateProtocolRevisionInImageFileAsInt = BitConverter.ToInt32(LoadedImageAsByteArray, 128);
                ProtocolRevision = EgsDeviceFirmwareUpdateModel.ConvertFromIntToFirmwareUpdateProtocolRevisionKinds(firmwareUpdateProtocolRevisionInImageFileAsInt);
                switch (ProtocolRevision)
                {
                    case FirmwareUpdateProtocolRevisionKinds.KickStarterFirstRelease:
                        DebugMessageAboutProtocolRevision = "This image file is KickStareter version (Rev. 11448).  It is used only for DFU test in Exvision." + Environment.NewLine;
                        break;
                    case FirmwareUpdateProtocolRevisionKinds.FileUpdater:
                        DebugMessageAboutProtocolRevision = "This image file is 'File Updater' to update a next firmware image file safely." + Environment.NewLine;
                        break;
                    case FirmwareUpdateProtocolRevisionKinds.Latest:
                        DebugMessageAboutProtocolRevision = "This image file uses the latest protocol." + Environment.NewLine;
                        break;
                    case FirmwareUpdateProtocolRevisionKinds.Unknown:
                        DebugMessageAboutProtocolRevision = "This image file does not have information about 'Firmware Update Protocol Revision'.  Only for used DFU test in Exvision." + Environment.NewLine;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }


                var firmwareVersionInImageFileAsIntArray = new int[4];
                for (int i = 0; i < 4; i++) { firmwareVersionInImageFileAsIntArray[i] = BitConverter.ToInt32(LoadedImageAsByteArray, 132 + i * 4); }
                FirmwareVersion = new Version(firmwareVersionInImageFileAsIntArray[0], firmwareVersionInImageFileAsIntArray[1], firmwareVersionInImageFileAsIntArray[2], firmwareVersionInImageFileAsIntArray[3]);


                //-- read payload header (second 256 bytes) --
                readSize = fstream.Read(LoadedImageAsByteArray, bufPos, Math.Min(256, remain));
                bufPos += readSize;
                remain -= readSize;

                PayloadLength = BitConverter.ToUInt32(LoadedImageAsByteArray, 260);
                //PayloadLength = (uint)LoadedImageAsByteArray[260] | (((uint)LoadedImageAsByteArray[261]) << 8) | (((uint)LoadedImageAsByteArray[262]) << 16) | (((uint)LoadedImageAsByteArray[263]) << 24);

                CrcWord = BitConverter.ToUInt32(LoadedImageAsByteArray, 264);
                //CrcWord = (uint)LoadedImageAsByteArray[264] | (((uint)LoadedImageAsByteArray[265]) << 8) | (((uint)LoadedImageAsByteArray[266]) << 16) | (((uint)LoadedImageAsByteArray[267]) << 24);

                PayloadOffset = 268;

                if (false)
                {
                    for (int i = 0; i < 512; i++) { Debug.WriteLine(i.ToString() + " " + LoadedImageAsByteArray[i].ToString()); }
                }

                while (remain > 0)
                {
                    readSize = fstream.Read(LoadedImageAsByteArray, bufPos, Math.Min(256, remain));

                    bufPos += readSize;
                    remain -= readSize;
                }

                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Read binary file: {0} bytes", LoadedImageAsByteArray.Length));
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Payload length= 0x{0:X}", PayloadLength));
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "CRC word= 0x{0:X}", CrcWord));
            }
        }
    }
}

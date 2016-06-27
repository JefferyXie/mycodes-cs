using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using log4net;
using System.Reflection;

namespace MyPlainAPI.Utils
{
    public static class StreamUtils
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static byte[] ReadAllBytes(Stream s, int ccbToRead)
        {
            byte[] buf = new byte[ccbToRead];
            int ccb = ReadBytes(s, buf, 0, ccbToRead);
            if (ccb != ccbToRead)
                throw new ApplicationException(string.Format("Could not read {0} bytes", ccbToRead));
            return buf;
        }

        /// <summary>
        /// Reads as many bytes as it can up to ccbToRead, making repeated read calls if necessary.
        /// </summary>
        public static int ReadBytes(Stream s, byte[] buf, int offset, int ccbToRead)
        {
            int ccbRead = 0;
            while (ccbToRead > 0)
            {
                int ccb = s.Read(buf, ccbRead + offset, ccbToRead);
                if (ccb == 0)
                    break;
                ccbToRead -= ccb;
                ccbRead += ccb;
            }
            return ccbRead;
        }

        public static int ReadInt32(Stream s)
        {
            int i;
            if (!TryReadInt32(s, out i))
                throw new ApplicationException(string.Format("Could not read an Int32"));
            return i;
        }

        public static string ReadAsciiString(Stream s, int ccb)
        {
            return ASCIIEncoding.ASCII.GetString(ReadAllBytes(s, ccb));
        }
        public static string ReadAllString(Stream s)
        {
            using (var sr = new StreamReader(s, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        public static ushort ReadUInt16(Stream s)
        {
            return BitConverter.ToUInt16(ReadAllBytes(s, 2), 0);
        }

        public static uint ReadUInt32(Stream s)
        {
            return BitConverter.ToUInt32(ReadAllBytes(s, 4), 0);
        }

        public static bool TryReadInt32(Stream s, out int i)
        {
            byte[] buf = new byte[4];
            int ccb = ReadBytes(s, buf, 0, 4);
            if (ccb == 0)
            {
                i = 0;
                return false;
            }
            else if (ccb != 4)
            {
                throw new ApplicationException(string.Format("Tried to read 4 bytes but only read {0}, stream is in unexpected position", ccb));
            }

            i = BitConverter.ToInt32(buf, 0);
            return true;
        }

        public static void WriteBytes(Stream s, byte[] buf)
        {
            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        ///  Read Text file from appData
        /// </summary>
        /// <param name="path">file Path</param>
        /// <param name="filename">input file name</param>
        /// <returns>text</returns>
        /*public static string ReadTextFromFile(string path, string filename)
        {
            try
            {
                string filePath = CacheLoadFileExtension.MapAppDataFile(Path.Combine(path, filename));

                {
                    if (File.Exists(filePath))
                    {
                        return File.ReadAllText(filePath, Encoding.UTF8);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                //Failed to map the path
                log.Error(ex);
            }
            catch (IOException ex)
            {
                log.Error(ex);
            }
            catch (SecurityException ex)
            {
                log.Error(ex);
            }
            catch (System.Web.HttpException ex)
            {
                log.Error(ex);
            }

            return null;
        }*/
    }
}

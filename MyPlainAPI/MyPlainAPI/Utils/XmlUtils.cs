using System;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using log4net;
using MyPlainAPI.Services;

namespace MyPlainAPI.Utils
{
    public static class XmlUtils
    {
        private static readonly Logger log = Global.Log;

        /// <summary>
        /// for internal xml reader settings
        /// </summary>
        /// <returns></returns>
        internal static XmlReaderSettings GetXmlReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                CheckCharacters = false,
                CloseInput = true,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            settings.ValidationEventHandler += ((sender, e) =>
            {
                //e.Severity = System.Xml.Schema.XmlSeverityType.Error;
            });

            return settings;
        }

        /// <summary>
        /// Load XPathNavigator From String Reader
        /// </summary>
        /// <param name="stream">Input Text Reader</param>
        /// <returns>XPathNavigator</returns>
        internal static XPathNavigator LoadFromReader(XmlReader text)
        {
            using (XmlReader reader = XmlReader.Create(text, GetXmlReaderSettings()))
            {
                try
                {
                    return new XPathDocument(reader).CreateNavigator();
                }
                catch (System.Xml.XmlException ex)
                {
                    log.Error(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Load XPathNavigator From Stream
        /// </summary>
        /// <param name="stream">Input Stream</param>
        /// <returns>XPathNavigator</returns>
        internal static XPathNavigator LoadFromStream(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream, GetXmlReaderSettings()))
            {
                try
                {
                    return new XPathDocument(reader).CreateNavigator();
                }
                catch (System.Xml.XmlException ex)
                {
                    log.Error(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Load xml reader from xml file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        /*internal static XmlReader LoadFromFile(string path, string filename)
        {
            try
            {
                string filePath = CacheLoadFileExtension.MapAppDataFile(Path.Combine(path, filename));

                {
                    if (File.Exists(filePath))
                    {
                        return XmlReader.Create(File.OpenRead(filePath), GetXmlReaderSettings());
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
        }

        /// <summary>
        /// Load XPathNavigator From String
        /// </summary>
        /// <param name="xml">input xml string</param>
        /// <returns>XPathNavigator</returns>
        internal static XPathNavigator LoadFromString(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                using (XmlReader reader = XmlTextReader.Create(sr, GetXmlReaderSettings()))
                {
                    try
                    {
                        return new XPathDocument(reader).CreateNavigator();
                    }
                    catch (System.Xml.XmlException)
                    {
                        log.ErrorFormat("Invalid Xml: {0}", xml);
                        return null;
                    }
                }
            }
        }*/

        /// <summary>
        /// Set node value
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        internal static void SetNodeValue(XPathNavigator nav, string path, string value)
        {
            XPathNavigator node = nav.SelectSingleNode(path);

            if (node != null)
            {
                node.SetValue(value);
            }
        }

        internal static void InsertUpdateAttibuteValue(XPathNavigator nav, string attributePath, string value)
        {
            if (string.IsNullOrEmpty(attributePath) || !attributePath.StartsWith("@"))
            {
                throw new NotSupportedException("Incorrect Attribute Path" + attributePath);
            }
            XPathNavigator node = nav.SelectSingleNode(attributePath);

            if (node != null)
            {
                node.SetValue(value);
            }
            else
            {
                if (value != null)
                {
                    string attrName = attributePath.Remove(0, 1);
                    nav.CreateAttribute(null, attrName, null, value);
                }
            }
        }

        /// <summary>
        /// Check whether contains the path node.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        internal static bool IsExists(XPathNavigator nav, string path)
        {
            XPathNavigator node = nav.SelectSingleNode(path);

            return node != null;
        }


        /// <summary>
        /// Get Node value with generic type
        /// </summary>
        /// <typeparam name="T">return object type</typeparam>
        /// <param name="nav">input xml node</param>
        /// <param name="path">target xpath</param>
        /// <param name="defaultValue">defaule value if xpath not exists</param>
        /// <returns>object value</returns>
        internal static T GetNodeValue<T>(XPathNavigator nav, string path, T defaultValue)
        {
            XPathNavigator node = nav == null ? null : nav.SelectSingleNode(path);
            try
            {
                if (node == null)
                {
                    return defaultValue;
                }
                else if (typeof(T) == typeof(Guid))
                {
                    return (T)Convert.ChangeType(new Guid(node.Value), typeof(T));
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    if (string.IsNullOrEmpty(node.Value))
                    {
                        return defaultValue;
                    }
                    else
                    {
                        return (T)Convert.ChangeType(DateTime.Parse(node.Value), typeof(T));
                    }
                }
                else
                {
                    return (T)Convert.ChangeType(node.Value, typeof(T));
                }
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            catch (OverflowException)
            {
                return defaultValue;
            }
            catch (ArgumentNullException)
            {
                return defaultValue;
            }

            //return default(T);
        }

        /// <summary>
        /// Get node Value for string
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static string GetNodeValue(XPathNavigator nav, string path, string defaultValue = null)
        {
            XPathNavigator node = nav == null ? null : nav.SelectSingleNode(path);
            return (node == null) ? defaultValue : node.Value;
        }

        internal static XPathNodeIterator GetNodeList(XPathNavigator nav, string path)
        {
            XPathNodeIterator nodes = nav == null ? null : nav.Select(path);
            return nodes;
        }

        /// <summary>
        /// Encode String
        /// </summary>
        /// <param name="text">encode xml string</param>
        /// <returns>xml</returns>
        internal static string EncodeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {
                return EncodeXmlString(StringUtils.Trim(text), true, true, true);
            }
        }

        /// <summary>
        /// Encode xml string for &, >, <, ", '
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bLtGt"></param>
        /// <param name="bQuote"></param>
        /// <param name="bApos"></param>
        /// <returns>string</returns>
        public static string EncodeXmlString(string text, bool bLtGt, bool bQuote, bool bApos)
        {
            StringBuilder enCodeText = new StringBuilder(text);
            //encode
            enCodeText = enCodeText.Replace("&", "&amp;");
            if (bApos)
            {
                enCodeText = enCodeText.Replace("'", "&apos;");
            }
            if (bQuote)
            {
                enCodeText = enCodeText.Replace("\"", "&quot;");
            }
            if (bLtGt)
            {
                enCodeText = enCodeText.Replace("<", "&lt;").Replace(">", "&gt;");
            }

            return enCodeText.ToString();
        }

        /// <summary>
        /// Load XPathNavigator From String
        /// </summary>
        /// <param name="stream">Input String</param>
        /// <returns>XmlDocument</returns>
        internal static XmlDocument LoadXmlDocFromString(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                using (XmlReader reader = XmlTextReader.Create(sr, GetXmlReaderSettings()))
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);
                        return xmlDoc;
                    }
                    catch (System.Xml.XmlException)
                    {
                        log.Error("Invalid Xml: {0}", xml);
                    }
                }
            }
            return null;
        }
    }
}

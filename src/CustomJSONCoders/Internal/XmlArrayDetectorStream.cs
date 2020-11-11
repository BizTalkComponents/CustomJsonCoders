using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Streaming;
using System.IO;
using System.Xml;
namespace BizTalkComponents.PipelineComponents.CustomJsonCoders.Internal
{
    internal class XmlArrayDetectorStream : XmlTranslatorStream
    {
        internal bool HasRootNodeChildren { get; private set; }
        public bool IsFirstLevelArray
        {
            get
            {
                return m_Is1stNodeArray;
            }
        }       
        public string ArrayNodeName
        {
            get
            { return IsFirstLevelArray ? m_arrayNodeName : ""; }
        }
        public XmlArrayDetectorStream(Stream input)
            : base(new XmlTextReader(input))
        {
        }
        private bool m_Is1stNodeArray;
        private string m_arrayNodeName;
        protected override void TranslateEndElement(bool full)
        {
            if (m_reader.Depth == 1)
            {
                HasRootNodeChildren = true;
                if (string.IsNullOrEmpty(m_arrayNodeName))
                {
                    m_arrayNodeName = m_reader.LocalName;
                    m_Is1stNodeArray = true;
                }
                if (m_Is1stNodeArray)
                    m_Is1stNodeArray &= (m_reader.LocalName == m_arrayNodeName);
            }
            base.TranslateEndElement(full);
        }

        protected override void TranslateXmlDeclaration(string target, string val)
        {
            m_writer.WriteProcessingInstruction(target, val);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using System.ComponentModel;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.XLANGs.BaseTypes;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.BizTalk.Streaming;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using BizTalkComponents.PipelineComponents.CustomJsonCoders.Internal;

namespace BizTalkComponents.PipelineComponents.CustomJsonCoders
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    [System.Runtime.InteropServices.Guid("9d0e4103-4cce-4536-83fa-4a5040674ad6")]
    public partial class JSONEncoder : IBaseComponent, IComponent, IComponentUI, IPersistPropertyBag
    {
        [RequiredRuntime]
        [DisplayName("Remove Outer Envelope")]
        [Description("Removes the outer envelope")]
        public bool RemoveOuterEnvelope { get; set; }

        [DisplayName("Array Output")]
        [Description("if true, it will make the output as JSON array if it is applicable.")]
        public bool ArrayOutput { get; set; }


        private const int buffLength = 1024;


        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if (Disabled)
            {
                return pInMsg;
            }

            string errorMessage;
            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }
            var originalStream = pInMsg.BodyPart.GetOriginalDataStream();
            var data = new XmlArrayDetectorStream(originalStream);
            pInMsg.BodyPart.Data = data;
            var jencoder = new Microsoft.BizTalk.Component.JsonEncoder
            {
                RemoveOuterEnvelope = this.RemoveOuterEnvelope | ArrayOutput
            };
            jencoder.Execute(pContext, pInMsg);
            if (ArrayOutput & (data.IsFirstLevelArray | !data.HasRootNodeChildren))
            {
                var stream = pInMsg.BodyPart.GetOriginalDataStream();
                StreamReader reader = new StreamReader(stream, true);
                var ms = JSONModifier.ConvertToJArray(new Newtonsoft.Json.JsonTextReader(reader));
                pInMsg.BodyPart.Data = ms;
                pContext.ResourceTracker.AddResource(ms);
            }
            return pInMsg;
        }
    }
}
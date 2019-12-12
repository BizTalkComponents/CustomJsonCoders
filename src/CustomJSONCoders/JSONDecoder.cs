using System;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using System.ComponentModel;
using BizTalkComponents.PipelineComponents.CustomJsonCoders.Internal;

namespace BizTalkComponents.PipelineComponents.CustomJsonCoders
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [System.Runtime.InteropServices.Guid("9d0e4103-4cce-4536-83fa-4a5040674ad6")]
    public partial class JSONDecoder : IBaseComponent, IComponent, IComponentUI, IPersistPropertyBag
    {
        [RequiredRuntime]
        [DisplayName("Root Node")]
        [Description("Specify the root node name to be used in the generated xml")]
        public string RootNode{ get; set; }

        [DisplayName("RootNode Namespace")]
        [Description("Specify the namespace to be used in the generated xml")]
        public string RootNodeNamespace { get; set; }

        [DisplayName("Array Node Name")]
        [Description("the root child node  name to be used in the generated xml that represents the JSON array, the default name is data.")]
        public string ArrayNodeName { get; set; }

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
            var data = new EncapsulatorStream(originalStream, ArrayNodeName);           
            pInMsg.BodyPart.Data = data;
            var jdecoder = new Microsoft.BizTalk.Component.JsonDecoder
            {
                RootNode = this.RootNode,
                RootNodeNamespace = this.RootNodeNamespace,
                AddMessageBodyForEmptyMessage = true,
            };            
            jdecoder.Execute(pContext, pInMsg);
            return pInMsg;
        }
    }
}
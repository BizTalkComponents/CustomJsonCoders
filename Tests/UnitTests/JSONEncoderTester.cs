using System;
using BizTalkComponents.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using Microsoft.BizTalk.Component;
using System.Net.Mail;
using BizTalkComponents.PipelineComponents.CustomJsonCoders;
using System.Text;
using System.IO;
namespace BizTalkComponents.PipelineComponents.CustomJsonCoders.Tests.UnitTests
{
    [TestClass]
    public class JSONEncoderTester
    {
        [TestMethod]
        public void TestArrayOfEventsJSONEncoder()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            var component = new JSONEncoder {
                ArrayOutput = true,
                RemoveOuterEnvelope = true
            };            
            pipeline.AddComponent(component, PipelineStage.Encode);
            var message = MessageHelper.CreateFromString(Properties.Resources.ArrayOfEvents);
            var output = pipeline.Execute(message);
            var retStr = MessageHelper.ReadString(output);
            var JsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(retStr);
            Assert.IsTrue(JsonObj is Newtonsoft.Json.Linq.JArray);
        }

        [TestMethod]
        public void TestDifferentEventsJSONEncoder()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            var component = new JSONEncoder
            {
                ArrayOutput = true,
                RemoveOuterEnvelope = true
            };
            pipeline.AddComponent(component, PipelineStage.Encode);
            var message = MessageHelper.CreateFromString(Properties.Resources.DifferentEvents);
            var output = pipeline.Execute(message);
            var stream = new StreamReader(output.BodyPart.GetOriginalDataStream(), Encoding.UTF8);
            var retStr = stream.ReadToEnd();
            var JsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(retStr);
            Assert.IsFalse(JsonObj is Newtonsoft.Json.Linq.JArray);
        }
        [TestMethod]
        public void TestEmptyRootNodeJSONEncoder()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            var component = new JSONEncoder
            {
                ArrayOutput = true,
                RemoveOuterEnvelope = true
            };
            pipeline.AddComponent(component, PipelineStage.Encode);
            var message = MessageHelper.CreateFromString(Properties.Resources.EmptyEvents);
            var output = pipeline.Execute(message);
            var stream = new StreamReader(output.BodyPart.GetOriginalDataStream(), Encoding.UTF8);
            var retStr = stream.ReadToEnd();
            var JsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(retStr);
            Assert.IsFalse(JsonObj is Newtonsoft.Json.Linq.JArray);
        }
    }
}

using System;
using BizTalkComponents.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using Microsoft.BizTalk.Component;
using System.Net.Mail;
using BizTalkComponents.PipelineComponents.CustomJsonCoders;
using System.Text;
using System.IO;
using System.Xml;

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
            var message = MessageHelper.CreateFromStream(TestHelper.GetTestStream("ArrayOfEvents.xml"));
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
            var message = MessageHelper.CreateFromStream(TestHelper.GetTestStream("DifferentEvents.xml"));
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
            var message = MessageHelper.CreateFromStream(TestHelper.GetTestStream("EmptyEvents.xml"));
            var output = pipeline.Execute(message);
            var stream = new StreamReader(output.BodyPart.GetOriginalDataStream(), Encoding.UTF8);
            var retStr = stream.ReadToEnd();
            var JsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(retStr);
            Assert.IsTrue(JsonObj is Newtonsoft.Json.Linq.JArray);
        }

        [TestMethod]
        public void TestLargeFileJSONEncoder()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            var component = new JSONEncoder
            {
                ArrayOutput = true,
                RemoveOuterEnvelope = true
            };
            pipeline.AddComponent(component, PipelineStage.Encode);
            var doc = new XmlDocument();
            var stream = TestHelper.GetTestStream("large.xml");
            doc.Load(stream);            
            stream.Seek(0, SeekOrigin.Begin);
            var message = MessageHelper.CreateFromStream(stream);
            var output = pipeline.Execute(message);
            var retStr = MessageHelper.ReadString(output);
            var JsonObj =Newtonsoft.Json.JsonConvert.DeserializeObject(retStr);
            Assert.IsTrue(JsonObj is Newtonsoft.Json.Linq.JArray);
            Assert.AreEqual(doc.DocumentElement.ChildNodes.Count, (JsonObj as Newtonsoft.Json.Linq.JArray).Count);
        }
    }
}

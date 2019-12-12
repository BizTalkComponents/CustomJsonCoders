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
    public class JSONDecoderTester
    {
        [TestMethod]
        public void TestJsonArrayJSONDecoder()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new JSONDecoder {
                ArrayNodeName="Event",
                RootNode="Events",
                RootNodeNamespace="Http://JsonDecoderTester/TestEvents"
            };            
            pipeline.AddComponent(component, PipelineStage.Decode);            
            var message = MessageHelper.CreateFromStream(TestHelper.GetTestStream("large.json"));
            var output = pipeline.Execute(message);
            var retstr = MessageHelper.ReadString(output[0]);
            var doc = new XmlDocument();
            doc.LoadXml(retstr);
            var arr = doc.SelectNodes("/*[local-name()='Events']/*[local-name()='Event']");
            Assert.AreEqual(arr.Count,doc.DocumentElement.ChildNodes.Count);            
        }
    }
}

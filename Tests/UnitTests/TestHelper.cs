using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.PipelineComponents.CustomJsonCoders.Tests.UnitTests
{
    public class TestHelper
    {
        public static Stream GetTestStream(string filename)
        {
            var fullname=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", filename);
            return File.OpenRead(fullname);
        }
    }
}

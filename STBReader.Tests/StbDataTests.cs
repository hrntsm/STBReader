using Microsoft.VisualStudio.TestTools.UnitTesting;
using STBReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STBReader.Tests
{
    [TestClass()]
    public class StbDataTests
    {
        [TestMethod]
        public void StbDataTest()
        {
            var paths = new List<string>
            {
                "..\\..\\..\\STBDataSamples\\ver1\\RC1.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\S1.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\S2.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model1.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model2.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model3.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model4.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model5.stb",
                "..\\..\\..\\STBDataSamples\\ver1\\Model6.stb",
                
                "..\\..\\..\\STBDataSamples\\ver2\\RC1.stb",
                "..\\..\\..\\STBDataSamples\\ver2\\RC2.stb",
                "..\\..\\..\\STBDataSamples\\ver2\\RC3.stb",
                "..\\..\\..\\STBDataSamples\\ver2\\S1.stb",
                "..\\..\\..\\STBDataSamples\\ver2\\S2.stb",
                "..\\..\\..\\STBDataSamples\\ver2\\S3.stb",
            };
            foreach (string path in paths)
            {
                var stbData = new StbData(path, 0.00, 0.00);
            }
        }
    }
}
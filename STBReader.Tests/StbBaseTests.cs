using Microsoft.VisualStudio.TestTools.UnitTesting;
using STBReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STBReader.Tests
{
    [TestClass]
    public class StbBaseTests
    {
        [TestMethod]
        public void Point3Test()
        {
            var point3 = new Point3(3d, 3d, 3d);
            int hashCode = point3.GetHashCode();
            Assert.IsTrue(point3.Equals(new Point3(3, 3, 3)));
            Assert.IsFalse(point3.Equals(3d));
            Assert.AreEqual(3, point3.X);
            Assert.AreEqual(3, point3.Y);
            Assert.AreEqual(3, point3.Z);
        }
    }
}
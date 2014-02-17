using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Artis.DataLoader.Test
{
    [TestFixture]
    public class Test
    {
        private DataLoader _dataLoader;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _dataLoader=new DataLoader();
        }

        [Test]
        public void CanDownloadActions()
        {
            //я пока не понимаю как это тестирвоать
        }
    }
}

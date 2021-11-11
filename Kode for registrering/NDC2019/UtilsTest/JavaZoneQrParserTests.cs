using System;
using System.Data;
using VismaKart.UtilsExtra;
using Xunit;

namespace UtilsTest
{
    public class JavaZoneQrParserTests
    {
        [Fact]
        public void QrParser_MeCard_Works()
        {
            // Arrange
            var qrcode = "MECARD:N:Vebjørn R Olsen;EMAIL:vro@macsimum.no;ORG:Macsimum;UID:UK3xJswpF+oUNj+d9HE10w==;;";

            // Act
            var data = JavaZoneQrParser.ParseQrCode(qrcode);

            // Assert
            Assert.Equal("Vebjørn R Olsen", data.Name);
            Assert.Equal("vro@macsimum.no", data.Mail);
            Assert.Equal("Macsimum", data.Company);
        }

        [Fact]
        public void QrParser_VCard_Works()
        {
            // Arrange
            var qrcode = "BEGIN:VCARD VERSION:2.1 N:Kværna;Halvard FN:Halvard Kværna TEL;CELL:+4741437947 EMAIL:halvard.hella.kvaerna@visma.com BMSTID:57334741 END:VCARD";
            // Act
            var data = JavaZoneQrParser.ParseQrCode(qrcode);

            // Assert
            Assert.Equal("Halvard Kværna", data.Name);
            Assert.Equal("halvard.hella.kvaerna@visma.com", data.Mail);
            Assert.Equal("+4741437947", data.Phone);
        }

        [Fact]
        public void QrParser_VCard_Works2()
        {
            // Arrange
            var qrcode = "BEGIN:VCARD VERSION:2.1 N:Rognskog;Helene FN:Helene Rognskog TEL;CELL:93882536 EMAIL:helene.rognskog@visma.com BMSTID:57334784 END:VCARD";
            
            // Act
            var data = JavaZoneQrParser.ParseQrCode(qrcode);

            // Assert
            Assert.Equal("Helene Rognskog", data.Name);
            Assert.Equal("helene.rognskog@visma.com", data.Mail);
            Assert.Equal("93882536", data.Phone);
        }
    }
}

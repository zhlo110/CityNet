using Aspose.Words;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class AsposeLicense
    {
        public static string wordlic =
            "<?xml version=\"1.0\"?>" +
            "<License>" +
            "<Data>" +
            "<LicensedTo>Sichuan TuoHui Technology Co.,LTD</LicensedTo>"+
            "<EmailTo>305024828@qq.com</EmailTo>"+
            "<LicenseType>Developer OEM</LicenseType>" +
            "<LicenseNote>One Developer And Unlimited Deployment Locations</LicenseNote>" +
            "<OrderID>201217045540</OrderID>" +
            "<UserID>864834</UserID>" +
            "<OEM>This is a redistributable license</OEM>"+
            "<Products>" +
            "<Product>Aspose.Words for .NET</Product>"+
            "</Products>" +
            "<EditionType>Professional</EditionType>" +
            "<SerialNumber>ad0e597d-ece0-45ee-91b6-163205941e55</SerialNumber>"+
            "<SubscriptionExpiry>20211217</SubscriptionExpiry>"+
            "<LicenseVersion>3.0</LicenseVersion>"+
            "<LicenseInstructions>https://purchase.aspose.com/policies/use-license</LicenseInstructions>"+
            "</Data>"+
            "<Signature>KO9stKHzlnxzks4CuhLXLQ52rHZ/5Riqcu4mfao7x7K6kyvOUbVVhjTfK9lZ5nwEmsxrqHdYM/YBStCaxjoGwgmcpdSHx8mwAHOmbas7qDY+Q8c4ZDrSx/L/S9iJaOxGjQeYnYcDHFJjRrnwhkuUljp1dKBtysxUrJTjKzgNnoObQCV98unZE5+wVLY0YaMcSVOnH1LZvfkHAk1Dz4K1JAmA3b6BugK+62D/UPOgiLAKpGWLUE+T1Ep2uTaUsTOq7QJvaiQxnPN8fQMya+t7F+a97Zlug+asqIDP1Y2UEXoBdlOvMdp3k6sqN+QaHojPOAt5jpXWHEQdsj/vUwiFTg==</Signature>"+
            "</License>";

        public static string pdflic = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjxMaWNlbnNlPg0KICAgIDxEYXRhPg0KICAgICAgICA8TGljZW5zZWRUbz5pckRldmVsb3BlcnMuY29tPC9MaWNlbnNlZFRvPg0KICAgICAgICA8RW1haWxUbz5pbmZvQGlyRGV2ZWxvcGVycy5jb208L0VtYWlsVG8+DQogICAgICAgIDxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4NCiAgICAgICAgPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMTAwMCBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPg0KICAgICAgICA8T3JkZXJJRD43ODQzMzY0Nzc4NTwvT3JkZXJJRD4NCiAgICAgICAgPFVzZXJJRD4xMTk0NDkyNDM3OTwvVXNlcklEPg0KICAgICAgICA8T0VNPlRoaXMgaXMgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KICAgICAgICA8UHJvZHVjdHM+DQogICAgICAgICAgICA8UHJvZHVjdD5Bc3Bvc2UuVG90YWwgUHJvZHVjdCBGYW1pbHk8L1Byb2R1Y3Q+DQogICAgICAgIDwvUHJvZHVjdHM+DQogICAgICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl0aW9uVHlwZT4NCiAgICAgICAgPFNlcmlhbE51bWJlcj57RjJCOTcwNDUtMUIyOS00QjNGLUJENTMtNjAxRUZGQTE1QUE5fTwvU2VyaWFsTnVtYmVyPg0KICAgICAgICA8U3Vic2NyaXB0aW9uRXhwaXJ5PjIwOTkxMjMxPC9TdWJzY3JpcHRpb25FeHBpcnk+DQogICAgICAgIDxMaWNlbnNlVmVyc2lvbj4zLjA8L0xpY2Vuc2VWZXJzaW9uPg0KICAgIDwvRGF0YT4NCiAgICA8U2lnbmF0dXJlPlFYTndiM05sTGxSdmRHRnNMb1B5YjJSMVkzUWdSbUZ0YVd4NTwvU2lnbmF0dXJlPg0KPC9MaWNlbnNlPg==";

        public static void active()
        {
            activeWord();
       //     activePdf();
        }
        /*
        public static void activePdf()
        {
            Stream stream = new MemoryStream(Convert.FromBase64String(AsposeLicense.pdflic));
            stream.Seek(0, SeekOrigin.Begin);
            new Aspose.Pdf.License().SetLicense(stream);
        }*/
        public static void activeWord()
        {
            MemoryStream stream = new MemoryStream();  
            StreamWriter writer = new StreamWriter( stream );  
            writer.Write( AsposeLicense.wordlic );  
            writer.Flush();
            stream.Position = 0; 
            License license = new License();
            license.SetLicense(stream);
        }


    }
}
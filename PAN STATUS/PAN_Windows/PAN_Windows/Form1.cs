using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PAN_Windows
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Execute();
        }
        public static HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://krapancheck.cvlindia.com/CVLPanInquiry.svc");
            //webRequest.Headers.Add(@"SOAP:Action");
            webRequest.Headers.Add("SOAPAction", "https://krapancheck.cvlindia.com/ICVLPanInquiry/GetPassword");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static void Execute()
        {
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"
              <Envelope xmlns=""http://schemas.xmlsoap.org/soap/envelope/"">
                <Body>
                    <GetPassword xmlns=""https://krapancheck.cvlindia.com"">
                        <webApi>
                            <password>test</password>
                            <passKey>test123</passKey>
                        </webApi>
                    </GetPassword>
               </Body>
            </Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    Console.WriteLine(soapResult);
                }
            }
        }
    }
}

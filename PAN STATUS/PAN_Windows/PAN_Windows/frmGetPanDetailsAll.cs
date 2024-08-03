using System;
using System;
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
using System.Xml.Linq;
using System.Configuration;


namespace PAN_Windows
{
    public partial class frmGetPanDetailsAll : Form
    {
        public static string SoapAction_Url;
        public static string ErrorMsg;
        public static string ServiceUrl;

        public static string PanNo;
        public static string DOB;
        public static string KraCode;
        public static string FetchTypeDetail;
        public static string FetchType;
        public static string PosCode;
        public static string UserName;
        public static string EncryPtPwd;
        public static string PassKey;
        public static string BodyUlrs;
        public frmGetPanDetailsAll()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                    SecurityProtocolType.Tls11 |
                                    SecurityProtocolType.Tls12;
        }

        private void btnPanStatus_Click(object sender, EventArgs e)
        {
            try
            {
                PanNo = TxtPanNo.Text;
                DOB = dtpDOB.Value.ToString("dd/MM/yyyy");
                KraCode = CboKraCode.Text;
                FetchTypeDetail = CboFetchType.Text;
                PosCode = PanInquiry_Model.PosCode;
                UserName = PanInquiry_Model.UserName;
                EncryPtPwd = PanInquiry_Model.EncryptPwd;
                PassKey = PanInquiry_Model.PassKey;
                BodyUlrs = ConfigurationManager.AppSettings["BodyUrl"].ToString();
                if (FetchTypeDetail == "Data And Image")
                {
                    FetchType = "I";
                }
                else if (FetchTypeDetail == "Only Data")
                {
                    FetchType = "E";
                }
                else if (FetchTypeDetail == "Only Image")
                {
                    FetchType = "X";
                }
             

                if (KraCode =="")
                {
                    MessageBox.Show("Kra Code is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CboKraCode.Focus();
                    return;
                }
                if (FetchTypeDetail == "" || FetchTypeDetail == null )
                {
                    MessageBox.Show("Fetch Type is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CboFetchType .Focus();
                    return;
                }

                SoapAction_Url = ConfigurationManager.AppSettings["SoapAction_PanDetailsAll"].ToString();
                ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                GetPanStatus(SoapAction_Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static HttpWebRequest CreateWebRequest(string SoapAction_Url)
        {


            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ServiceUrl);
            //webRequest.Headers.Add(@"SOAP:Action");
            webRequest.Headers.Add("SOAPAction", SoapAction_Url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private void GetPanStatus(string SoapAction_Url)
        {
            try
            {


                HttpWebRequest request = CreateWebRequest(SoapAction_Url);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                XmlDocument OutPut_doc = new XmlDocument();

                XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
                XNamespace myns = BodyUlrs;

                XDocument CDataInput = new XDocument(
                     new XDeclaration("1.0", "UTF-8", "no"),
                     new XElement("APP_REQ_ROOT",
                     new XElement("APP_PAN_INQ",
                     new XElement("APP_PAN_NO", PanNo),
                     new XElement("APP_DOB_INCORP", DOB),
                     new XElement("APP_POS_CODE", PosCode),
                     new XElement("APP_RTA_CODE", PosCode),
                     new XElement("APP_KRA_CODE", KraCode),
                     new XElement("FETCH_TYPE", FetchType)
                            )
                        )
                    );

                XDocument soapRequest = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "no"),
                    new XElement(ns + "Envelope",
                    new XElement(ns + "Body",
                    new XElement(myns + "SolicitPANDetailsFetchALLKRA",
                    new XElement(myns + "webApi",
                    new XElement(myns + "inputXml",
                    new XCData(CDataInput.ToString())),
                    new XElement(myns + "userName", UserName),
                    new XElement(myns + "posCode", PosCode),
                    new XElement(myns + "password", EncryPtPwd),
                    new XElement(myns + "passKey", PassKey)
                                    )
                                 )
                        )
                    ));

                soapEnvelopeXml.LoadXml(soapRequest.ToString());

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse())
                {

                    OutPut_doc.Load(response.GetResponseStream());

                    foreach (XmlNode xmlNode_Hdr in OutPut_doc.GetElementsByTagName("HEADER"))
                    {
                        foreach (XmlNode childNode in xmlNode_Hdr.ChildNodes)
                        {
                            if (childNode.Name == "COMPANY_CODE")
                            {
                                string COMPANY_CODE = childNode.InnerText;
                            }
                            else if (childNode.Name == "BATCH_DATE")
                            {
                                string BATCH_DATE = childNode.InnerText;
                            }
                        }

                    }

                    foreach (XmlNode xmlNode in OutPut_doc.GetElementsByTagName("APP_PAN_INQ"))
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "APP_IOP_FLG")
                            {
                                string APP_IOP_FLG = childNode.InnerText;
                                if (APP_IOP_FLG == "RI")
                                {
                                    TxtIopFlag.Text = "Registrar Modification";
                                }
                                else if (APP_IOP_FLG == "RS")
                                {
                                    TxtIopFlag.Text = "Registrar Download/Fetch";
                                }
                                else if (APP_IOP_FLG == "RE")
                                {
                                    TxtIopFlag.Text = "Registrar Inquiry";
                                }
                                else if (APP_IOP_FLG == "II")
                                {
                                    TxtIopFlag.Text = "Intermediary Modification";
                                }
                                else if (APP_IOP_FLG == "IS")
                                {
                                    TxtIopFlag.Text = "Intermediary Download/Fetch";
                                }
                                else if (APP_IOP_FLG == "IE")
                                {
                                    TxtIopFlag.Text = "Intermediary Inquiry";
                                }
                            }
                            else if (childNode.Name == "APP_POS_CODE")
                            {
                                TxtPOSCode.Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_UPDTFLG")
                            {
                                string APP_UPDTFLG = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INT_CODE")
                            {
                                string APP_INT_CODE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_TYPE")
                            {
                                string APP_TYPE = childNode.InnerText;
                                if (APP_TYPE == "I")
                                {
                                    TxtAppType.Text = "Individual";
                                }
                                else if (APP_TYPE == "N")
                                {
                                    TxtAppType.Text = "Non-Individual";
                                }
                            }
                            else if (childNode.Name == "APP_NO")
                            {
                                TxtAppNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DATE")
                            {
                                string APP_DATE = childNode.InnerText;
                                DateTime dt;
                                if (APP_DATE != "")
                                {
                                    dt = Convert.ToDateTime(childNode.InnerText);
                                    TxtAppDate.Text = dt.ToString("dd/MM/yyyy");
                                }

                            }
                            else if (childNode.Name == "APP_EXMT")
                            {
                                string APP_EXMT = childNode.InnerText;
                                if (APP_EXMT == "Y")
                                {
                                    TxtExmt.Text = "Yes";
                                }
                                else if (APP_EXMT == "N")
                                {
                                    TxtExmt.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_EXMT_CAT")
                            {
                                string APP_EXMT_CAT = childNode.InnerText;
                                if (APP_EXMT_CAT == "01" || APP_EXMT_CAT == "1")
                                {
                                    TxtExmptCat.Text = "SIKKIM Resident";
                                }
                                else if (APP_EXMT_CAT == "02" || APP_EXMT_CAT == "2")
                                {
                                    TxtExmptCat.Text = "Transactions carried out on behalf of  STATE GOVT";
                                }
                                else if (APP_EXMT_CAT == "03" || APP_EXMT_CAT == "3")
                                {
                                    TxtExmptCat.Text = "Transactions carried out on behalf of CENTRAL GOVT";
                                }
                                else if (APP_EXMT_CAT == "04" || APP_EXMT_CAT == "4")
                                {
                                    TxtExmptCat.Text = "COURT APPOINTED OFFICIALS";
                                }
                                else if (APP_EXMT_CAT == "05" || APP_EXMT_CAT == "5")
                                {
                                    TxtExmptCat.Text = "UN Entity/Multilateral agency exempt from paying tax in India";
                                }
                                else if (APP_EXMT_CAT == "06" || APP_EXMT_CAT == "6")
                                {
                                    TxtExmptCat.Text = "Official Liquidator";
                                }
                                else if (APP_EXMT_CAT == "07" || APP_EXMT_CAT == "7")
                                {
                                    TxtExmptCat.Text = "Court Receiver";
                                }
                                else if (APP_EXMT_CAT == "08" || APP_EXMT_CAT == "8")
                                {
                                    TxtExmptCat.Text = "SIP of Mutual Funds Upto Rs. 50,000/- p.a.";
                                }
                            }
                            else if (childNode.Name == "APP_EXMT_ID_PROOF")
                            {
                                string APP_EXMT_ID_PROOF = childNode.InnerText;
                                if (APP_EXMT_ID_PROOF == "01")
                                {
                                    TxtExmptCat.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_EXMT_ID_PROOF == "02")
                                {
                                    TxtExmpIdProof.Text = "Passport";
                                }
                                else if (APP_EXMT_ID_PROOF == "03")
                                {
                                    TxtExmpIdProof.Text = "Voter ID Card";
                                }
                                else if (APP_EXMT_ID_PROOF == "04")
                                {
                                    TxtExmpIdProof.Text = "Driving License";
                                }
                                else if (APP_EXMT_ID_PROOF == "05")
                                {
                                    TxtExmpIdProof.Text = "PAN Card with Photograph";
                                }
                                else if (APP_EXMT_ID_PROOF == "06")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Central / State Govt";
                                }
                                else if (APP_EXMT_ID_PROOF == "07")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Statutory/Regulatory Authorities";
                                }
                                else if (APP_EXMT_ID_PROOF == "08")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Public Sector Undertakings";
                                }
                                else if (APP_EXMT_ID_PROOF == "09")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Scheduled Commercial Banks";
                                }
                                else if (APP_EXMT_ID_PROOF == "10")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Public Financial Institutions";
                                }
                                else if (APP_EXMT_ID_PROOF == "11")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Colleges affiliated to Universities";
                                }
                                else if (APP_EXMT_ID_PROOF == "12")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council etc., to their Members";
                                }
                                else if (APP_EXMT_ID_PROOF == "13")
                                {
                                    TxtExmpIdProof.Text = "Credit cards/Debit cards issued by Banks";
                                }
                            }
                            else if (childNode.Name == "APP_IPV_FLAG")
                            {
                                string APP_IPV_FLAG = childNode.InnerText;
                                if (APP_IPV_FLAG == "Y")
                                {
                                    TxtIpvFlag.Text = "Yes";
                                }
                                else if (APP_IPV_FLAG == "N")
                                {
                                    TxtIpvFlag.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_IPV_DATE")
                            {
                                string APP_IPV_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_GEN")
                            {
                                string APP_GEN = childNode.InnerText;
                                if (APP_GEN == "M")
                                {
                                    TxtGender.Text = "Male";
                                }
                                else if (APP_GEN == "F")
                                {
                                    TxtGender.Text = "Female";
                                }
                            }
                            else if (childNode.Name == "APP_NAME")
                            {
                                TxtName.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_F_NAME")
                            {
                                TxtFthName.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DOB_DT")
                            {
                                string APP_DOB_DT = childNode.InnerText;
                                DateTime dt;
                                if (APP_DOB_DT != "")
                                {
                                    dt = Convert.ToDateTime(childNode.InnerText);
                                    TxtDobDt.Text = dt.ToString("dd/MM/yyyy");
                                }
                            }
                            else if (childNode.Name == "APP_DOI_DT")
                            {
                                TxtDoiDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_REGNO")
                            {
                                TxtRegNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COMMENCE_DT")
                            {
                                TxtCommence.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_NATIONALITY")
                            {
                                string APP_NATIONALITY = childNode.InnerText;
                                if (APP_NATIONALITY == "01")
                                {
                                    TxtNationality.Text = "Indian";
                                }
                                else if (APP_NATIONALITY == "99")
                                {
                                    TxtNationality.Text = "Other (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_NATIONALITY")
                            {
                                string APP_OTH_NATIONALITY = childNode.InnerText;

                            }
                            else if (childNode.Name == "APP_COMP_STATUS")
                            {
                                string APP_COMP_STATUS = childNode.InnerText;
                                if (APP_COMP_STATUS == "01")
                                {
                                    TxtCompStatus.Text = "Private Ltd Company";
                                }
                                else if (APP_COMP_STATUS == "02")
                                {
                                    TxtCompStatus.Text = "Public Ltd Company";
                                }
                                else if (APP_COMP_STATUS == "03")
                                {
                                    TxtCompStatus.Text = "Body Corporate";
                                }
                                else if (APP_COMP_STATUS == "04")
                                {
                                    TxtCompStatus.Text = "Partnership";
                                }
                                else if (APP_COMP_STATUS == "05")
                                {
                                    TxtCompStatus.Text = "Trust / Charities / NGOs";
                                }
                                else if (APP_COMP_STATUS == "06")
                                {
                                    TxtCompStatus.Text = "FI";
                                }
                                else if (APP_COMP_STATUS == "07")
                                {
                                    TxtCompStatus.Text = "FII";
                                }
                                else if (APP_COMP_STATUS == "08")
                                {
                                    TxtCompStatus.Text = "HUF";
                                }
                                else if (APP_COMP_STATUS == "09")
                                {
                                    TxtCompStatus.Text = "AOP";
                                }
                                else if (APP_COMP_STATUS == "10")
                                {
                                    TxtCompStatus.Text = "Bank";
                                }
                                else if (APP_COMP_STATUS == "11")
                                {
                                    TxtCompStatus.Text = "Government Body";
                                }
                                else if (APP_COMP_STATUS == "12")
                                {
                                    TxtCompStatus.Text = "Non-Government Organisation";
                                }
                                else if (APP_COMP_STATUS == "13")
                                {
                                    TxtCompStatus.Text = "Defense Establishment";
                                }
                                else if (APP_COMP_STATUS == "14")
                                {
                                    TxtCompStatus.Text = "Body of Individuals";
                                }
                                else if (APP_COMP_STATUS == "15")
                                {
                                    TxtCompStatus.Text = "Society";
                                }
                                else if (APP_COMP_STATUS == "16")
                                {
                                    TxtCompStatus.Text = "LLP";
                                }
                                else if (APP_COMP_STATUS == "99")
                                {
                                    TxtCompStatus.Text = "Others";
                                }
                                else if (APP_COMP_STATUS == "19")
                                {
                                    TxtCompStatus.Text = "FPI - Category I";
                                }
                                else if (APP_COMP_STATUS == "20")
                                {
                                    TxtCompStatus.Text = "FPI - Category II";
                                }
                                else if (APP_COMP_STATUS == "21")
                                {
                                    TxtCompStatus.Text = "FPI - Category III";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_COMP_STATUS")
                            {

                                TxtOthCompStatus.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RES_STATUS")
                            {
                                string APP_RES_STATUS = childNode.InnerText;

                                if (APP_RES_STATUS == "R")
                                {
                                    TxtResStatus.Text = "Resident Individual";
                                }
                                else if (APP_RES_STATUS == "N")
                                {
                                    TxtResStatus.Text = "Non Resident";
                                }
                                else if (APP_RES_STATUS == "P")
                                {
                                    TxtResStatus.Text = "Foreign National";
                                }
                            }
                            else if (childNode.Name == "APP_RES_STATUS_PROOF")
                            {
                                TxtRefProof.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PAN_NO")
                            {
                                TxtAppPan.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PANEX_NO")
                            {
                                TxtPanexNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PAN_COPY")
                            {
                                string APP_PAN_COPY = childNode.InnerText;
                                if (APP_PAN_COPY == "Y")
                                {
                                    TxtPanCopy.Text = "Yes";
                                }
                                else if (APP_PAN_COPY == "N")
                                {
                                    TxtPanCopy.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_UID_NO")
                            {
                                TxtUidNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD1")
                            {
                                TxtCurrAddr1.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD2")
                            {
                                TxtCurrAddr2.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD3")
                            {
                                TxtCurrAddr3.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_CITY")
                            {
                                TxtCurrCity.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_PINCD")
                            {
                                TxtCurrPincode.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_STATE")
                            {
                                string APP_COR_STATE = childNode.InnerText;
                                if(APP_COR_STATE == "035")
                                {
                                    TxtCurrState.Text = "Andaman & Nicobar Islands";
                                }
                                else if (APP_COR_STATE == "028")
                                {
                                    TxtCurrState.Text = "Andhra Pradesh";
                                }
                                else if(APP_COR_STATE == "012")
                                {
                                    TxtCurrState .Text = "Arunachal Pradesh";
                                }
                                else if (APP_COR_STATE == "013")
                                {
                                    TxtCurrState.Text = "Assam";
                                }
                                else if(APP_COR_STATE == "010")
                                {
                                    TxtCurrState.Text = "Bihar";
                                }
                                else if (APP_COR_STATE == "004")
                                {
                                    TxtCurrState.Text = "Chandigarh";
                                }
                                else if (APP_COR_STATE == "026")
                                {
                                    TxtCurrState.Text = "Dadra & Nagar Haveli";
                                }
                                else if (APP_COR_STATE == "025")
                                {
                                    TxtCurrState.Text = "Daman & Diu";
                                }
                                else if (APP_COR_STATE == "007")
                                {
                                    TxtCurrState.Text = "Delhi";
                                }
                                else if (APP_COR_STATE == "030")
                                {
                                    TxtCurrState.Text = "Goa";
                                }
                                else if (APP_COR_STATE == "024")
                                {
                                    TxtCurrState.Text = "Gujarat";
                                }
                                else if (APP_COR_STATE == "006")
                                {
                                    TxtCurrState.Text = "Haryana";
                                }
                                else if (APP_COR_STATE == "002")
                                {
                                    TxtCurrState.Text = "Himachal Pradesh";
                                }
                                else if (APP_COR_STATE == "001")
                                {
                                    TxtCurrState.Text = "Jammu & Kashmir";
                                }
                                else if (APP_COR_STATE == "029")
                                {
                                    TxtCurrState.Text = "Karnataka";
                                }
                                else if (APP_COR_STATE == "032")
                                {
                                    TxtCurrState.Text = "Kerala";
                                }
                                else if (APP_COR_STATE == "031")
                                {
                                    TxtCurrState.Text = "Lakhswadeep";
                                }
                                else if (APP_COR_STATE == "023")
                                {
                                    TxtCurrState.Text = "Madhya Pradesh";
                                }
                                else if (APP_COR_STATE == "027")
                                {
                                    TxtCurrState.Text = "Maharashtra";
                                }
                                else if (APP_COR_STATE == "014")
                                {
                                    TxtCurrState.Text = "Manipur";
                                }
                                else if (APP_COR_STATE == "017")
                                {
                                    TxtCurrState.Text = "Meghalaya";
                                }
                                else if (APP_COR_STATE == "015")
                                {
                                    TxtCurrState.Text = "Mizoram";
                                }
                                else if (APP_COR_STATE == "018")
                                {
                                    TxtCurrState.Text = "Nagaland";
                                }
                                else if (APP_COR_STATE == "021")
                                {
                                    TxtCurrState.Text = "Orissa";
                                }
                                else if (APP_COR_STATE == "034")
                                {
                                    TxtCurrState.Text = "Pondicherry";
                                }
                                else if (APP_COR_STATE == "003")
                                {
                                    TxtCurrState.Text = "Punjab";
                                }
                                else if (APP_COR_STATE == "008")
                                {
                                    TxtCurrState.Text = "Rajasthan";
                                }
                                else if (APP_COR_STATE == "011")
                                {
                                    TxtCurrState.Text = "Sikkim";
                                }
                                else if (APP_COR_STATE == "033")
                                {
                                    TxtCurrState.Text = "Tamil Nadu";
                                }
                                else if (APP_COR_STATE == "016")
                                {
                                    TxtCurrState.Text = "Tripura";
                                }
                                else if (APP_COR_STATE == "009")
                                {
                                    TxtCurrState.Text = "Uttar Pradesh";
                                }
                                else if (APP_COR_STATE == "019")
                                {
                                    TxtCurrState.Text = "West Bengal";
                                }
                                else if (APP_COR_STATE == "022")
                                {
                                    TxtCurrState.Text = "Chhattisgarh";
                                }
                                else if (APP_COR_STATE == "005")
                                {
                                    TxtCurrState.Text = "Uttaranchal";
                                }
                                else if (APP_COR_STATE == "020")
                                {
                                    TxtCurrState.Text = "Jharkhand";
                                }
                                else if (APP_COR_STATE == "037")
                                {
                                    TxtCurrState.Text = "Telangana";
                                }
                                else if (APP_COR_STATE == "099")
                                {
                                    TxtCurrState.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_COR_CTRY")
                            {
                                string APP_COR_CTRY = childNode.InnerText;
                                if (APP_COR_CTRY == "001")
                                {
                                    TxtCurrnCountry.Text = "AFGHANISTAN";
                                }
                                else if (APP_COR_CTRY == "003")
                                {
                                    TxtCurrnCountry.Text = "ALBENIA";
                                }
                                else if(APP_COR_CTRY == "004")
                                {
                                    TxtCurrnCountry.Text = "ALGERIA";
                                }
                                else if(APP_COR_CTRY == "007")
                                {
                                    TxtCurrnCountry .Text ="ANGOLA";
                                }
                                else if (APP_COR_CTRY == "011")
                                {
                                    TxtCurrnCountry.Text = "ARGENTINA";
                                }
                                else if (APP_COR_CTRY == "012")
                                {
                                    TxtCurrnCountry.Text = "ARMENIA";
                                }
                                else if (APP_COR_CTRY == "013")
                                {
                                    TxtCurrnCountry.Text = "ARUBA";
                                }
                                else if (APP_COR_CTRY == "014")
                                {
                                    TxtCurrnCountry.Text = "AUSTRALIA";
                                }
                                else if (APP_COR_CTRY == "015")
                                {
                                    TxtCurrnCountry.Text = "AUSTRIA";
                                }
                                else if (APP_COR_CTRY == "016")
                                {
                                    TxtCurrnCountry.Text = "AZARBAIJAN";
                                }
                                else if (APP_COR_CTRY == "017")
                                {
                                    TxtCurrnCountry.Text = "BAHAMAS";
                                }
                                else if (APP_COR_CTRY == "018")
                                {
                                    TxtCurrnCountry.Text = "BAHRAIN";
                                }
                                else if (APP_COR_CTRY == "019")
                                {
                                    TxtCurrnCountry.Text = "BANGLADESH";
                                }
                                else if (APP_COR_CTRY == "020")
                                {
                                    TxtCurrnCountry.Text = "BARBADOS";
                                }
                                else if (APP_COR_CTRY == "021")
                                {
                                    TxtCurrnCountry.Text = "BELARUS";
                                }
                                else if (APP_COR_CTRY == "022")
                                {
                                    TxtCurrnCountry.Text = "BELGIUM";
                                }
                                else if (APP_COR_CTRY == "023")
                                {
                                    TxtCurrnCountry.Text = "BELIZE";
                                }
                                else if (APP_COR_CTRY == "024")
                                {
                                    TxtCurrnCountry.Text = "BENIN";
                                }
                                else if (APP_COR_CTRY == "025")
                                {
                                    TxtCurrnCountry.Text = "BERMUDA";
                                }
                                else if (APP_COR_CTRY == "026")
                                {
                                    TxtCurrnCountry.Text = "BHUTAN";
                                }
                                else if (APP_COR_CTRY == "027")
                                {
                                    TxtCurrnCountry.Text = "BOLIVIAN";
                                }
                                else if (APP_COR_CTRY == "028")
                                {
                                    TxtCurrnCountry.Text = "BOSNIA-HERZEGOVINA";
                                }
                                else if (APP_COR_CTRY == "029")
                                {
                                    TxtCurrnCountry.Text = "BOTSWANA";
                                }
                                else if (APP_COR_CTRY == "031")
                                {
                                    TxtCurrnCountry.Text = "BRAZIL";
                                }
                                else if (APP_COR_CTRY == "25")
                                {
                                    TxtCurrnCountry.Text = "BRUNEI";
                                }
                                else if (APP_COR_CTRY == "034")
                                {
                                    TxtCurrnCountry.Text = "BULGARIA";
                                }
                                else if (APP_COR_CTRY == "035")
                                {
                                    TxtCurrnCountry.Text = "BURKINA FASO";
                                }
                                else if (APP_COR_CTRY == "036")
                                {
                                    TxtCurrnCountry.Text = "BURUNDI";
                                }
                                else if(APP_COR_CTRY == "")
                                {
                                    TxtCurrnCountry.Text = "CAMEROON REPUBLIC";
                                }
                                else if(APP_COR_CTRY =="039")
                                {
                                    TxtCurrnCountry.Text = "CANADA";
                                }
                                else if(APP_COR_CTRY == "040")
                                {
                                    TxtCurrnCountry.Text = "CAPE VERDE";
                                }
                                else if (APP_COR_CTRY == "041")
                                {
                                    TxtCurrnCountry.Text = "CAYMAN ISLANDS";
                                }
                                else if (APP_COR_CTRY == "042")
                                {
                                    TxtCurrnCountry.Text = "CENTRAL AFRICAN REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "043")
                                {
                                    TxtCurrnCountry.Text = "CHAD";
                                }
                                else if (APP_COR_CTRY == "044")
                                {
                                    TxtCurrnCountry.Text = "CHILE";
                                }
                                else if (APP_COR_CTRY == "045")
                                {
                                    TxtCurrnCountry.Text = "CHINA";
                                }
                                else if (APP_COR_CTRY == "048")
                                {
                                    TxtCurrnCountry.Text = "COLOMBIA";
                                }
                                else if (APP_COR_CTRY == "037")
                                {
                                    TxtCurrnCountry.Text = "COMBODIA";
                                }
                                else if (APP_COR_CTRY == "038")
                                {
                                    TxtCurrnCountry.Text = "COMOROS";
                                }
                                else if (APP_COR_CTRY == "050")
                                {
                                    TxtCurrnCountry.Text = "CONGO";
                                }
                                else if (APP_COR_CTRY == "052")
                                {
                                    TxtCurrnCountry.Text = "COOK ISLANDS";
                                }
                                else if (APP_COR_CTRY == "053")
                                {
                                    TxtCurrnCountry.Text = "COSTA RICA";
                                }
                                else if (APP_COR_CTRY == "054")
                                {
                                    TxtCurrnCountry.Text = "COTE D'IVOIRE";
                                }
                                else if (APP_COR_CTRY == "055")
                                {
                                    TxtCurrnCountry.Text = "CROATIA";
                                }
                                else if (APP_COR_CTRY == "056")
                                {
                                    TxtCurrnCountry.Text = "CUBA";
                                }
                                else if (APP_COR_CTRY == "057")
                                {
                                    TxtCurrnCountry.Text = "CYPRUS";
                                }
                                else if (APP_COR_CTRY == "058")
                                {
                                    TxtCurrnCountry.Text = "CZECH REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "059")
                                {
                                    TxtCurrnCountry.Text = "DENMARK";
                                }
                                else if (APP_COR_CTRY == "060")
                                {
                                    TxtCurrnCountry.Text = "DJIBOUTI";
                                }
                                else if (APP_COR_CTRY == "061")
                                {
                                    TxtCurrnCountry.Text = "DOMINICA";
                                }
                                else if (APP_COR_CTRY == "062")
                                {
                                    TxtCurrnCountry.Text = "DOMINICAN REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "063")
                                {
                                    TxtCurrnCountry.Text = "ECUADOR";
                                }
                                else if (APP_COR_CTRY == "064")
                                {
                                    TxtCurrnCountry.Text = "EGYPT";
                                }
                                else if (APP_COR_CTRY == "065")
                                {
                                    TxtCurrnCountry.Text = "EL SALVADOR";
                                }
                                else if (APP_COR_CTRY == "066")
                                {
                                    TxtCurrnCountry.Text = "EQUATORIAL GUINEA";
                                }
                                else if (APP_COR_CTRY == "068")
                                {
                                    TxtCurrnCountry.Text = "ESTONIA";
                                }
                                else if (APP_COR_CTRY == "069")
                                {
                                    TxtCurrnCountry.Text = "ETHIOPIA";
                                }
                                else if (APP_COR_CTRY == "072")
                                {
                                    TxtCurrnCountry.Text = "FIJI";
                                }
                                else if (APP_COR_CTRY == "073")
                                {
                                    TxtCurrnCountry.Text = "FINLAND";
                                }
                                else if (APP_COR_CTRY == "074")
                                {
                                    TxtCurrnCountry.Text = "FRANCE";
                                }
                                else if (APP_COR_CTRY == "075")
                                {
                                    TxtCurrnCountry.Text = "FRENCH GUIANA";
                                }
                                else if (APP_COR_CTRY == "076")
                                {
                                    TxtCurrnCountry.Text = "FRENCH POLYNESIA";
                                }
                                else if (APP_COR_CTRY == "078")
                                {
                                    TxtCurrnCountry.Text = "GABON";
                                }
                                else if (APP_COR_CTRY == "079")
                                {
                                    TxtCurrnCountry.Text = "GAMBIA";
                                }
                                else if (APP_COR_CTRY == "080")
                                {
                                    TxtCurrnCountry.Text = "GEORGIA";
                                }
                                else if (APP_COR_CTRY == "081")
                                {
                                    TxtCurrnCountry.Text = "GERMANY";
                                }
                                else if (APP_COR_CTRY == "082")
                                {
                                    TxtCurrnCountry.Text = "GHANA";
                                }
                                else if (APP_COR_CTRY == "083")
                                {
                                    TxtCurrnCountry.Text = "GIBRALTOR";
                                }
                                else if (APP_COR_CTRY == "084")
                                {
                                    TxtCurrnCountry.Text = "GREECE";
                                }
                                else if (APP_COR_CTRY == "085")
                                {
                                    TxtCurrnCountry.Text = "GREENLAND";
                                }
                                else if (APP_COR_CTRY == "086")
                                {
                                    TxtCurrnCountry.Text = "GRENADA";
                                }
                                else if (APP_COR_CTRY == "087")
                                {
                                    TxtCurrnCountry.Text = "GUADELOUPE";
                                }
                                else if (APP_COR_CTRY == "088")
                                {
                                    TxtCurrnCountry.Text = "GUAM";
                                }
                                else if (APP_COR_CTRY == "089")
                                {
                                    TxtCurrnCountry.Text = "GUATEMALA";
                                }
                                else if (APP_COR_CTRY == "090")
                                {
                                    TxtCurrnCountry.Text = "GUERNSEY";
                                }
                                else if (APP_COR_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_COR_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_COR_CTRY == "092")
                                {
                                    TxtCurrnCountry.Text = "GUINEA-BISSAU";
                                }
                                else if (APP_COR_CTRY == "093")
                                {
                                    TxtCurrnCountry.Text = "GUYANA";
                                }
                                else if (APP_COR_CTRY == "094")
                                {
                                    TxtCurrnCountry.Text = "HAITI";
                                }
                                else if (APP_COR_CTRY == "097")
                                {
                                    TxtCurrnCountry.Text = "HONDURAS";
                                }
                                else if (APP_COR_CTRY == "098")
                                {
                                    TxtCurrnCountry.Text = "HONGKONG";
                                }
                                else if (APP_COR_CTRY == "100")
                                {
                                    TxtCurrnCountry.Text = "ICELAND";
                                }
                                else if (APP_COR_CTRY == "101")
                                {
                                    TxtCurrnCountry.Text = "INDIA";
                                }
                                else if (APP_COR_CTRY == "102")
                                {
                                    TxtCurrnCountry.Text = "INDONESIA";
                                }
                                else if (APP_COR_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN";
                                }
                                else if (APP_COR_CTRY == "104")
                                {
                                    TxtCurrnCountry.Text = "IRAQ";
                                }
                                else if (APP_COR_CTRY == "105")
                                {
                                    TxtCurrnCountry.Text = "IRELAND";
                                }
                                else if (APP_COR_CTRY == "107")
                                {
                                    TxtCurrnCountry.Text = "ISRAEL";
                                }
                                else if (APP_COR_CTRY == "108")
                                {
                                    TxtCurrnCountry.Text = "ITALY";
                                }
                                else if (APP_COR_CTRY == "109")
                                {
                                    TxtCurrnCountry.Text = "JAMAICA";
                                }
                                else if (APP_COR_CTRY == "110")
                                {
                                    TxtCurrnCountry.Text = "JAPAN";
                                }
                                else if (APP_COR_CTRY == "112")
                                {
                                    TxtCurrnCountry.Text = "JORDAN";
                                }
                                else if (APP_COR_CTRY == "113")
                                {
                                    TxtCurrnCountry.Text = "KAZAKSTAN";
                                }
                                else if (APP_COR_CTRY == "114")
                                {
                                    TxtCurrnCountry.Text = "KENYA";
                                }
                                else if (APP_COR_CTRY == "118")
                                {
                                    TxtCurrnCountry.Text = "KUWAIT";
                                }
                                else if (APP_COR_CTRY == "119")
                                {
                                    TxtCurrnCountry.Text = "KYRGYZSTAN";
                                }
                                else if (APP_COR_CTRY == "121")
                                {
                                    TxtCurrnCountry.Text = "LATVIA";
                                }
                                else if (APP_COR_CTRY == "122")
                                {
                                    TxtCurrnCountry.Text = "LEBANON";
                                }
                                else if (APP_COR_CTRY == "123")
                                {
                                    TxtCurrnCountry.Text = "LESOTHO";
                                }
                                else if (APP_COR_CTRY == "124")
                                {
                                    TxtCurrnCountry.Text = "LIBERIA";
                                }
                                else if (APP_COR_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYA";
                                }
                                else if (APP_COR_CTRY == "127")
                                {
                                    TxtCurrnCountry.Text = "LITHUANIA";
                                }
                                else if (APP_COR_CTRY == "128")
                                {
                                    TxtCurrnCountry.Text = "LUXEMBOURG";
                                }
                                else if (APP_COR_CTRY == "129")
                                {
                                    TxtCurrnCountry.Text = "MACAU";
                                }
                                else if (APP_COR_CTRY == "130")
                                {
                                    TxtCurrnCountry.Text = "MACEDONIA";
                                }
                                else if (APP_COR_CTRY == "131")
                                {
                                    TxtCurrnCountry.Text = "MADAGASCAR";
                                }
                                else if (APP_COR_CTRY == "132")
                                {
                                    TxtCurrnCountry.Text = "MALAWI";
                                }
                                else if (APP_COR_CTRY == "133")
                                {
                                    TxtCurrnCountry.Text = "MALAYSIA";
                                }
                                else if (APP_COR_CTRY == "134")
                                {
                                    TxtCurrnCountry.Text = "MALDIVES";
                                }
                                else if (APP_COR_CTRY == "135")
                                {
                                    TxtCurrnCountry.Text = "MALI";
                                }
                                else if (APP_COR_CTRY == "136")
                                {
                                    TxtCurrnCountry.Text = "MALTA";
                                }
                                else if (APP_COR_CTRY == "139")
                                {
                                    TxtCurrnCountry.Text = "MAURITANIA";
                                }
                                else if (APP_COR_CTRY == "140")
                                {
                                    TxtCurrnCountry.Text = "MAURITIUS";
                                }
                                else if (APP_COR_CTRY == "142")
                                {
                                    TxtCurrnCountry.Text = "MEXICO";
                                }
                                else if (APP_COR_CTRY == "144")
                                {
                                    TxtCurrnCountry.Text = "MOLDOVA";
                                }
                                else if (APP_COR_CTRY == "146")
                                {
                                    TxtCurrnCountry.Text = "MONGOLIA";
                                }
                                else if (APP_COR_CTRY == "147")
                                {
                                    TxtCurrnCountry.Text = "MONTSERRAT";
                                }
                                else if (APP_COR_CTRY == "148")
                                {
                                    TxtCurrnCountry.Text = "MOROCCA";
                                }
                                else if (APP_COR_CTRY == "149")
                                {
                                    TxtCurrnCountry.Text = "MOZAMBIQUE";
                                }
                                else if (APP_COR_CTRY == "150")
                                {
                                    TxtCurrnCountry.Text = "MYANMAR";
                                }
                                else if (APP_COR_CTRY == "151")
                                {
                                    TxtCurrnCountry.Text = "NAMIBIA";
                                }
                                else if (APP_COR_CTRY == "153")
                                {
                                    TxtCurrnCountry.Text = "NEPAL";
                                }
                                else if (APP_COR_CTRY == "154")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS";
                                }
                                else if (APP_COR_CTRY == "155")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS ANTILLES";
                                }
                                else if (APP_COR_CTRY == "156")
                                {
                                    TxtCurrnCountry.Text = "NEW CALEDONIA";
                                }
                                else if (APP_COR_CTRY == "157")
                                {
                                    TxtCurrnCountry.Text = "NEW ZEALAND";
                                }
                                else if (APP_COR_CTRY == "158")
                                {
                                    TxtCurrnCountry.Text = "NICARAGUA";
                                }
                                else if (APP_COR_CTRY == "159")
                                {
                                    TxtCurrnCountry.Text = "NIGER";
                                }
                                else if (APP_COR_CTRY == "160")
                                {
                                    TxtCurrnCountry.Text = "NIGERIA";
                                }
                                else if (APP_COR_CTRY == "116")
                                {
                                    TxtCurrnCountry.Text = "NORTH KOREA";
                                }
                                else if (APP_COR_CTRY == "164")
                                {
                                    TxtCurrnCountry.Text = "NORWAY";
                                }
                                else if (APP_COR_CTRY == "165")
                                {
                                    TxtCurrnCountry.Text = "OMAN";
                                }
                                else if (APP_COR_CTRY == "166")
                                {
                                    TxtCurrnCountry.Text = "PAKISTAN";
                                }
                                else if (APP_COR_CTRY == "169")
                                {
                                    TxtCurrnCountry.Text = "PANAMA";
                                }
                                else if (APP_COR_CTRY == "170")
                                {
                                    TxtCurrnCountry.Text = "PAPUA NEW GUINEA";
                                }
                                else if (APP_COR_CTRY == "171")
                                {
                                    TxtCurrnCountry.Text = "PARAGUAY";
                                }
                                else if (APP_COR_CTRY == "172")
                                {
                                    TxtCurrnCountry.Text = "PERU";
                                }
                                else if (APP_COR_CTRY == "173")
                                {
                                    TxtCurrnCountry.Text = "PHILIPPINES";
                                }
                                else if (APP_COR_CTRY == "175")
                                {
                                    TxtCurrnCountry.Text = "POLAND";
                                }
                                else if (APP_COR_CTRY == "176")
                                {
                                    TxtCurrnCountry.Text = "PORTUGAL";
                                }
                                else if (APP_COR_CTRY == "178")
                                {
                                    TxtCurrnCountry.Text = "QATAR";
                                }
                                else if (APP_COR_CTRY == "180")
                                {
                                    TxtCurrnCountry.Text = "ROMANIA";
                                }
                                else if (APP_COR_CTRY == "181")
                                {
                                    TxtCurrnCountry.Text = "RUSSIA";
                                }
                                else if (APP_COR_CTRY == "182")
                                {
                                    TxtCurrnCountry.Text = "RWANDA";
                                }
                                else if (APP_COR_CTRY == "191")
                                {
                                    TxtCurrnCountry.Text = "SAUDI ARABIA";
                                }
                                else if (APP_COR_CTRY == "192")
                                {
                                    TxtCurrnCountry.Text = "SENEGAL";
                                }
                                else if (APP_COR_CTRY == "194")
                                {
                                    TxtCurrnCountry.Text = "SEYCHELLES";
                                }
                                else if (APP_COR_CTRY == "196")
                                {
                                    TxtCurrnCountry.Text = "SINGAPORE";
                                }
                                else if (APP_COR_CTRY == "197")
                                {
                                    TxtCurrnCountry.Text = "SLOVAKIA";
                                }
                                else if (APP_COR_CTRY == "198")
                                {
                                    TxtCurrnCountry.Text = "SLOVENIA";
                                }
                                else if (APP_COR_CTRY == "199")
                                {
                                    TxtCurrnCountry.Text = "SOLOMON ISLANDS";
                                }
                                else if (APP_COR_CTRY == "200")
                                {
                                    TxtCurrnCountry.Text = "SOMALIA";
                                }
                                else if (APP_COR_CTRY == "201")
                                {
                                    TxtCurrnCountry.Text = "SOUTH AFRICA";
                                }
                                else if (APP_COR_CTRY == "117")
                                {
                                    TxtCurrnCountry.Text = "SOUTH KOREA";
                                }
                                else if (APP_COR_CTRY == "203")
                                {
                                    TxtCurrnCountry.Text = "SPAIN";
                                }
                                else if (APP_COR_CTRY == "204")
                                {
                                    TxtCurrnCountry.Text = "SRI LANKA";
                                }
                                else if (APP_COR_CTRY == "205")
                                {
                                    TxtCurrnCountry.Text = "SUDAN";
                                }
                                else if (APP_COR_CTRY == "206")
                                {
                                    TxtCurrnCountry.Text = "SURINAME";
                                }
                                else if (APP_COR_CTRY == "208")
                                {
                                    TxtCurrnCountry.Text = "SWAZILAND";
                                }
                                else if (APP_COR_CTRY == "209")
                                {
                                    TxtCurrnCountry.Text = "SWEDEN";
                                }
                                else if (APP_COR_CTRY == "210")
                                {
                                    TxtCurrnCountry.Text = "SWITZERLAND";
                                }
                                else if (APP_COR_CTRY == "212")
                                {
                                    TxtCurrnCountry.Text = "TAIWAN";
                                }
                                else if (APP_COR_CTRY == "213")
                                {
                                    TxtCurrnCountry.Text = "TAJIKISTHAN";
                                }
                                else if (APP_COR_CTRY == "214")
                                {
                                    TxtCurrnCountry.Text = "TANZANIA";
                                }
                                else if (APP_COR_CTRY == "215")
                                {
                                    TxtCurrnCountry.Text = "THAILAND";
                                }
                                else if (APP_COR_CTRY == "217")
                                {
                                    TxtCurrnCountry.Text = "TOGO REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "219")
                                {
                                    TxtCurrnCountry.Text = "TONGA";
                                }
                                else if (APP_COR_CTRY == "220")
                                {
                                    TxtCurrnCountry.Text = "TRINIDAD AND TOBAGO";
                                }
                                else if (APP_COR_CTRY == "221")
                                {
                                    TxtCurrnCountry.Text = "TUNISIA";
                                }
                                else if (APP_COR_CTRY == "222")
                                {
                                    TxtCurrnCountry.Text = "TURKEY";
                                }
                                else if (APP_COR_CTRY == "223")
                                {
                                    TxtCurrnCountry.Text = "TURKMENISTAN";
                                }
                                else if (APP_COR_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "U A E";
                                }
                                else if (APP_COR_CTRY == "226")
                                {
                                    TxtCurrnCountry.Text = "UGANDA";
                                }
                                else if (APP_COR_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "UNITED ARAB EMIRATES";
                                }
                                else if (APP_COR_CTRY == "229")
                                {
                                    TxtCurrnCountry.Text = "UNITED KINGDOM";
                                }
                                else if (APP_COR_CTRY == "232")
                                {
                                    TxtCurrnCountry.Text = "URUGUAY";
                                }
                                else if (APP_COR_CTRY == "230")
                                {
                                    TxtCurrnCountry.Text = "USA";
                                }
                                else if (APP_COR_CTRY == "234")
                                {
                                    TxtCurrnCountry.Text = "VANUATU";
                                }
                                else if (APP_COR_CTRY == "235")
                                {
                                    TxtCurrnCountry.Text = "VENEZUELA";
                                }
                                else if (APP_COR_CTRY == "236")
                                {
                                    TxtCurrnCountry.Text = "VIETNAM";
                                }
                                else if (APP_COR_CTRY == "241")
                                {
                                    TxtCurrnCountry.Text = "YEMEN";
                                }
                                else if (APP_COR_CTRY == "242")
                                {
                                    TxtCurrnCountry.Text = "ZAMBIA";
                                }
                                else if (APP_COR_CTRY == "243")
                                {
                                    TxtCurrnCountry.Text = "ZIMBABWE";
                                }
                                else if (APP_COR_CTRY == "002")
                                {
                                    TxtCurrnCountry.Text = "ALAND ISLANDS";
                                }
                                else if (APP_COR_CTRY == "005")
                                {
                                    TxtCurrnCountry.Text = "AMERICAN SAMOA";
                                }
                                else if (APP_COR_CTRY == "006")
                                {
                                    TxtCurrnCountry.Text = "ANDORRA";
                                }
                                else if (APP_COR_CTRY == "008")
                                {
                                    TxtCurrnCountry.Text = "ANGUILLA";
                                }
                                else if (APP_COR_CTRY == "009")
                                {
                                    TxtCurrnCountry.Text = "ANTARCTICA";
                                }
                                else if (APP_COR_CTRY == "010")
                                {
                                    TxtCurrnCountry.Text = "ANTIGUA AND BARBUDA";
                                }
                                else if (APP_COR_CTRY == "030")
                                {
                                    TxtCurrnCountry.Text = "BOUVET ISLAND";
                                }
                                else if (APP_COR_CTRY == "032")
                                {
                                    TxtCurrnCountry.Text = "BRITISH INDIAN OCEAN TERRITORY";
                                }
                                else if (APP_COR_CTRY == "046")
                                {
                                    TxtCurrnCountry.Text = "CHRISTMAS ISLAND";
                                }
                                else if (APP_COR_CTRY == "047")
                                {
                                    TxtCurrnCountry.Text = "COCOS (KEELING) ISLANDS";
                                }
                                else if (APP_COR_CTRY == "067")
                                {
                                    TxtCurrnCountry.Text = "ERITREA";
                                }
                                else if (APP_COR_CTRY == "070")
                                {
                                    TxtCurrnCountry.Text = "FALKLAND ISLANDS (MALVINAS)";
                                }
                                else if (APP_COR_CTRY == "071")
                                {
                                    TxtCurrnCountry.Text = "FAROE ISLANDS";
                                }
                                else if (APP_COR_CTRY == "077")
                                {
                                    TxtCurrnCountry.Text = "FRENCH SOUTHERN TERRITORIES";
                                }
                                else if (APP_COR_CTRY == "095")
                                {
                                    TxtCurrnCountry.Text = "HEARD ISLAND AND MCDONALD ISLANDS";
                                }
                                else if (APP_COR_CTRY == "096")
                                {
                                    TxtCurrnCountry.Text = "HOLY SEE (VATICAN CITY STATE)";
                                }
                                else if (APP_COR_CTRY == "099")
                                {
                                    TxtCurrnCountry.Text = "HUNGARY";
                                }
                                else if (APP_COR_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN, ISLAMIC REPUBLIC OF";
                                }
                                else if (APP_COR_CTRY == "106")
                                {
                                    TxtCurrnCountry.Text = "ISLE OF MAN";
                                }
                                else if (APP_COR_CTRY == "115")
                                {
                                    TxtCurrnCountry.Text = "KIRIBATI";
                                }
                                else if (APP_COR_CTRY == "120")
                                {
                                    TxtCurrnCountry.Text = "LAO PEOPLE'S DEMOCRATIC REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYAN ARAB JAMAHIRIYA";
                                }
                                else if (APP_COR_CTRY == "126")
                                {
                                    TxtCurrnCountry.Text = "LIECHTENSTEIN";
                                }
                                else if (APP_COR_CTRY == "137")
                                {
                                    TxtCurrnCountry.Text = "MARSHALL ISLANDS";
                                }
                                else if(APP_COR_CTRY == "138")
                                {
                                    TxtCurrnCountry.Text = "MARTINIQUE";
                                }
                                else if (APP_COR_CTRY == "141")
                                {
                                    TxtCurrnCountry.Text = "MAYOTTE";
                                }
                                else if (APP_COR_CTRY == "143")
                                {
                                    TxtCurrnCountry.Text = "MICRONESIA, FEDERATED STATES OF";
                                }
                                else if (APP_COR_CTRY == "145")
                                {
                                    TxtCurrnCountry.Text = "MONACO";
                                }
                                else if (APP_COR_CTRY == "152")
                                {
                                    TxtCurrnCountry.Text = "NAURU";
                                }
                                else if (APP_COR_CTRY == "161")
                                {
                                    TxtCurrnCountry.Text = "NIUE";
                                }
                                else if (APP_COR_CTRY == "162")
                                {
                                    TxtCurrnCountry.Text = "NORFOLK ISLAND";
                                }
                                else if (APP_COR_CTRY == "163")
                                {
                                    TxtCurrnCountry.Text = "NORTHERN MARIANA ISLANDS";
                                }
                                else if (APP_COR_CTRY == "167")
                                {
                                    TxtCurrnCountry.Text = "PALAU";
                                }
                                else if (APP_COR_CTRY == "168")
                                {
                                    TxtCurrnCountry.Text = "PALESTINIAN TERRITORY, OCCUPIED";
                                }
                                else if (APP_COR_CTRY == "174")
                                {
                                    TxtCurrnCountry.Text = "PITCAIRN";
                                }
                                else if (APP_COR_CTRY == "177")
                                {
                                    TxtCurrnCountry.Text = "PUERTO RICO";
                                }
                                else if (APP_COR_CTRY == "179")
                                {
                                    TxtCurrnCountry.Text = "REUNION";
                                }
                                else if (APP_COR_CTRY == "183")
                                {
                                    TxtCurrnCountry.Text = "SAINT HELENA";
                                }
                                else if (APP_COR_CTRY == "184")
                                {
                                    TxtCurrnCountry.Text = "SAINT KITTS AND NEVIS";
                                }
                                else if (APP_COR_CTRY == "185")
                                {
                                    TxtCurrnCountry.Text = "SAINT LUCIA";
                                }
                                else if (APP_COR_CTRY == "186")
                                {
                                    TxtCurrnCountry.Text = "SAINT PIERRE AND MIQUELON";
                                }
                                else if (APP_COR_CTRY == "187")
                                {
                                    TxtCurrnCountry.Text = "SAINT VINCENT AND THE GRENADINES";
                                }
                                else if (APP_COR_CTRY == "188")
                                {
                                    TxtCurrnCountry.Text = "SAMOA";
                                }
                                else if (APP_COR_CTRY == "189")
                                {
                                    TxtCurrnCountry.Text = "SAN MARINO";
                                }
                                else if (APP_COR_CTRY == "190")
                                {
                                    TxtCurrnCountry.Text = "SAO TOME AND PRINCIPE";
                                }
                                else if (APP_COR_CTRY == "193")
                                {
                                    TxtCurrnCountry.Text = "SERBIA AND MONTENEGRO";
                                }
                                else if (APP_COR_CTRY == "195")
                                {
                                    TxtCurrnCountry.Text = "SIERRA LEONE";
                                }
                                else if (APP_COR_CTRY == "202")
                                {
                                    TxtCurrnCountry.Text = "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS";
                                }
                                else if(APP_COR_CTRY == "207")
                                {
                                    TxtCurrnCountry.Text = "SVALBARD AND JAN MAYEN";
                                }
                                else if (APP_COR_CTRY == "211")
                                {
                                    TxtCurrnCountry.Text = "SYRIAN ARAB REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "216")
                                {
                                    TxtCurrnCountry.Text = "TIMOR-LESTE";
                                }
                                else if (APP_COR_CTRY == "218")
                                {
                                    TxtCurrnCountry.Text = "TOKELAU";
                                }
                                else if (APP_COR_CTRY == "224")
                                {
                                    TxtCurrnCountry.Text = "TURKS AND CAICOS ISLANDS";
                                }
                                else if (APP_COR_CTRY == "225")
                                {
                                    TxtCurrnCountry.Text = "TUVALU";
                                }
                                else if (APP_COR_CTRY == "227")
                                {
                                    TxtCurrnCountry.Text = "UKRAINE";
                                }
                                else if (APP_COR_CTRY == "231")
                                {
                                    TxtCurrnCountry.Text = "UNITED STATES MINOR OUTLYING ISLANDS";
                                }
                                else if (APP_COR_CTRY == "233")
                                {
                                    TxtCurrnCountry.Text = "UZBEKISTAN";
                                }
                                else if (APP_COR_CTRY == "237")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, BRITISH";
                                }
                                else if (APP_COR_CTRY == "238")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, U.S.";
                                }
                                else if (APP_COR_CTRY == "239")
                                {
                                    TxtCurrnCountry.Text = "WALLIS AND FUTUNA";
                                }
                                else if (APP_COR_CTRY == "240")
                                {
                                    TxtCurrnCountry.Text = "WESTERN SAHARA";
                                }
                               
                            }
                            else if (childNode.Name == "APP_OFF_NO")
                            {
                                TxtOffNo .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RES_NO")
                            {
                                TxtResNo .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_MOB_NO")
                            {
                                TxtMobNo .Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FAX_NO")
                            {
                                TxtFaxNo .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_EMAIL")
                            {
                                TxtEmailId .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD_PROOF")
                            {
                                string APP_COR_ADD_PROOF = childNode.InnerText;

                                if (APP_COR_ADD_PROOF == "01")
                                {
                                    TxtCurAddrProof.Text = "Passport";
                                }
                                else if (APP_COR_ADD_PROOF == "06")
                                {
                                    TxtCurAddrProof.Text = "Voter Identity Card";
                                }
                                else if (APP_COR_ADD_PROOF == "07")
                                {
                                    TxtCurAddrProof.Text = "Ration Card";
                                }
                                else if (APP_COR_ADD_PROOF == "08")
                                {
                                    TxtCurAddrProof.Text = "Registered Lease / Sale Agreement of Residence";
                                }
                                else if (APP_COR_ADD_PROOF == "02")
                                {
                                    TxtCurAddrProof.Text = "Driving License";
                                }
                                else if (APP_COR_ADD_PROOF == "13")
                                {
                                    TxtCurAddrProof.Text = "Flat Maintenance Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "14")
                                {
                                    TxtCurAddrProof.Text = "Insurance copy";
                                }
                                else if (APP_COR_ADD_PROOF == "09")
                                {
                                    TxtCurAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "10")
                                {
                                    TxtCurAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "11")
                                {
                                    TxtCurAddrProof.Text = "Gas Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "03")
                                {
                                    TxtCurAddrProof.Text = "Latest Bank Passbook";
                                }
                                else if (APP_COR_ADD_PROOF == "04")
                                {
                                    TxtCurAddrProof.Text = "Latest Bank Account Statement";
                                }
                                else if (APP_COR_ADD_PROOF == "15")
                                {
                                    TxtCurAddrProof.Text = "Self Declaration by High Court / Supreme Court Judge";
                                }
                                else if (APP_COR_ADD_PROOF == "17")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Scheduled Commercial Banks / Scheduled Co-operative Banks / Multinational Foreign banks.";
                                }
                                else if (APP_COR_ADD_PROOF == "22")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Gazetted Officer";
                                }
                                else if (APP_COR_ADD_PROOF == "21")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Notary Public";
                                }
                                else if (APP_COR_ADD_PROOF == "18")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Elected representatives to the Legislative Assembly";
                                }
                                else if (APP_COR_ADD_PROOF == "19")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Parliament";
                                }
                                else if (APP_COR_ADD_PROOF == "12")
                                {
                                    TxtCurAddrProof.Text = "Registration Certificate issued under Shops and Establishments Act";
                                }
                                else if (APP_COR_ADD_PROOF == "20")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by any Government / Statutory Authority";
                                }
                                else if (APP_COR_ADD_PROOF == "23")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Central / State Government ";
                                }
                                else if (APP_COR_ADD_PROOF == "24")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Statutory / Regulatory Authorities";
                                }
                                else if (APP_COR_ADD_PROOF == "25")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Public Sector Undertakings";
                                }
                                else if (APP_COR_ADD_PROOF == "26")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Scheduled Commercial Banks";
                                }
                                else if (APP_COR_ADD_PROOF == "27")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Public Financial Institutions";
                                }
                                else if (APP_COR_ADD_PROOF == "28")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Colleges affiliated to universities";
                                }
                                else if (APP_COR_ADD_PROOF == "29")
                                {
                                    TxtCurAddrProof.Text = "ID Card issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council, etc. to their Members";
                                }
                                else if (APP_COR_ADD_PROOF == "16")
                                {
                                    TxtCurAddrProof.Text = "Power of Attorney given by FII/sub-account to the Custodians (which are duly notarised and/or apostiled or consularised) giving registered address.";
                                }
                                else if (APP_COR_ADD_PROOF == "31")
                                {
                                    TxtCurAddrProof.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_COR_ADD_PROOF == "05")
                                {
                                    TxtCurAddrProof.Text = "Latest Demat Account Statement";
                                }
                                
                            }
                            else if (childNode.Name == "APP_COR_ADD_REF")
                            {
                                TxtCurrAddrRef .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD_DT")
                            {
                                TxtCurrAddrDt .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD_FLAG")
                            {
                                string APP_PER_ADD_FLAG = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD1")
                            {
                                TxtPerAddr1 .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD2")
                            {
                                TxtPerAddr2 .Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD3")
                            {
                                TxtPrtAddr3 .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_CITY")
                            {
                                TxtPerCity .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_PINCD")
                            {
                                TxtPerAddrPincode .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_STATE")
                            {
                                string APP_PER_STATE = childNode.InnerText;

                                if (APP_PER_STATE == "035")
                                {
                                    TxtPerAddrState.Text = "Andaman & Nicobar Islands";
                                }
                                else if (APP_PER_STATE == "028")
                                {
                                    TxtPerAddrState.Text = "Andhra Pradesh";
                                }
                                else if (APP_PER_STATE == "012")
                                {
                                    TxtPerAddrState.Text = "Arunachal Pradesh";
                                }
                                else if (APP_PER_STATE == "013")
                                {
                                    TxtPerAddrState.Text = "Assam";
                                }
                                else if (APP_PER_STATE == "010")
                                {
                                    TxtPerAddrState.Text = "Bihar";
                                }
                                else if (APP_PER_STATE == "004")
                                {
                                    TxtPerAddrState.Text = "Chandigarh";
                                }
                                else if (APP_PER_STATE == "026")
                                {
                                    TxtPerAddrState.Text = "Dadra & Nagar Haveli";
                                }
                                else if (APP_PER_STATE == "025")
                                {
                                    TxtPerAddrState.Text = "Daman & Diu";
                                }
                                else if (APP_PER_STATE == "007")
                                {
                                    TxtPerAddrState.Text = "Delhi";
                                }
                                else if (APP_PER_STATE == "030")
                                {
                                    TxtPerAddrState.Text = "Goa";
                                }
                                else if (APP_PER_STATE == "024")
                                {
                                    TxtPerAddrState.Text = "Gujarat";
                                }
                                else if (APP_PER_STATE == "006")
                                {
                                    TxtPerAddrState.Text = "Haryana";
                                }
                                else if (APP_PER_STATE == "002")
                                {
                                    TxtPerAddrState.Text = "Himachal Pradesh";
                                }
                                else if (APP_PER_STATE == "001")
                                {
                                    TxtPerAddrState.Text = "Jammu & Kashmir";
                                }
                                else if (APP_PER_STATE == "029")
                                {
                                    TxtPerAddrState.Text = "Karnataka";
                                }
                                else if (APP_PER_STATE == "032")
                                {
                                    TxtPerAddrState.Text = "Kerala";
                                }
                                else if (APP_PER_STATE == "031")
                                {
                                    TxtPerAddrState.Text = "Lakhswadeep";
                                }
                                else if (APP_PER_STATE == "023")
                                {
                                    TxtPerAddrState.Text = "Madhya Pradesh";
                                }
                                else if (APP_PER_STATE == "027")
                                {
                                    TxtPerAddrState.Text = "Maharashtra";
                                }
                                else if (APP_PER_STATE == "014")
                                {
                                    TxtPerAddrState.Text = "Manipur";
                                }
                                else if (APP_PER_STATE == "017")
                                {
                                    TxtPerAddrState.Text = "Meghalaya";
                                }
                                else if (APP_PER_STATE == "015")
                                {
                                    TxtPerAddrState.Text = "Mizoram";
                                }
                                else if (APP_PER_STATE == "018")
                                {
                                    TxtPerAddrState.Text = "Nagaland";
                                }
                                else if (APP_PER_STATE == "021")
                                {
                                    TxtPerAddrState.Text = "Orissa";
                                }
                                else if (APP_PER_STATE == "034")
                                {
                                    TxtPerAddrState.Text = "Pondicherry";
                                }
                                else if (APP_PER_STATE == "003")
                                {
                                    TxtPerAddrState.Text = "Punjab";
                                }
                                else if (APP_PER_STATE == "008")
                                {
                                    TxtPerAddrState.Text = "Rajasthan";
                                }
                                else if (APP_PER_STATE == "011")
                                {
                                    TxtPerAddrState.Text = "Sikkim";
                                }
                                else if (APP_PER_STATE == "033")
                                {
                                    TxtPerAddrState.Text = "Tamil Nadu";
                                }
                                else if (APP_PER_STATE == "016")
                                {
                                    TxtPerAddrState.Text = "Tripura";
                                }
                                else if (APP_PER_STATE == "009")
                                {
                                    TxtPerAddrState.Text = "Uttar Pradesh";
                                }
                                else if (APP_PER_STATE == "019")
                                {
                                    TxtPerAddrState.Text = "West Bengal";
                                }
                                else if (APP_PER_STATE == "022")
                                {
                                    TxtPerAddrState.Text = "Chhattisgarh";
                                }
                                else if (APP_PER_STATE == "005")
                                {
                                    TxtPerAddrState.Text = "Uttaranchal";
                                }
                                else if (APP_PER_STATE == "020")
                                {
                                    TxtPerAddrState.Text = "Jharkhand";
                                }
                                else if (APP_PER_STATE == "037")
                                {
                                    TxtPerAddrState.Text = "Telangana";
                                }
                                else if (APP_PER_STATE == "099")
                                {
                                    TxtPerAddrState.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_PER_CTRY")
                            {
                                string APP_PER_CTRY = childNode.InnerText;

                                if (APP_PER_CTRY == "001")
                                {
                                    TxtCurrnCountry.Text = "AFGHANISTAN";
                                }
                                else if (APP_PER_CTRY == "003")
                                {
                                    TxtCurrnCountry.Text = "ALBENIA";
                                }
                                else if (APP_PER_CTRY == "004")
                                {
                                    TxtCurrnCountry.Text = "ALGERIA";
                                }
                                else if (APP_PER_CTRY == "007")
                                {
                                    TxtCurrnCountry.Text = "ANGOLA";
                                }
                                else if (APP_PER_CTRY == "011")
                                {
                                    TxtCurrnCountry.Text = "ARGENTINA";
                                }
                                else if (APP_PER_CTRY == "012")
                                {
                                    TxtCurrnCountry.Text = "ARMENIA";
                                }
                                else if (APP_PER_CTRY == "013")
                                {
                                    TxtCurrnCountry.Text = "ARUBA";
                                }
                                else if (APP_PER_CTRY == "014")
                                {
                                    TxtCurrnCountry.Text = "AUSTRALIA";
                                }
                                else if (APP_PER_CTRY == "015")
                                {
                                    TxtCurrnCountry.Text = "AUSTRIA";
                                }
                                else if (APP_PER_CTRY == "016")
                                {
                                    TxtCurrnCountry.Text = "AZARBAIJAN";
                                }
                                else if (APP_PER_CTRY == "017")
                                {
                                    TxtCurrnCountry.Text = "BAHAMAS";
                                }
                                else if (APP_PER_CTRY == "018")
                                {
                                    TxtCurrnCountry.Text = "BAHRAIN";
                                }
                                else if (APP_PER_CTRY == "019")
                                {
                                    TxtCurrnCountry.Text = "BANGLADESH";
                                }
                                else if (APP_PER_CTRY == "020")
                                {
                                    TxtCurrnCountry.Text = "BARBADOS";
                                }
                                else if (APP_PER_CTRY == "021")
                                {
                                    TxtCurrnCountry.Text = "BELARUS";
                                }
                                else if (APP_PER_CTRY == "022")
                                {
                                    TxtCurrnCountry.Text = "BELGIUM";
                                }
                                else if (APP_PER_CTRY == "023")
                                {
                                    TxtCurrnCountry.Text = "BELIZE";
                                }
                                else if (APP_PER_CTRY == "024")
                                {
                                    TxtCurrnCountry.Text = "BENIN";
                                }
                                else if (APP_PER_CTRY == "025")
                                {
                                    TxtCurrnCountry.Text = "BERMUDA";
                                }
                                else if (APP_PER_CTRY == "026")
                                {
                                    TxtCurrnCountry.Text = "BHUTAN";
                                }
                                else if (APP_PER_CTRY == "027")
                                {
                                    TxtCurrnCountry.Text = "BOLIVIAN";
                                }
                                else if (APP_PER_CTRY == "028")
                                {
                                    TxtCurrnCountry.Text = "BOSNIA-HERZEGOVINA";
                                }
                                else if (APP_PER_CTRY == "029")
                                {
                                    TxtCurrnCountry.Text = "BOTSWANA";
                                }
                                else if (APP_PER_CTRY == "031")
                                {
                                    TxtCurrnCountry.Text = "BRAZIL";
                                }
                                else if (APP_PER_CTRY == "25")
                                {
                                    TxtCurrnCountry.Text = "BRUNEI";
                                }
                                else if (APP_PER_CTRY == "034")
                                {
                                    TxtCurrnCountry.Text = "BULGARIA";
                                }
                                else if (APP_PER_CTRY == "035")
                                {
                                    TxtCurrnCountry.Text = "BURKINA FASO";
                                }
                                else if (APP_PER_CTRY == "036")
                                {
                                    TxtCurrnCountry.Text = "BURUNDI";
                                }
                                else if (APP_PER_CTRY == "")
                                {
                                    TxtCurrnCountry.Text = "CAMEROON REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "039")
                                {
                                    TxtCurrnCountry.Text = "CANADA";
                                }
                                else if (APP_PER_CTRY == "040")
                                {
                                    TxtCurrnCountry.Text = "CAPE VERDE";
                                }
                                else if (APP_PER_CTRY == "041")
                                {
                                    TxtCurrnCountry.Text = "CAYMAN ISLANDS";
                                }
                                else if (APP_PER_CTRY == "042")
                                {
                                    TxtCurrnCountry.Text = "CENTRAL AFRICAN REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "043")
                                {
                                    TxtCurrnCountry.Text = "CHAD";
                                }
                                else if (APP_PER_CTRY == "044")
                                {
                                    TxtCurrnCountry.Text = "CHILE";
                                }
                                else if (APP_PER_CTRY == "045")
                                {
                                    TxtCurrnCountry.Text = "CHINA";
                                }
                                else if (APP_PER_CTRY == "048")
                                {
                                    TxtCurrnCountry.Text = "COLOMBIA";
                                }
                                else if (APP_PER_CTRY == "037")
                                {
                                    TxtCurrnCountry.Text = "COMBODIA";
                                }
                                else if (APP_PER_CTRY == "038")
                                {
                                    TxtCurrnCountry.Text = "COMOROS";
                                }
                                else if (APP_PER_CTRY == "050")
                                {
                                    TxtCurrnCountry.Text = "CONGO";
                                }
                                else if (APP_PER_CTRY == "052")
                                {
                                    TxtCurrnCountry.Text = "COOK ISLANDS";
                                }
                                else if (APP_PER_CTRY == "053")
                                {
                                    TxtCurrnCountry.Text = "COSTA RICA";
                                }
                                else if (APP_PER_CTRY == "054")
                                {
                                    TxtCurrnCountry.Text = "COTE D'IVOIRE";
                                }
                                else if (APP_PER_CTRY == "055")
                                {
                                    TxtCurrnCountry.Text = "CROATIA";
                                }
                                else if (APP_PER_CTRY == "056")
                                {
                                    TxtCurrnCountry.Text = "CUBA";
                                }
                                else if (APP_PER_CTRY == "057")
                                {
                                    TxtCurrnCountry.Text = "CYPRUS";
                                }
                                else if (APP_PER_CTRY == "058")
                                {
                                    TxtCurrnCountry.Text = "CZECH REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "059")
                                {
                                    TxtCurrnCountry.Text = "DENMARK";
                                }
                                else if (APP_PER_CTRY == "060")
                                {
                                    TxtCurrnCountry.Text = "DJIBOUTI";
                                }
                                else if (APP_PER_CTRY == "061")
                                {
                                    TxtCurrnCountry.Text = "DOMINICA";
                                }
                                else if (APP_PER_CTRY == "062")
                                {
                                    TxtCurrnCountry.Text = "DOMINICAN REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "063")
                                {
                                    TxtCurrnCountry.Text = "ECUADOR";
                                }
                                else if (APP_PER_CTRY == "064")
                                {
                                    TxtCurrnCountry.Text = "EGYPT";
                                }
                                else if (APP_PER_CTRY == "065")
                                {
                                    TxtCurrnCountry.Text = "EL SALVADOR";
                                }
                                else if (APP_PER_CTRY == "066")
                                {
                                    TxtCurrnCountry.Text = "EQUATORIAL GUINEA";
                                }
                                else if (APP_PER_CTRY == "068")
                                {
                                    TxtCurrnCountry.Text = "ESTONIA";
                                }
                                else if (APP_PER_CTRY == "069")
                                {
                                    TxtCurrnCountry.Text = "ETHIOPIA";
                                }
                                else if (APP_PER_CTRY == "072")
                                {
                                    TxtCurrnCountry.Text = "FIJI";
                                }
                                else if (APP_PER_CTRY == "073")
                                {
                                    TxtCurrnCountry.Text = "FINLAND";
                                }
                                else if (APP_PER_CTRY == "074")
                                {
                                    TxtCurrnCountry.Text = "FRANCE";
                                }
                                else if (APP_PER_CTRY == "075")
                                {
                                    TxtCurrnCountry.Text = "FRENCH GUIANA";
                                }
                                else if (APP_PER_CTRY == "076")
                                {
                                    TxtCurrnCountry.Text = "FRENCH POLYNESIA";
                                }
                                else if (APP_PER_CTRY == "078")
                                {
                                    TxtCurrnCountry.Text = "GABON";
                                }
                                else if (APP_PER_CTRY == "079")
                                {
                                    TxtCurrnCountry.Text = "GAMBIA";
                                }
                                else if (APP_PER_CTRY == "080")
                                {
                                    TxtCurrnCountry.Text = "GEORGIA";
                                }
                                else if (APP_PER_CTRY == "081")
                                {
                                    TxtCurrnCountry.Text = "GERMANY";
                                }
                                else if (APP_PER_CTRY == "082")
                                {
                                    TxtCurrnCountry.Text = "GHANA";
                                }
                                else if (APP_PER_CTRY == "083")
                                {
                                    TxtCurrnCountry.Text = "GIBRALTOR";
                                }
                                else if (APP_PER_CTRY == "084")
                                {
                                    TxtCurrnCountry.Text = "GREECE";
                                }
                                else if (APP_PER_CTRY == "085")
                                {
                                    TxtCurrnCountry.Text = "GREENLAND";
                                }
                                else if (APP_PER_CTRY == "086")
                                {
                                    TxtCurrnCountry.Text = "GRENADA";
                                }
                                else if (APP_PER_CTRY == "087")
                                {
                                    TxtCurrnCountry.Text = "GUADELOUPE";
                                }
                                else if (APP_PER_CTRY == "088")
                                {
                                    TxtCurrnCountry.Text = "GUAM";
                                }
                                else if (APP_PER_CTRY == "089")
                                {
                                    TxtCurrnCountry.Text = "GUATEMALA";
                                }
                                else if (APP_PER_CTRY == "090")
                                {
                                    TxtCurrnCountry.Text = "GUERNSEY";
                                }
                                else if (APP_PER_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_PER_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_PER_CTRY == "092")
                                {
                                    TxtCurrnCountry.Text = "GUINEA-BISSAU";
                                }
                                else if (APP_PER_CTRY == "093")
                                {
                                    TxtCurrnCountry.Text = "GUYANA";
                                }
                                else if (APP_PER_CTRY == "094")
                                {
                                    TxtCurrnCountry.Text = "HAITI";
                                }
                                else if (APP_PER_CTRY == "097")
                                {
                                    TxtCurrnCountry.Text = "HONDURAS";
                                }
                                else if (APP_PER_CTRY == "098")
                                {
                                    TxtCurrnCountry.Text = "HONGKONG";
                                }
                                else if (APP_PER_CTRY == "100")
                                {
                                    TxtCurrnCountry.Text = "ICELAND";
                                }
                                else if (APP_PER_CTRY == "101")
                                {
                                    TxtCurrnCountry.Text = "INDIA";
                                }
                                else if (APP_PER_CTRY == "102")
                                {
                                    TxtCurrnCountry.Text = "INDONESIA";
                                }
                                else if (APP_PER_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN";
                                }
                                else if (APP_PER_CTRY == "104")
                                {
                                    TxtCurrnCountry.Text = "IRAQ";
                                }
                                else if (APP_PER_CTRY == "105")
                                {
                                    TxtCurrnCountry.Text = "IRELAND";
                                }
                                else if (APP_PER_CTRY == "107")
                                {
                                    TxtCurrnCountry.Text = "ISRAEL";
                                }
                                else if (APP_PER_CTRY == "108")
                                {
                                    TxtCurrnCountry.Text = "ITALY";
                                }
                                else if (APP_PER_CTRY == "109")
                                {
                                    TxtCurrnCountry.Text = "JAMAICA";
                                }
                                else if (APP_PER_CTRY == "110")
                                {
                                    TxtCurrnCountry.Text = "JAPAN";
                                }
                                else if (APP_PER_CTRY == "112")
                                {
                                    TxtCurrnCountry.Text = "JORDAN";
                                }
                                else if (APP_PER_CTRY == "113")
                                {
                                    TxtCurrnCountry.Text = "KAZAKSTAN";
                                }
                                else if (APP_PER_CTRY == "114")
                                {
                                    TxtCurrnCountry.Text = "KENYA";
                                }
                                else if (APP_PER_CTRY == "118")
                                {
                                    TxtCurrnCountry.Text = "KUWAIT";
                                }
                                else if (APP_PER_CTRY == "119")
                                {
                                    TxtCurrnCountry.Text = "KYRGYZSTAN";
                                }
                                else if (APP_PER_CTRY == "121")
                                {
                                    TxtCurrnCountry.Text = "LATVIA";
                                }
                                else if (APP_PER_CTRY == "122")
                                {
                                    TxtCurrnCountry.Text = "LEBANON";
                                }
                                else if (APP_PER_CTRY == "123")
                                {
                                    TxtCurrnCountry.Text = "LESOTHO";
                                }
                                else if (APP_PER_CTRY == "124")
                                {
                                    TxtCurrnCountry.Text = "LIBERIA";
                                }
                                else if (APP_PER_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYA";
                                }
                                else if (APP_PER_CTRY == "127")
                                {
                                    TxtCurrnCountry.Text = "LITHUANIA";
                                }
                                else if (APP_PER_CTRY == "128")
                                {
                                    TxtCurrnCountry.Text = "LUXEMBOURG";
                                }
                                else if (APP_PER_CTRY == "129")
                                {
                                    TxtCurrnCountry.Text = "MACAU";
                                }
                                else if (APP_PER_CTRY == "130")
                                {
                                    TxtCurrnCountry.Text = "MACEDONIA";
                                }
                                else if (APP_PER_CTRY == "131")
                                {
                                    TxtCurrnCountry.Text = "MADAGASCAR";
                                }
                                else if (APP_PER_CTRY == "132")
                                {
                                    TxtCurrnCountry.Text = "MALAWI";
                                }
                                else if (APP_PER_CTRY == "133")
                                {
                                    TxtCurrnCountry.Text = "MALAYSIA";
                                }
                                else if (APP_PER_CTRY == "134")
                                {
                                    TxtCurrnCountry.Text = "MALDIVES";
                                }
                                else if (APP_PER_CTRY == "135")
                                {
                                    TxtCurrnCountry.Text = "MALI";
                                }
                                else if (APP_PER_CTRY == "136")
                                {
                                    TxtCurrnCountry.Text = "MALTA";
                                }
                                else if (APP_PER_CTRY == "139")
                                {
                                    TxtCurrnCountry.Text = "MAURITANIA";
                                }
                                else if (APP_PER_CTRY == "140")
                                {
                                    TxtCurrnCountry.Text = "MAURITIUS";
                                }
                                else if (APP_PER_CTRY == "142")
                                {
                                    TxtCurrnCountry.Text = "MEXICO";
                                }
                                else if (APP_PER_CTRY == "144")
                                {
                                    TxtCurrnCountry.Text = "MOLDOVA";
                                }
                                else if (APP_PER_CTRY == "146")
                                {
                                    TxtCurrnCountry.Text = "MONGOLIA";
                                }
                                else if (APP_PER_CTRY == "147")
                                {
                                    TxtCurrnCountry.Text = "MONTSERRAT";
                                }
                                else if (APP_PER_CTRY == "148")
                                {
                                    TxtCurrnCountry.Text = "MOROCCA";
                                }
                                else if (APP_PER_CTRY == "149")
                                {
                                    TxtCurrnCountry.Text = "MOZAMBIQUE";
                                }
                                else if (APP_PER_CTRY == "150")
                                {
                                    TxtCurrnCountry.Text = "MYANMAR";
                                }
                                else if (APP_PER_CTRY == "151")
                                {
                                    TxtCurrnCountry.Text = "NAMIBIA";
                                }
                                else if (APP_PER_CTRY == "153")
                                {
                                    TxtCurrnCountry.Text = "NEPAL";
                                }
                                else if (APP_PER_CTRY == "154")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS";
                                }
                                else if (APP_PER_CTRY == "155")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS ANTILLES";
                                }
                                else if (APP_PER_CTRY == "156")
                                {
                                    TxtCurrnCountry.Text = "NEW CALEDONIA";
                                }
                                else if (APP_PER_CTRY == "157")
                                {
                                    TxtCurrnCountry.Text = "NEW ZEALAND";
                                }
                                else if (APP_PER_CTRY == "158")
                                {
                                    TxtCurrnCountry.Text = "NICARAGUA";
                                }
                                else if (APP_PER_CTRY == "159")
                                {
                                    TxtCurrnCountry.Text = "NIGER";
                                }
                                else if (APP_PER_CTRY == "160")
                                {
                                    TxtCurrnCountry.Text = "NIGERIA";
                                }
                                else if (APP_PER_CTRY == "116")
                                {
                                    TxtCurrnCountry.Text = "NORTH KOREA";
                                }
                                else if (APP_PER_CTRY == "164")
                                {
                                    TxtCurrnCountry.Text = "NORWAY";
                                }
                                else if (APP_PER_CTRY == "165")
                                {
                                    TxtCurrnCountry.Text = "OMAN";
                                }
                                else if (APP_PER_CTRY == "166")
                                {
                                    TxtCurrnCountry.Text = "PAKISTAN";
                                }
                                else if (APP_PER_CTRY == "169")
                                {
                                    TxtCurrnCountry.Text = "PANAMA";
                                }
                                else if (APP_PER_CTRY == "170")
                                {
                                    TxtCurrnCountry.Text = "PAPUA NEW GUINEA";
                                }
                                else if (APP_PER_CTRY == "171")
                                {
                                    TxtCurrnCountry.Text = "PARAGUAY";
                                }
                                else if (APP_PER_CTRY == "172")
                                {
                                    TxtCurrnCountry.Text = "PERU";
                                }
                                else if (APP_PER_CTRY == "173")
                                {
                                    TxtCurrnCountry.Text = "PHILIPPINES";
                                }
                                else if (APP_PER_CTRY == "175")
                                {
                                    TxtCurrnCountry.Text = "POLAND";
                                }
                                else if (APP_PER_CTRY == "176")
                                {
                                    TxtCurrnCountry.Text = "PORTUGAL";
                                }
                                else if (APP_PER_CTRY == "178")
                                {
                                    TxtCurrnCountry.Text = "QATAR";
                                }
                                else if (APP_PER_CTRY == "180")
                                {
                                    TxtCurrnCountry.Text = "ROMANIA";
                                }
                                else if (APP_PER_CTRY == "181")
                                {
                                    TxtCurrnCountry.Text = "RUSSIA";
                                }
                                else if (APP_PER_CTRY == "182")
                                {
                                    TxtCurrnCountry.Text = "RWANDA";
                                }
                                else if (APP_PER_CTRY == "191")
                                {
                                    TxtCurrnCountry.Text = "SAUDI ARABIA";
                                }
                                else if (APP_PER_CTRY == "192")
                                {
                                    TxtCurrnCountry.Text = "SENEGAL";
                                }
                                else if (APP_PER_CTRY == "194")
                                {
                                    TxtCurrnCountry.Text = "SEYCHELLES";
                                }
                                else if (APP_PER_CTRY == "196")
                                {
                                    TxtCurrnCountry.Text = "SINGAPORE";
                                }
                                else if (APP_PER_CTRY == "197")
                                {
                                    TxtCurrnCountry.Text = "SLOVAKIA";
                                }
                                else if (APP_PER_CTRY == "198")
                                {
                                    TxtCurrnCountry.Text = "SLOVENIA";
                                }
                                else if (APP_PER_CTRY == "199")
                                {
                                    TxtCurrnCountry.Text = "SOLOMON ISLANDS";
                                }
                                else if (APP_PER_CTRY == "200")
                                {
                                    TxtCurrnCountry.Text = "SOMALIA";
                                }
                                else if (APP_PER_CTRY == "201")
                                {
                                    TxtCurrnCountry.Text = "SOUTH AFRICA";
                                }
                                else if (APP_PER_CTRY == "117")
                                {
                                    TxtCurrnCountry.Text = "SOUTH KOREA";
                                }
                                else if (APP_PER_CTRY == "203")
                                {
                                    TxtCurrnCountry.Text = "SPAIN";
                                }
                                else if (APP_PER_CTRY == "204")
                                {
                                    TxtCurrnCountry.Text = "SRI LANKA";
                                }
                                else if (APP_PER_CTRY == "205")
                                {
                                    TxtCurrnCountry.Text = "SUDAN";
                                }
                                else if (APP_PER_CTRY == "206")
                                {
                                    TxtCurrnCountry.Text = "SURINAME";
                                }
                                else if (APP_PER_CTRY == "208")
                                {
                                    TxtCurrnCountry.Text = "SWAZILAND";
                                }
                                else if (APP_PER_CTRY == "209")
                                {
                                    TxtCurrnCountry.Text = "SWEDEN";
                                }
                                else if (APP_PER_CTRY == "210")
                                {
                                    TxtCurrnCountry.Text = "SWITZERLAND";
                                }
                                else if (APP_PER_CTRY == "212")
                                {
                                    TxtCurrnCountry.Text = "TAIWAN";
                                }
                                else if (APP_PER_CTRY == "213")
                                {
                                    TxtCurrnCountry.Text = "TAJIKISTHAN";
                                }
                                else if (APP_PER_CTRY == "214")
                                {
                                    TxtCurrnCountry.Text = "TANZANIA";
                                }
                                else if (APP_PER_CTRY == "215")
                                {
                                    TxtCurrnCountry.Text = "THAILAND";
                                }
                                else if (APP_PER_CTRY == "217")
                                {
                                    TxtCurrnCountry.Text = "TOGO REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "219")
                                {
                                    TxtCurrnCountry.Text = "TONGA";
                                }
                                else if (APP_PER_CTRY == "220")
                                {
                                    TxtCurrnCountry.Text = "TRINIDAD AND TOBAGO";
                                }
                                else if (APP_PER_CTRY == "221")
                                {
                                    TxtCurrnCountry.Text = "TUNISIA";
                                }
                                else if (APP_PER_CTRY == "222")
                                {
                                    TxtCurrnCountry.Text = "TURKEY";
                                }
                                else if (APP_PER_CTRY == "223")
                                {
                                    TxtCurrnCountry.Text = "TURKMENISTAN";
                                }
                                else if (APP_PER_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "U A E";
                                }
                                else if (APP_PER_CTRY == "226")
                                {
                                    TxtCurrnCountry.Text = "UGANDA";
                                }
                                else if (APP_PER_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "UNITED ARAB EMIRATES";
                                }
                                else if (APP_PER_CTRY == "229")
                                {
                                    TxtCurrnCountry.Text = "UNITED KINGDOM";
                                }
                                else if (APP_PER_CTRY == "232")
                                {
                                    TxtCurrnCountry.Text = "URUGUAY";
                                }
                                else if (APP_PER_CTRY == "230")
                                {
                                    TxtCurrnCountry.Text = "USA";
                                }
                                else if (APP_PER_CTRY == "234")
                                {
                                    TxtCurrnCountry.Text = "VANUATU";
                                }
                                else if (APP_PER_CTRY == "235")
                                {
                                    TxtCurrnCountry.Text = "VENEZUELA";
                                }
                                else if (APP_PER_CTRY == "236")
                                {
                                    TxtCurrnCountry.Text = "VIETNAM";
                                }
                                else if (APP_PER_CTRY == "241")
                                {
                                    TxtCurrnCountry.Text = "YEMEN";
                                }
                                else if (APP_PER_CTRY == "242")
                                {
                                    TxtCurrnCountry.Text = "ZAMBIA";
                                }
                                else if (APP_PER_CTRY == "243")
                                {
                                    TxtCurrnCountry.Text = "ZIMBABWE";
                                }
                                else if (APP_PER_CTRY == "002")
                                {
                                    TxtCurrnCountry.Text = "ALAND ISLANDS";
                                }
                                else if (APP_PER_CTRY == "005")
                                {
                                    TxtCurrnCountry.Text = "AMERICAN SAMOA";
                                }
                                else if (APP_PER_CTRY == "006")
                                {
                                    TxtCurrnCountry.Text = "ANDORRA";
                                }
                                else if (APP_PER_CTRY == "008")
                                {
                                    TxtCurrnCountry.Text = "ANGUILLA";
                                }
                                else if (APP_PER_CTRY == "009")
                                {
                                    TxtCurrnCountry.Text = "ANTARCTICA";
                                }
                                else if (APP_PER_CTRY == "010")
                                {
                                    TxtCurrnCountry.Text = "ANTIGUA AND BARBUDA";
                                }
                                else if (APP_PER_CTRY == "030")
                                {
                                    TxtCurrnCountry.Text = "BOUVET ISLAND";
                                }
                                else if (APP_PER_CTRY == "032")
                                {
                                    TxtCurrnCountry.Text = "BRITISH INDIAN OCEAN TERRITORY";
                                }
                                else if (APP_PER_CTRY == "046")
                                {
                                    TxtCurrnCountry.Text = "CHRISTMAS ISLAND";
                                }
                                else if (APP_PER_CTRY == "047")
                                {
                                    TxtCurrnCountry.Text = "COCOS (KEELING) ISLANDS";
                                }
                                else if (APP_PER_CTRY == "067")
                                {
                                    TxtCurrnCountry.Text = "ERITREA";
                                }
                                else if (APP_PER_CTRY == "070")
                                {
                                    TxtCurrnCountry.Text = "FALKLAND ISLANDS (MALVINAS)";
                                }
                                else if (APP_PER_CTRY == "071")
                                {
                                    TxtCurrnCountry.Text = "FAROE ISLANDS";
                                }
                                else if (APP_PER_CTRY == "077")
                                {
                                    TxtCurrnCountry.Text = "FRENCH SOUTHERN TERRITORIES";
                                }
                                else if (APP_PER_CTRY == "095")
                                {
                                    TxtCurrnCountry.Text = "HEARD ISLAND AND MCDONALD ISLANDS";
                                }
                                else if (APP_PER_CTRY == "096")
                                {
                                    TxtCurrnCountry.Text = "HOLY SEE (VATICAN CITY STATE)";
                                }
                                else if (APP_PER_CTRY == "099")
                                {
                                    TxtCurrnCountry.Text = "HUNGARY";
                                }
                                else if (APP_PER_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN, ISLAMIC REPUBLIC OF";
                                }
                                else if (APP_PER_CTRY == "106")
                                {
                                    TxtCurrnCountry.Text = "ISLE OF MAN";
                                }
                                else if (APP_PER_CTRY == "115")
                                {
                                    TxtCurrnCountry.Text = "KIRIBATI";
                                }
                                else if (APP_PER_CTRY == "120")
                                {
                                    TxtCurrnCountry.Text = "LAO PEOPLE'S DEMOCRATIC REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYAN ARAB JAMAHIRIYA";
                                }
                                else if (APP_PER_CTRY == "126")
                                {
                                    TxtCurrnCountry.Text = "LIECHTENSTEIN";
                                }
                                else if (APP_PER_CTRY == "137")
                                {
                                    TxtCurrnCountry.Text = "MARSHALL ISLANDS";
                                }
                                else if (APP_PER_CTRY == "138")
                                {
                                    TxtCurrnCountry.Text = "MARTINIQUE";
                                }
                                else if (APP_PER_CTRY == "141")
                                {
                                    TxtCurrnCountry.Text = "MAYOTTE";
                                }
                                else if (APP_PER_CTRY == "143")
                                {
                                    TxtCurrnCountry.Text = "MICRONESIA, FEDERATED STATES OF";
                                }
                                else if (APP_PER_CTRY == "145")
                                {
                                    TxtCurrnCountry.Text = "MONACO";
                                }
                                else if (APP_PER_CTRY == "152")
                                {
                                    TxtCurrnCountry.Text = "NAURU";
                                }
                                else if (APP_PER_CTRY == "161")
                                {
                                    TxtCurrnCountry.Text = "NIUE";
                                }
                                else if (APP_PER_CTRY == "162")
                                {
                                    TxtCurrnCountry.Text = "NORFOLK ISLAND";
                                }
                                else if (APP_PER_CTRY == "163")
                                {
                                    TxtCurrnCountry.Text = "NORTHERN MARIANA ISLANDS";
                                }
                                else if (APP_PER_CTRY == "167")
                                {
                                    TxtCurrnCountry.Text = "PALAU";
                                }
                                else if (APP_PER_CTRY == "168")
                                {
                                    TxtCurrnCountry.Text = "PALESTINIAN TERRITORY, OCCUPIED";
                                }
                                else if (APP_PER_CTRY == "174")
                                {
                                    TxtCurrnCountry.Text = "PITCAIRN";
                                }
                                else if (APP_PER_CTRY == "177")
                                {
                                    TxtCurrnCountry.Text = "PUERTO RICO";
                                }
                                else if (APP_PER_CTRY == "179")
                                {
                                    TxtCurrnCountry.Text = "REUNION";
                                }
                                else if (APP_PER_CTRY == "183")
                                {
                                    TxtCurrnCountry.Text = "SAINT HELENA";
                                }
                                else if (APP_PER_CTRY == "184")
                                {
                                    TxtCurrnCountry.Text = "SAINT KITTS AND NEVIS";
                                }
                                else if (APP_PER_CTRY == "185")
                                {
                                    TxtCurrnCountry.Text = "SAINT LUCIA";
                                }
                                else if (APP_PER_CTRY == "186")
                                {
                                    TxtCurrnCountry.Text = "SAINT PIERRE AND MIQUELON";
                                }
                                else if (APP_PER_CTRY == "187")
                                {
                                    TxtCurrnCountry.Text = "SAINT VINCENT AND THE GRENADINES";
                                }
                                else if (APP_PER_CTRY == "188")
                                {
                                    TxtCurrnCountry.Text = "SAMOA";
                                }
                                else if (APP_PER_CTRY == "189")
                                {
                                    TxtCurrnCountry.Text = "SAN MARINO";
                                }
                                else if (APP_PER_CTRY == "190")
                                {
                                    TxtCurrnCountry.Text = "SAO TOME AND PRINCIPE";
                                }
                                else if (APP_PER_CTRY == "193")
                                {
                                    TxtCurrnCountry.Text = "SERBIA AND MONTENEGRO";
                                }
                                else if (APP_PER_CTRY == "195")
                                {
                                    TxtCurrnCountry.Text = "SIERRA LEONE";
                                }
                                else if (APP_PER_CTRY == "202")
                                {
                                    TxtCurrnCountry.Text = "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS";
                                }
                                else if (APP_PER_CTRY == "207")
                                {
                                    TxtCurrnCountry.Text = "SVALBARD AND JAN MAYEN";
                                }
                                else if (APP_PER_CTRY == "211")
                                {
                                    TxtCurrnCountry.Text = "SYRIAN ARAB REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "216")
                                {
                                    TxtCurrnCountry.Text = "TIMOR-LESTE";
                                }
                                else if (APP_PER_CTRY == "218")
                                {
                                    TxtCurrnCountry.Text = "TOKELAU";
                                }
                                else if (APP_PER_CTRY == "224")
                                {
                                    TxtCurrnCountry.Text = "TURKS AND CAICOS ISLANDS";
                                }
                                else if (APP_PER_CTRY == "225")
                                {
                                    TxtCurrnCountry.Text = "TUVALU";
                                }
                                else if (APP_PER_CTRY == "227")
                                {
                                    TxtCurrnCountry.Text = "UKRAINE";
                                }
                                else if (APP_PER_CTRY == "231")
                                {
                                    TxtCurrnCountry.Text = "UNITED STATES MINOR OUTLYING ISLANDS";
                                }
                                else if (APP_PER_CTRY == "233")
                                {
                                    TxtCurrnCountry.Text = "UZBEKISTAN";
                                }
                                else if (APP_PER_CTRY == "237")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, BRITISH";
                                }
                                else if (APP_PER_CTRY == "238")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, U.S.";
                                }
                                else if (APP_PER_CTRY == "239")
                                {
                                    TxtCurrnCountry.Text = "WALLIS AND FUTUNA";
                                }
                                else if (APP_PER_CTRY == "240")
                                {
                                    TxtCurrnCountry.Text = "WESTERN SAHARA";
                                }
                            }
                            else if (childNode.Name == "APP_PER_ADD_PROOF")
                            {
                                string APP_PER_ADD_PROOF = childNode.InnerText;

                                if (APP_PER_ADD_PROOF == "01")
                                {
                                    TxtPrmAddrProof.Text = "Passport";
                                }
                                else if (APP_PER_ADD_PROOF == "06")
                                {
                                    TxtPrmAddrProof.Text = "Voter Identity Card";
                                }
                                else if (APP_PER_ADD_PROOF == "07")
                                {
                                    TxtPrmAddrProof.Text = "Ration Card";
                                }
                                else if (APP_PER_ADD_PROOF == "08")
                                {
                                    TxtPrmAddrProof.Text = "Registered Lease / Sale Agreement of Residence";
                                }
                                else if (APP_PER_ADD_PROOF == "02")
                                {
                                    TxtPrmAddrProof.Text = "Driving License";
                                }
                                else if (APP_PER_ADD_PROOF == "13")
                                {
                                    TxtPrmAddrProof.Text = "Flat Maintenance Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "14")
                                {
                                    TxtPrmAddrProof.Text = "Insurance copy";
                                }
                                else if (APP_PER_ADD_PROOF == "09")
                                {
                                    TxtPrmAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "10")
                                {
                                    TxtPrmAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "11")
                                {
                                    TxtPrmAddrProof.Text = "Gas Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "03")
                                {
                                    TxtPrmAddrProof.Text = "Latest Bank Passbook";
                                }
                                else if (APP_PER_ADD_PROOF == "04")
                                {
                                    TxtPrmAddrProof.Text = "Latest Bank Account Statement";
                                }
                                else if (APP_PER_ADD_PROOF == "15")
                                {
                                    TxtPrmAddrProof.Text = "Self Declaration by High Court / Supreme Court Judge";
                                }
                                else if (APP_PER_ADD_PROOF == "17")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Scheduled Commercial Banks / Scheduled Co-operative Banks / Multinational Foreign banks.";
                                }
                                else if (APP_PER_ADD_PROOF == "22")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Gazetted Officer";
                                }
                                else if (APP_PER_ADD_PROOF == "21")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Notary Public";
                                }
                                else if (APP_PER_ADD_PROOF == "18")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Elected representatives to the Legislative Assembly";
                                }
                                else if (APP_PER_ADD_PROOF == "19")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Parliament";
                                }
                                else if (APP_PER_ADD_PROOF == "12")
                                {
                                    TxtPrmAddrProof.Text = "Registration Certificate issued under Shops and Establishments Act";
                                }
                                else if (APP_PER_ADD_PROOF == "20")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by any Government / Statutory Authority";
                                }
                                else if (APP_PER_ADD_PROOF == "23")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Central / State Government ";
                                }
                                else if (APP_PER_ADD_PROOF == "24")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Statutory / Regulatory Authorities";
                                }
                                else if (APP_PER_ADD_PROOF == "25")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Public Sector Undertakings";
                                }
                                else if (APP_PER_ADD_PROOF == "26")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Scheduled Commercial Banks";
                                }
                                else if (APP_PER_ADD_PROOF == "27")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Public Financial Institutions";
                                }
                                else if (APP_PER_ADD_PROOF == "28")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Colleges affiliated to universities";
                                }
                                else if (APP_PER_ADD_PROOF == "29")
                                {
                                    TxtPrmAddrProof.Text = "ID Card issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council, etc. to their Members";
                                }
                                else if (APP_PER_ADD_PROOF == "16")
                                {
                                    TxtPrmAddrProof.Text = "Power of Attorney given by FII/sub-account to the Custodians (which are duly notarised and/or apostiled or consularised) giving registered address.";
                                }
                                else if (APP_PER_ADD_PROOF == "31")
                                {
                                    TxtPrmAddrProof.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_PER_ADD_PROOF == "05")
                                {
                                    TxtPrmAddrProof.Text = "Latest Demat Account Statement";
                                }


                            }
                            else if (childNode.Name == "APP_PER_ADD_REF")
                            {
                                TxtPrmAddrRef .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD_DT")
                            {
                                TxtPerAddrDt .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INCOME")
                            {
                                string APP_INCOME = childNode.InnerText;

                                if (APP_INCOME=="01")
                                {
                                    TxtIncome.Text = "Below Rs. 1  Lac";
                                }
                                else if (APP_INCOME == "02")
                                {
                                    TxtIncome.Text = "Btw Rs. 1 to Rs. 5 Lacs";
                                }
                                else if (APP_INCOME == "03")
                                {
                                    TxtIncome.Text = "Btw Rs. 5 to Rs. 10 Lacs";
                                }
                                else if (APP_INCOME == "04")
                                {
                                    TxtIncome.Text = "Btw Rs. 10 to Rs. 25 Lacs";
                                }
                                else if (APP_INCOME == "05")
                                {
                                    TxtIncome.Text = "More than Rs. 25 Lacs";
                                }
                               
                            }
                            else if (childNode.Name == "APP_OCC")
                            {
                                string APP_OCC = childNode.InnerText;

                                if (APP_OCC=="02")
                                {
                                    TxtOcc.Text = "Public Sector";
                                }
                                else if (APP_OCC == "01")
                                {
                                    TxtOcc.Text = "Private Sector";
                                }
                                else if (APP_OCC == "10")
                                {
                                    TxtOcc.Text = "Government Service";
                                }
                                else if (APP_OCC == "03")
                                {
                                    TxtOcc.Text = "Business";
                                }
                                else if (APP_OCC == "04")
                                {
                                    TxtOcc.Text = "Professional";
                                }
                                else if (APP_OCC == "05")
                                {
                                    TxtOcc.Text = "Agriculturist";
                                }
                                else if (APP_OCC == "06")
                                {
                                    TxtOcc.Text = "Retired";
                                }
                                else if (APP_OCC == "07")
                                {
                                    TxtOcc.Text = "Housewife";
                                }
                                else if (APP_OCC == "08")
                                {
                                    TxtOcc.Text = "Student";
                                }
                                else if (APP_OCC == "99")
                                {
                                    TxtOcc.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_OCC")
                            {
                                TxtOthOcc .Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_POL_CONN")
                            {
                                string APP_POL_CONN = childNode.InnerText;

                                if (APP_POL_CONN == "00" || APP_POL_CONN=="NA")
                                {
                                    TxtPoExposedPerson.Text = "Not Applicable";
                                }
                                else if (APP_POL_CONN == "01" || APP_POL_CONN == "RPEP")
                                {
                                    TxtPoExposedPerson.Text = "PEP";
                                }
                                else if (APP_POL_CONN == "02" || APP_POL_CONN == "PEP")
                                {
                                    TxtPoExposedPerson.Text = " Related to a PEP";
                                }
                                
                            }
                            else if (childNode.Name == "APP_DOC_PROOF")
                            {
                                string APP_DOC_PROOF = childNode.InnerText;

                                if (APP_DOC_PROOF == "S")
                                {
                                    TxtDocProof.Text = "Self Certified Copies Submitted (Originals Verified)";
                                }
                                else if(APP_DOC_PROOF == "T")
                                {
                                    TxtDocProof.Text = "True Copies of Documents Received";
                                }
                            }
                            else if (childNode.Name == "APP_INTERNAL_REF")
                            {
                                TxtInternalRef .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_BRANCH_CODE")
                            {
                                TxtBranchCode .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_MAR_STATUS")
                            {
                                string APP_MAR_STATUS = childNode.InnerText;

                                if (APP_MAR_STATUS == "01")
                                {
                                    TxtMaritalStatus.Text = "Married";
                                }
                                else if (APP_MAR_STATUS == "02")
                                {
                                    TxtMaritalStatus.Text = "Unmarried";
                                }
                            }
                            else if (childNode.Name == "APP_NETWRTH")
                            {
                                TxtNetWrth .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_NETWORTH_DT")
                            {
                                TxtNetWrtDt .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INCORP_PLC")
                            {
                                TxtIncorpPlc .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHERINFO")
                            {
                                TxtOthrInfo .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_REMARKS")
                            {
                                TxtRemarks .Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER1")
                            {
                                TxtFiller1 .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER2")
                            {
                                TxtFiller2 .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER3")
                            {
                                TxtFiller3 .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_STATUS")
                            {
                                string APP_STATUS = childNode.InnerText;

                                if (APP_STATUS=="01" || APP_STATUS =="11")
                                {
                                    TxtStatus.Text = " UNDER_PROCESS";
                                }
                                else if (APP_STATUS == "02" || APP_STATUS =="12")
                                {
                                    TxtStatus.Text = " KYC REGISTERED ";
                                }
                                else if (APP_STATUS == "03" || APP_STATUS =="13")
                                {
                                    TxtStatus.Text = " ON HOLD ";
                                }
                                else if (APP_STATUS == "04")
                                {
                                    TxtStatus.Text = " KYC REJECTED";
                                }
                                else if (APP_STATUS == "05")
                                {
                                    TxtStatus.Text = "NOT AVAILABLE";
                                }
                                else if (APP_STATUS == "06")
                                {
                                    TxtStatus.Text = "Deactivate";
                                }
                                else if (APP_STATUS == "14")
                                {
                                    TxtStatus.Text = "KYC REJECTED ";
                                }
                                else if (APP_STATUS == "21")
                                {
                                    TxtStatus.Text = "Mutual Fund under process";
                                }
                                else if (APP_STATUS == "22")
                                {
                                    TxtStatus.Text = "Mutual Fund verified";
                                }
                            }
                            else if (childNode.Name == "APP_STATUSDT")
                            {
                                TxtStatusDt .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_ERROR_DESC")
                            {
                                TxtErrorDesc .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DUMP_TYPE")
                            {
                                string APP_DUMP_TYPE = childNode.InnerText;

                                if (APP_DUMP_TYPE == "I")
                                {
                                    TxtDumpType.Text = "Incremental";
                                }
                                else if (APP_DUMP_TYPE == "F")
                                {
                                    TxtDumpType.Text = "Full Download";
                                }
                                else if (APP_DUMP_TYPE == "P")
                                {
                                    TxtDumpType.Text = "Partial Download";
                                }
                                else if (APP_DUMP_TYPE == "E")
                                {
                                    TxtDumpType.Text = "EOD";
                                }
                                else if (APP_DUMP_TYPE == "S")
                                {
                                    TxtDumpType.Text = "Solicited";
                                }
                                else if (APP_DUMP_TYPE == "U")
                                {
                                    TxtDumpType.Text = "Unsolicited";
                                }
                            }
                            else if (childNode.Name == "APP_DNLDDT")
                            {
                                TxtDnldDt .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_KRA_INFO")
                            {
                                TxtKraInfo .Text  = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_KYC_MODE")
                            {
                                string APP_KYC_MODE = childNode.InnerText;
                                if (APP_KYC_MODE == "01" || APP_KYC_MODE == "1")
                                {
                                    TxtKycMode.Text = "New";
                                }
                                else if (APP_KYC_MODE == "02" || APP_KYC_MODE == "2")
                                {
                                    TxtKycMode.Text = "Modify with documents";
                                }
                                else if (APP_KYC_MODE == "03" || APP_KYC_MODE == "3")
                                {
                                    TxtKycMode.Text = "Modify without documents";
                                }
                                else if (APP_KYC_MODE == "04" || APP_KYC_MODE == "4")
                                {
                                    TxtKycMode.Text = "Delete";
                                }
                            }
                            else if (childNode.Name == "APP_VAULT_REF")
                            {
                                string APP_VAULT_REF = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_UID_TOKEN")
                            {
                                string APP_UID_TOKEN = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_VER_NO")
                            {
                                string APP_VER_NO = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_SIGN")
                            {
                                string APP_SIGN = childNode.InnerText;
                            }
                        }
                    }

                    //Added By Mohan on 29-11-2021
                    foreach (XmlNode xmlNode in OutPut_doc.GetElementsByTagName("KYC_DATA"))
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "APP_IOP_FLG")
                            {
                                string APP_IOP_FLG = childNode.InnerText;
                                if (APP_IOP_FLG == "RI")
                                {
                                    TxtIopFlag.Text = "Registrar Modification";
                                }
                                else if (APP_IOP_FLG == "RS")
                                {
                                    TxtIopFlag.Text = "Registrar Download/Fetch";
                                }
                                else if (APP_IOP_FLG == "RE")
                                {
                                    TxtIopFlag.Text = "Registrar Inquiry";
                                }
                                else if (APP_IOP_FLG == "II")
                                {
                                    TxtIopFlag.Text = "Intermediary Modification";
                                }
                                else if (APP_IOP_FLG == "IS")
                                {
                                    TxtIopFlag.Text = "Intermediary Download/Fetch";
                                }
                                else if (APP_IOP_FLG == "IE")
                                {
                                    TxtIopFlag.Text = "Intermediary Inquiry";
                                }
                            }
                            else if (childNode.Name == "APP_POS_CODE")
                            {
                                TxtPOSCode.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_UPDTFLG")
                            {
                                string APP_UPDTFLG = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INT_CODE")
                            {
                                string APP_INT_CODE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_TYPE")
                            {
                                string APP_TYPE = childNode.InnerText;
                                if (APP_TYPE == "I")
                                {
                                    TxtAppType.Text = "Individual";
                                }
                                else if (APP_TYPE == "N")
                                {
                                    TxtAppType.Text = "Non-Individual";
                                }
                            }
                            else if (childNode.Name == "APP_NO")
                            {
                                TxtAppNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DATE")
                            {
                                string APP_DATE = childNode.InnerText;
                                DateTime dt;
                                if (APP_DATE != "")
                                {
                                    dt = Convert.ToDateTime(childNode.InnerText);
                                    TxtAppDate.Text = dt.ToString("dd/MM/yyyy");
                                }

                            }
                            else if (childNode.Name == "APP_EXMT")
                            {
                                string APP_EXMT = childNode.InnerText;
                                if (APP_EXMT == "Y")
                                {
                                    TxtExmt.Text = "Yes";
                                }
                                else if (APP_EXMT == "N")
                                {
                                    TxtExmt.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_EXMT_CAT")
                            {
                                string APP_EXMT_CAT = childNode.InnerText;
                                if (APP_EXMT_CAT == "01" || APP_EXMT_CAT == "1")
                                {
                                    TxtExmptCat.Text = "SIKKIM Resident";
                                }
                                else if (APP_EXMT_CAT == "02" || APP_EXMT_CAT == "2")
                                {
                                    TxtExmptCat.Text = "Transactions carried out on behalf of  STATE GOVT";
                                }
                                else if (APP_EXMT_CAT == "03" || APP_EXMT_CAT == "3")
                                {
                                    TxtExmptCat.Text = "Transactions carried out on behalf of CENTRAL GOVT";
                                }
                                else if (APP_EXMT_CAT == "04" || APP_EXMT_CAT == "4")
                                {
                                    TxtExmptCat.Text = "COURT APPOINTED OFFICIALS";
                                }
                                else if (APP_EXMT_CAT == "05" || APP_EXMT_CAT == "5")
                                {
                                    TxtExmptCat.Text = "UN Entity/Multilateral agency exempt from paying tax in India";
                                }
                                else if (APP_EXMT_CAT == "06" || APP_EXMT_CAT == "6")
                                {
                                    TxtExmptCat.Text = "Official Liquidator";
                                }
                                else if (APP_EXMT_CAT == "07" || APP_EXMT_CAT == "7")
                                {
                                    TxtExmptCat.Text = "Court Receiver";
                                }
                                else if (APP_EXMT_CAT == "08" || APP_EXMT_CAT == "8")
                                {
                                    TxtExmptCat.Text = "SIP of Mutual Funds Upto Rs. 50,000/- p.a.";
                                }
                            }
                            else if (childNode.Name == "APP_EXMT_ID_PROOF")
                            {
                                string APP_EXMT_ID_PROOF = childNode.InnerText;
                                if (APP_EXMT_ID_PROOF == "01")
                                {
                                    TxtExmptCat.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_EXMT_ID_PROOF == "02")
                                {
                                    TxtExmpIdProof.Text = "Passport";
                                }
                                else if (APP_EXMT_ID_PROOF == "03")
                                {
                                    TxtExmpIdProof.Text = "Voter ID Card";
                                }
                                else if (APP_EXMT_ID_PROOF == "04")
                                {
                                    TxtExmpIdProof.Text = "Driving License";
                                }
                                else if (APP_EXMT_ID_PROOF == "05")
                                {
                                    TxtExmpIdProof.Text = "PAN Card with Photograph";
                                }
                                else if (APP_EXMT_ID_PROOF == "06")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Central / State Govt";
                                }
                                else if (APP_EXMT_ID_PROOF == "07")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Statutory/Regulatory Authorities";
                                }
                                else if (APP_EXMT_ID_PROOF == "08")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Public Sector Undertakings";
                                }
                                else if (APP_EXMT_ID_PROOF == "09")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Scheduled Commercial Banks";
                                }
                                else if (APP_EXMT_ID_PROOF == "10")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Public Financial Institutions";
                                }
                                else if (APP_EXMT_ID_PROOF == "11")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Colleges affiliated to Universities";
                                }
                                else if (APP_EXMT_ID_PROOF == "12")
                                {
                                    TxtExmpIdProof.Text = "ID Card with photo issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council etc., to their Members";
                                }
                                else if (APP_EXMT_ID_PROOF == "13")
                                {
                                    TxtExmpIdProof.Text = "Credit cards/Debit cards issued by Banks";
                                }
                            }
                            else if (childNode.Name == "APP_IPV_FLAG")
                            {
                                string APP_IPV_FLAG = childNode.InnerText;
                                if (APP_IPV_FLAG == "Y")
                                {
                                    TxtIpvFlag.Text = "Yes";
                                }
                                else if (APP_IPV_FLAG == "N")
                                {
                                    TxtIpvFlag.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_IPV_DATE")
                            {
                                string APP_IPV_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_GEN")
                            {
                                string APP_GEN = childNode.InnerText;
                                if (APP_GEN == "M")
                                {
                                    TxtGender.Text = "Male";
                                }
                                else if (APP_GEN == "F")
                                {
                                    TxtGender.Text = "Female";
                                }
                            }
                            else if (childNode.Name == "APP_NAME")
                            {
                                TxtName.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_F_NAME")
                            {
                                TxtFthName.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DOB_DT")
                            {
                                string APP_DOB_DT = childNode.InnerText;
                                DateTime dt;
                                if (APP_DOB_DT != "")
                                {
                                    dt = Convert.ToDateTime(childNode.InnerText);
                                    TxtDobDt.Text = dt.ToString("dd/MM/yyyy");
                                }
                            }
                            else if (childNode.Name == "APP_DOI_DT")
                            {
                                TxtDoiDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_REGNO")
                            {
                                TxtRegNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COMMENCE_DT")
                            {
                                TxtCommence.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_NATIONALITY")
                            {
                                string APP_NATIONALITY = childNode.InnerText;
                                if (APP_NATIONALITY == "01")
                                {
                                    TxtNationality.Text = "Indian";
                                }
                                else if (APP_NATIONALITY == "99")
                                {
                                    TxtNationality.Text = "Other (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_NATIONALITY")
                            {
                                string APP_OTH_NATIONALITY = childNode.InnerText;

                            }
                            else if (childNode.Name == "APP_COMP_STATUS")
                            {
                                string APP_COMP_STATUS = childNode.InnerText;
                                if (APP_COMP_STATUS == "01")
                                {
                                    TxtCompStatus.Text = "Private Ltd Company";
                                }
                                else if (APP_COMP_STATUS == "02")
                                {
                                    TxtCompStatus.Text = "Public Ltd Company";
                                }
                                else if (APP_COMP_STATUS == "03")
                                {
                                    TxtCompStatus.Text = "Body Corporate";
                                }
                                else if (APP_COMP_STATUS == "04")
                                {
                                    TxtCompStatus.Text = "Partnership";
                                }
                                else if (APP_COMP_STATUS == "05")
                                {
                                    TxtCompStatus.Text = "Trust / Charities / NGOs";
                                }
                                else if (APP_COMP_STATUS == "06")
                                {
                                    TxtCompStatus.Text = "FI";
                                }
                                else if (APP_COMP_STATUS == "07")
                                {
                                    TxtCompStatus.Text = "FII";
                                }
                                else if (APP_COMP_STATUS == "08")
                                {
                                    TxtCompStatus.Text = "HUF";
                                }
                                else if (APP_COMP_STATUS == "09")
                                {
                                    TxtCompStatus.Text = "AOP";
                                }
                                else if (APP_COMP_STATUS == "10")
                                {
                                    TxtCompStatus.Text = "Bank";
                                }
                                else if (APP_COMP_STATUS == "11")
                                {
                                    TxtCompStatus.Text = "Government Body";
                                }
                                else if (APP_COMP_STATUS == "12")
                                {
                                    TxtCompStatus.Text = "Non-Government Organisation";
                                }
                                else if (APP_COMP_STATUS == "13")
                                {
                                    TxtCompStatus.Text = "Defense Establishment";
                                }
                                else if (APP_COMP_STATUS == "14")
                                {
                                    TxtCompStatus.Text = "Body of Individuals";
                                }
                                else if (APP_COMP_STATUS == "15")
                                {
                                    TxtCompStatus.Text = "Society";
                                }
                                else if (APP_COMP_STATUS == "16")
                                {
                                    TxtCompStatus.Text = "LLP";
                                }
                                else if (APP_COMP_STATUS == "99")
                                {
                                    TxtCompStatus.Text = "Others";
                                }
                                else if (APP_COMP_STATUS == "19")
                                {
                                    TxtCompStatus.Text = "FPI - Category I";
                                }
                                else if (APP_COMP_STATUS == "20")
                                {
                                    TxtCompStatus.Text = "FPI - Category II";
                                }
                                else if (APP_COMP_STATUS == "21")
                                {
                                    TxtCompStatus.Text = "FPI - Category III";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_COMP_STATUS")
                            {

                                TxtOthCompStatus.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RES_STATUS")
                            {
                                string APP_RES_STATUS = childNode.InnerText;

                                if (APP_RES_STATUS == "R")
                                {
                                    TxtResStatus.Text = "Resident Individual";
                                }
                                else if (APP_RES_STATUS == "N")
                                {
                                    TxtResStatus.Text = "Non Resident";
                                }
                                else if (APP_RES_STATUS == "P")
                                {
                                    TxtResStatus.Text = "Foreign National";
                                }
                            }
                            else if (childNode.Name == "APP_RES_STATUS_PROOF")
                            {
                                TxtRefProof.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PAN_NO")
                            {
                                TxtAppPan.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PANEX_NO")
                            {
                                TxtPanexNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PAN_COPY")
                            {
                                string APP_PAN_COPY = childNode.InnerText;
                                if (APP_PAN_COPY == "Y")
                                {
                                    TxtPanCopy.Text = "Yes";
                                }
                                else if (APP_PAN_COPY == "N")
                                {
                                    TxtPanCopy.Text = "No";
                                }
                            }
                            else if (childNode.Name == "APP_UID_NO")
                            {
                                TxtUidNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD1")
                            {
                                TxtCurrAddr1.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD2")
                            {
                                TxtCurrAddr2.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD3")
                            {
                                TxtCurrAddr3.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_CITY")
                            {
                                TxtCurrCity.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_PINCD")
                            {
                                TxtCurrPincode.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_STATE")
                            {
                                string APP_COR_STATE = childNode.InnerText;
                                if (APP_COR_STATE == "035")
                                {
                                    TxtCurrState.Text = "Andaman & Nicobar Islands";
                                }
                                else if (APP_COR_STATE == "028")
                                {
                                    TxtCurrState.Text = "Andhra Pradesh";
                                }
                                else if (APP_COR_STATE == "012")
                                {
                                    TxtCurrState.Text = "Arunachal Pradesh";
                                }
                                else if (APP_COR_STATE == "013")
                                {
                                    TxtCurrState.Text = "Assam";
                                }
                                else if (APP_COR_STATE == "010")
                                {
                                    TxtCurrState.Text = "Bihar";
                                }
                                else if (APP_COR_STATE == "004")
                                {
                                    TxtCurrState.Text = "Chandigarh";
                                }
                                else if (APP_COR_STATE == "026")
                                {
                                    TxtCurrState.Text = "Dadra & Nagar Haveli";
                                }
                                else if (APP_COR_STATE == "025")
                                {
                                    TxtCurrState.Text = "Daman & Diu";
                                }
                                else if (APP_COR_STATE == "007")
                                {
                                    TxtCurrState.Text = "Delhi";
                                }
                                else if (APP_COR_STATE == "030")
                                {
                                    TxtCurrState.Text = "Goa";
                                }
                                else if (APP_COR_STATE == "024")
                                {
                                    TxtCurrState.Text = "Gujarat";
                                }
                                else if (APP_COR_STATE == "006")
                                {
                                    TxtCurrState.Text = "Haryana";
                                }
                                else if (APP_COR_STATE == "002")
                                {
                                    TxtCurrState.Text = "Himachal Pradesh";
                                }
                                else if (APP_COR_STATE == "001")
                                {
                                    TxtCurrState.Text = "Jammu & Kashmir";
                                }
                                else if (APP_COR_STATE == "029")
                                {
                                    TxtCurrState.Text = "Karnataka";
                                }
                                else if (APP_COR_STATE == "032")
                                {
                                    TxtCurrState.Text = "Kerala";
                                }
                                else if (APP_COR_STATE == "031")
                                {
                                    TxtCurrState.Text = "Lakhswadeep";
                                }
                                else if (APP_COR_STATE == "023")
                                {
                                    TxtCurrState.Text = "Madhya Pradesh";
                                }
                                else if (APP_COR_STATE == "027")
                                {
                                    TxtCurrState.Text = "Maharashtra";
                                }
                                else if (APP_COR_STATE == "014")
                                {
                                    TxtCurrState.Text = "Manipur";
                                }
                                else if (APP_COR_STATE == "017")
                                {
                                    TxtCurrState.Text = "Meghalaya";
                                }
                                else if (APP_COR_STATE == "015")
                                {
                                    TxtCurrState.Text = "Mizoram";
                                }
                                else if (APP_COR_STATE == "018")
                                {
                                    TxtCurrState.Text = "Nagaland";
                                }
                                else if (APP_COR_STATE == "021")
                                {
                                    TxtCurrState.Text = "Orissa";
                                }
                                else if (APP_COR_STATE == "034")
                                {
                                    TxtCurrState.Text = "Pondicherry";
                                }
                                else if (APP_COR_STATE == "003")
                                {
                                    TxtCurrState.Text = "Punjab";
                                }
                                else if (APP_COR_STATE == "008")
                                {
                                    TxtCurrState.Text = "Rajasthan";
                                }
                                else if (APP_COR_STATE == "011")
                                {
                                    TxtCurrState.Text = "Sikkim";
                                }
                                else if (APP_COR_STATE == "033")
                                {
                                    TxtCurrState.Text = "Tamil Nadu";
                                }
                                else if (APP_COR_STATE == "016")
                                {
                                    TxtCurrState.Text = "Tripura";
                                }
                                else if (APP_COR_STATE == "009")
                                {
                                    TxtCurrState.Text = "Uttar Pradesh";
                                }
                                else if (APP_COR_STATE == "019")
                                {
                                    TxtCurrState.Text = "West Bengal";
                                }
                                else if (APP_COR_STATE == "022")
                                {
                                    TxtCurrState.Text = "Chhattisgarh";
                                }
                                else if (APP_COR_STATE == "005")
                                {
                                    TxtCurrState.Text = "Uttaranchal";
                                }
                                else if (APP_COR_STATE == "020")
                                {
                                    TxtCurrState.Text = "Jharkhand";
                                }
                                else if (APP_COR_STATE == "037")
                                {
                                    TxtCurrState.Text = "Telangana";
                                }
                                else if (APP_COR_STATE == "099")
                                {
                                    TxtCurrState.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_COR_CTRY")
                            {
                                string APP_COR_CTRY = childNode.InnerText;
                                if (APP_COR_CTRY == "001")
                                {
                                    TxtCurrnCountry.Text = "AFGHANISTAN";
                                }
                                else if (APP_COR_CTRY == "003")
                                {
                                    TxtCurrnCountry.Text = "ALBENIA";
                                }
                                else if (APP_COR_CTRY == "004")
                                {
                                    TxtCurrnCountry.Text = "ALGERIA";
                                }
                                else if (APP_COR_CTRY == "007")
                                {
                                    TxtCurrnCountry.Text = "ANGOLA";
                                }
                                else if (APP_COR_CTRY == "011")
                                {
                                    TxtCurrnCountry.Text = "ARGENTINA";
                                }
                                else if (APP_COR_CTRY == "012")
                                {
                                    TxtCurrnCountry.Text = "ARMENIA";
                                }
                                else if (APP_COR_CTRY == "013")
                                {
                                    TxtCurrnCountry.Text = "ARUBA";
                                }
                                else if (APP_COR_CTRY == "014")
                                {
                                    TxtCurrnCountry.Text = "AUSTRALIA";
                                }
                                else if (APP_COR_CTRY == "015")
                                {
                                    TxtCurrnCountry.Text = "AUSTRIA";
                                }
                                else if (APP_COR_CTRY == "016")
                                {
                                    TxtCurrnCountry.Text = "AZARBAIJAN";
                                }
                                else if (APP_COR_CTRY == "017")
                                {
                                    TxtCurrnCountry.Text = "BAHAMAS";
                                }
                                else if (APP_COR_CTRY == "018")
                                {
                                    TxtCurrnCountry.Text = "BAHRAIN";
                                }
                                else if (APP_COR_CTRY == "019")
                                {
                                    TxtCurrnCountry.Text = "BANGLADESH";
                                }
                                else if (APP_COR_CTRY == "020")
                                {
                                    TxtCurrnCountry.Text = "BARBADOS";
                                }
                                else if (APP_COR_CTRY == "021")
                                {
                                    TxtCurrnCountry.Text = "BELARUS";
                                }
                                else if (APP_COR_CTRY == "022")
                                {
                                    TxtCurrnCountry.Text = "BELGIUM";
                                }
                                else if (APP_COR_CTRY == "023")
                                {
                                    TxtCurrnCountry.Text = "BELIZE";
                                }
                                else if (APP_COR_CTRY == "024")
                                {
                                    TxtCurrnCountry.Text = "BENIN";
                                }
                                else if (APP_COR_CTRY == "025")
                                {
                                    TxtCurrnCountry.Text = "BERMUDA";
                                }
                                else if (APP_COR_CTRY == "026")
                                {
                                    TxtCurrnCountry.Text = "BHUTAN";
                                }
                                else if (APP_COR_CTRY == "027")
                                {
                                    TxtCurrnCountry.Text = "BOLIVIAN";
                                }
                                else if (APP_COR_CTRY == "028")
                                {
                                    TxtCurrnCountry.Text = "BOSNIA-HERZEGOVINA";
                                }
                                else if (APP_COR_CTRY == "029")
                                {
                                    TxtCurrnCountry.Text = "BOTSWANA";
                                }
                                else if (APP_COR_CTRY == "031")
                                {
                                    TxtCurrnCountry.Text = "BRAZIL";
                                }
                                else if (APP_COR_CTRY == "25")
                                {
                                    TxtCurrnCountry.Text = "BRUNEI";
                                }
                                else if (APP_COR_CTRY == "034")
                                {
                                    TxtCurrnCountry.Text = "BULGARIA";
                                }
                                else if (APP_COR_CTRY == "035")
                                {
                                    TxtCurrnCountry.Text = "BURKINA FASO";
                                }
                                else if (APP_COR_CTRY == "036")
                                {
                                    TxtCurrnCountry.Text = "BURUNDI";
                                }
                                else if (APP_COR_CTRY == "")
                                {
                                    TxtCurrnCountry.Text = "CAMEROON REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "039")
                                {
                                    TxtCurrnCountry.Text = "CANADA";
                                }
                                else if (APP_COR_CTRY == "040")
                                {
                                    TxtCurrnCountry.Text = "CAPE VERDE";
                                }
                                else if (APP_COR_CTRY == "041")
                                {
                                    TxtCurrnCountry.Text = "CAYMAN ISLANDS";
                                }
                                else if (APP_COR_CTRY == "042")
                                {
                                    TxtCurrnCountry.Text = "CENTRAL AFRICAN REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "043")
                                {
                                    TxtCurrnCountry.Text = "CHAD";
                                }
                                else if (APP_COR_CTRY == "044")
                                {
                                    TxtCurrnCountry.Text = "CHILE";
                                }
                                else if (APP_COR_CTRY == "045")
                                {
                                    TxtCurrnCountry.Text = "CHINA";
                                }
                                else if (APP_COR_CTRY == "048")
                                {
                                    TxtCurrnCountry.Text = "COLOMBIA";
                                }
                                else if (APP_COR_CTRY == "037")
                                {
                                    TxtCurrnCountry.Text = "COMBODIA";
                                }
                                else if (APP_COR_CTRY == "038")
                                {
                                    TxtCurrnCountry.Text = "COMOROS";
                                }
                                else if (APP_COR_CTRY == "050")
                                {
                                    TxtCurrnCountry.Text = "CONGO";
                                }
                                else if (APP_COR_CTRY == "052")
                                {
                                    TxtCurrnCountry.Text = "COOK ISLANDS";
                                }
                                else if (APP_COR_CTRY == "053")
                                {
                                    TxtCurrnCountry.Text = "COSTA RICA";
                                }
                                else if (APP_COR_CTRY == "054")
                                {
                                    TxtCurrnCountry.Text = "COTE D'IVOIRE";
                                }
                                else if (APP_COR_CTRY == "055")
                                {
                                    TxtCurrnCountry.Text = "CROATIA";
                                }
                                else if (APP_COR_CTRY == "056")
                                {
                                    TxtCurrnCountry.Text = "CUBA";
                                }
                                else if (APP_COR_CTRY == "057")
                                {
                                    TxtCurrnCountry.Text = "CYPRUS";
                                }
                                else if (APP_COR_CTRY == "058")
                                {
                                    TxtCurrnCountry.Text = "CZECH REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "059")
                                {
                                    TxtCurrnCountry.Text = "DENMARK";
                                }
                                else if (APP_COR_CTRY == "060")
                                {
                                    TxtCurrnCountry.Text = "DJIBOUTI";
                                }
                                else if (APP_COR_CTRY == "061")
                                {
                                    TxtCurrnCountry.Text = "DOMINICA";
                                }
                                else if (APP_COR_CTRY == "062")
                                {
                                    TxtCurrnCountry.Text = "DOMINICAN REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "063")
                                {
                                    TxtCurrnCountry.Text = "ECUADOR";
                                }
                                else if (APP_COR_CTRY == "064")
                                {
                                    TxtCurrnCountry.Text = "EGYPT";
                                }
                                else if (APP_COR_CTRY == "065")
                                {
                                    TxtCurrnCountry.Text = "EL SALVADOR";
                                }
                                else if (APP_COR_CTRY == "066")
                                {
                                    TxtCurrnCountry.Text = "EQUATORIAL GUINEA";
                                }
                                else if (APP_COR_CTRY == "068")
                                {
                                    TxtCurrnCountry.Text = "ESTONIA";
                                }
                                else if (APP_COR_CTRY == "069")
                                {
                                    TxtCurrnCountry.Text = "ETHIOPIA";
                                }
                                else if (APP_COR_CTRY == "072")
                                {
                                    TxtCurrnCountry.Text = "FIJI";
                                }
                                else if (APP_COR_CTRY == "073")
                                {
                                    TxtCurrnCountry.Text = "FINLAND";
                                }
                                else if (APP_COR_CTRY == "074")
                                {
                                    TxtCurrnCountry.Text = "FRANCE";
                                }
                                else if (APP_COR_CTRY == "075")
                                {
                                    TxtCurrnCountry.Text = "FRENCH GUIANA";
                                }
                                else if (APP_COR_CTRY == "076")
                                {
                                    TxtCurrnCountry.Text = "FRENCH POLYNESIA";
                                }
                                else if (APP_COR_CTRY == "078")
                                {
                                    TxtCurrnCountry.Text = "GABON";
                                }
                                else if (APP_COR_CTRY == "079")
                                {
                                    TxtCurrnCountry.Text = "GAMBIA";
                                }
                                else if (APP_COR_CTRY == "080")
                                {
                                    TxtCurrnCountry.Text = "GEORGIA";
                                }
                                else if (APP_COR_CTRY == "081")
                                {
                                    TxtCurrnCountry.Text = "GERMANY";
                                }
                                else if (APP_COR_CTRY == "082")
                                {
                                    TxtCurrnCountry.Text = "GHANA";
                                }
                                else if (APP_COR_CTRY == "083")
                                {
                                    TxtCurrnCountry.Text = "GIBRALTOR";
                                }
                                else if (APP_COR_CTRY == "084")
                                {
                                    TxtCurrnCountry.Text = "GREECE";
                                }
                                else if (APP_COR_CTRY == "085")
                                {
                                    TxtCurrnCountry.Text = "GREENLAND";
                                }
                                else if (APP_COR_CTRY == "086")
                                {
                                    TxtCurrnCountry.Text = "GRENADA";
                                }
                                else if (APP_COR_CTRY == "087")
                                {
                                    TxtCurrnCountry.Text = "GUADELOUPE";
                                }
                                else if (APP_COR_CTRY == "088")
                                {
                                    TxtCurrnCountry.Text = "GUAM";
                                }
                                else if (APP_COR_CTRY == "089")
                                {
                                    TxtCurrnCountry.Text = "GUATEMALA";
                                }
                                else if (APP_COR_CTRY == "090")
                                {
                                    TxtCurrnCountry.Text = "GUERNSEY";
                                }
                                else if (APP_COR_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_COR_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_COR_CTRY == "092")
                                {
                                    TxtCurrnCountry.Text = "GUINEA-BISSAU";
                                }
                                else if (APP_COR_CTRY == "093")
                                {
                                    TxtCurrnCountry.Text = "GUYANA";
                                }
                                else if (APP_COR_CTRY == "094")
                                {
                                    TxtCurrnCountry.Text = "HAITI";
                                }
                                else if (APP_COR_CTRY == "097")
                                {
                                    TxtCurrnCountry.Text = "HONDURAS";
                                }
                                else if (APP_COR_CTRY == "098")
                                {
                                    TxtCurrnCountry.Text = "HONGKONG";
                                }
                                else if (APP_COR_CTRY == "100")
                                {
                                    TxtCurrnCountry.Text = "ICELAND";
                                }
                                else if (APP_COR_CTRY == "101")
                                {
                                    TxtCurrnCountry.Text = "INDIA";
                                }
                                else if (APP_COR_CTRY == "102")
                                {
                                    TxtCurrnCountry.Text = "INDONESIA";
                                }
                                else if (APP_COR_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN";
                                }
                                else if (APP_COR_CTRY == "104")
                                {
                                    TxtCurrnCountry.Text = "IRAQ";
                                }
                                else if (APP_COR_CTRY == "105")
                                {
                                    TxtCurrnCountry.Text = "IRELAND";
                                }
                                else if (APP_COR_CTRY == "107")
                                {
                                    TxtCurrnCountry.Text = "ISRAEL";
                                }
                                else if (APP_COR_CTRY == "108")
                                {
                                    TxtCurrnCountry.Text = "ITALY";
                                }
                                else if (APP_COR_CTRY == "109")
                                {
                                    TxtCurrnCountry.Text = "JAMAICA";
                                }
                                else if (APP_COR_CTRY == "110")
                                {
                                    TxtCurrnCountry.Text = "JAPAN";
                                }
                                else if (APP_COR_CTRY == "112")
                                {
                                    TxtCurrnCountry.Text = "JORDAN";
                                }
                                else if (APP_COR_CTRY == "113")
                                {
                                    TxtCurrnCountry.Text = "KAZAKSTAN";
                                }
                                else if (APP_COR_CTRY == "114")
                                {
                                    TxtCurrnCountry.Text = "KENYA";
                                }
                                else if (APP_COR_CTRY == "118")
                                {
                                    TxtCurrnCountry.Text = "KUWAIT";
                                }
                                else if (APP_COR_CTRY == "119")
                                {
                                    TxtCurrnCountry.Text = "KYRGYZSTAN";
                                }
                                else if (APP_COR_CTRY == "121")
                                {
                                    TxtCurrnCountry.Text = "LATVIA";
                                }
                                else if (APP_COR_CTRY == "122")
                                {
                                    TxtCurrnCountry.Text = "LEBANON";
                                }
                                else if (APP_COR_CTRY == "123")
                                {
                                    TxtCurrnCountry.Text = "LESOTHO";
                                }
                                else if (APP_COR_CTRY == "124")
                                {
                                    TxtCurrnCountry.Text = "LIBERIA";
                                }
                                else if (APP_COR_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYA";
                                }
                                else if (APP_COR_CTRY == "127")
                                {
                                    TxtCurrnCountry.Text = "LITHUANIA";
                                }
                                else if (APP_COR_CTRY == "128")
                                {
                                    TxtCurrnCountry.Text = "LUXEMBOURG";
                                }
                                else if (APP_COR_CTRY == "129")
                                {
                                    TxtCurrnCountry.Text = "MACAU";
                                }
                                else if (APP_COR_CTRY == "130")
                                {
                                    TxtCurrnCountry.Text = "MACEDONIA";
                                }
                                else if (APP_COR_CTRY == "131")
                                {
                                    TxtCurrnCountry.Text = "MADAGASCAR";
                                }
                                else if (APP_COR_CTRY == "132")
                                {
                                    TxtCurrnCountry.Text = "MALAWI";
                                }
                                else if (APP_COR_CTRY == "133")
                                {
                                    TxtCurrnCountry.Text = "MALAYSIA";
                                }
                                else if (APP_COR_CTRY == "134")
                                {
                                    TxtCurrnCountry.Text = "MALDIVES";
                                }
                                else if (APP_COR_CTRY == "135")
                                {
                                    TxtCurrnCountry.Text = "MALI";
                                }
                                else if (APP_COR_CTRY == "136")
                                {
                                    TxtCurrnCountry.Text = "MALTA";
                                }
                                else if (APP_COR_CTRY == "139")
                                {
                                    TxtCurrnCountry.Text = "MAURITANIA";
                                }
                                else if (APP_COR_CTRY == "140")
                                {
                                    TxtCurrnCountry.Text = "MAURITIUS";
                                }
                                else if (APP_COR_CTRY == "142")
                                {
                                    TxtCurrnCountry.Text = "MEXICO";
                                }
                                else if (APP_COR_CTRY == "144")
                                {
                                    TxtCurrnCountry.Text = "MOLDOVA";
                                }
                                else if (APP_COR_CTRY == "146")
                                {
                                    TxtCurrnCountry.Text = "MONGOLIA";
                                }
                                else if (APP_COR_CTRY == "147")
                                {
                                    TxtCurrnCountry.Text = "MONTSERRAT";
                                }
                                else if (APP_COR_CTRY == "148")
                                {
                                    TxtCurrnCountry.Text = "MOROCCA";
                                }
                                else if (APP_COR_CTRY == "149")
                                {
                                    TxtCurrnCountry.Text = "MOZAMBIQUE";
                                }
                                else if (APP_COR_CTRY == "150")
                                {
                                    TxtCurrnCountry.Text = "MYANMAR";
                                }
                                else if (APP_COR_CTRY == "151")
                                {
                                    TxtCurrnCountry.Text = "NAMIBIA";
                                }
                                else if (APP_COR_CTRY == "153")
                                {
                                    TxtCurrnCountry.Text = "NEPAL";
                                }
                                else if (APP_COR_CTRY == "154")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS";
                                }
                                else if (APP_COR_CTRY == "155")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS ANTILLES";
                                }
                                else if (APP_COR_CTRY == "156")
                                {
                                    TxtCurrnCountry.Text = "NEW CALEDONIA";
                                }
                                else if (APP_COR_CTRY == "157")
                                {
                                    TxtCurrnCountry.Text = "NEW ZEALAND";
                                }
                                else if (APP_COR_CTRY == "158")
                                {
                                    TxtCurrnCountry.Text = "NICARAGUA";
                                }
                                else if (APP_COR_CTRY == "159")
                                {
                                    TxtCurrnCountry.Text = "NIGER";
                                }
                                else if (APP_COR_CTRY == "160")
                                {
                                    TxtCurrnCountry.Text = "NIGERIA";
                                }
                                else if (APP_COR_CTRY == "116")
                                {
                                    TxtCurrnCountry.Text = "NORTH KOREA";
                                }
                                else if (APP_COR_CTRY == "164")
                                {
                                    TxtCurrnCountry.Text = "NORWAY";
                                }
                                else if (APP_COR_CTRY == "165")
                                {
                                    TxtCurrnCountry.Text = "OMAN";
                                }
                                else if (APP_COR_CTRY == "166")
                                {
                                    TxtCurrnCountry.Text = "PAKISTAN";
                                }
                                else if (APP_COR_CTRY == "169")
                                {
                                    TxtCurrnCountry.Text = "PANAMA";
                                }
                                else if (APP_COR_CTRY == "170")
                                {
                                    TxtCurrnCountry.Text = "PAPUA NEW GUINEA";
                                }
                                else if (APP_COR_CTRY == "171")
                                {
                                    TxtCurrnCountry.Text = "PARAGUAY";
                                }
                                else if (APP_COR_CTRY == "172")
                                {
                                    TxtCurrnCountry.Text = "PERU";
                                }
                                else if (APP_COR_CTRY == "173")
                                {
                                    TxtCurrnCountry.Text = "PHILIPPINES";
                                }
                                else if (APP_COR_CTRY == "175")
                                {
                                    TxtCurrnCountry.Text = "POLAND";
                                }
                                else if (APP_COR_CTRY == "176")
                                {
                                    TxtCurrnCountry.Text = "PORTUGAL";
                                }
                                else if (APP_COR_CTRY == "178")
                                {
                                    TxtCurrnCountry.Text = "QATAR";
                                }
                                else if (APP_COR_CTRY == "180")
                                {
                                    TxtCurrnCountry.Text = "ROMANIA";
                                }
                                else if (APP_COR_CTRY == "181")
                                {
                                    TxtCurrnCountry.Text = "RUSSIA";
                                }
                                else if (APP_COR_CTRY == "182")
                                {
                                    TxtCurrnCountry.Text = "RWANDA";
                                }
                                else if (APP_COR_CTRY == "191")
                                {
                                    TxtCurrnCountry.Text = "SAUDI ARABIA";
                                }
                                else if (APP_COR_CTRY == "192")
                                {
                                    TxtCurrnCountry.Text = "SENEGAL";
                                }
                                else if (APP_COR_CTRY == "194")
                                {
                                    TxtCurrnCountry.Text = "SEYCHELLES";
                                }
                                else if (APP_COR_CTRY == "196")
                                {
                                    TxtCurrnCountry.Text = "SINGAPORE";
                                }
                                else if (APP_COR_CTRY == "197")
                                {
                                    TxtCurrnCountry.Text = "SLOVAKIA";
                                }
                                else if (APP_COR_CTRY == "198")
                                {
                                    TxtCurrnCountry.Text = "SLOVENIA";
                                }
                                else if (APP_COR_CTRY == "199")
                                {
                                    TxtCurrnCountry.Text = "SOLOMON ISLANDS";
                                }
                                else if (APP_COR_CTRY == "200")
                                {
                                    TxtCurrnCountry.Text = "SOMALIA";
                                }
                                else if (APP_COR_CTRY == "201")
                                {
                                    TxtCurrnCountry.Text = "SOUTH AFRICA";
                                }
                                else if (APP_COR_CTRY == "117")
                                {
                                    TxtCurrnCountry.Text = "SOUTH KOREA";
                                }
                                else if (APP_COR_CTRY == "203")
                                {
                                    TxtCurrnCountry.Text = "SPAIN";
                                }
                                else if (APP_COR_CTRY == "204")
                                {
                                    TxtCurrnCountry.Text = "SRI LANKA";
                                }
                                else if (APP_COR_CTRY == "205")
                                {
                                    TxtCurrnCountry.Text = "SUDAN";
                                }
                                else if (APP_COR_CTRY == "206")
                                {
                                    TxtCurrnCountry.Text = "SURINAME";
                                }
                                else if (APP_COR_CTRY == "208")
                                {
                                    TxtCurrnCountry.Text = "SWAZILAND";
                                }
                                else if (APP_COR_CTRY == "209")
                                {
                                    TxtCurrnCountry.Text = "SWEDEN";
                                }
                                else if (APP_COR_CTRY == "210")
                                {
                                    TxtCurrnCountry.Text = "SWITZERLAND";
                                }
                                else if (APP_COR_CTRY == "212")
                                {
                                    TxtCurrnCountry.Text = "TAIWAN";
                                }
                                else if (APP_COR_CTRY == "213")
                                {
                                    TxtCurrnCountry.Text = "TAJIKISTHAN";
                                }
                                else if (APP_COR_CTRY == "214")
                                {
                                    TxtCurrnCountry.Text = "TANZANIA";
                                }
                                else if (APP_COR_CTRY == "215")
                                {
                                    TxtCurrnCountry.Text = "THAILAND";
                                }
                                else if (APP_COR_CTRY == "217")
                                {
                                    TxtCurrnCountry.Text = "TOGO REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "219")
                                {
                                    TxtCurrnCountry.Text = "TONGA";
                                }
                                else if (APP_COR_CTRY == "220")
                                {
                                    TxtCurrnCountry.Text = "TRINIDAD AND TOBAGO";
                                }
                                else if (APP_COR_CTRY == "221")
                                {
                                    TxtCurrnCountry.Text = "TUNISIA";
                                }
                                else if (APP_COR_CTRY == "222")
                                {
                                    TxtCurrnCountry.Text = "TURKEY";
                                }
                                else if (APP_COR_CTRY == "223")
                                {
                                    TxtCurrnCountry.Text = "TURKMENISTAN";
                                }
                                else if (APP_COR_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "U A E";
                                }
                                else if (APP_COR_CTRY == "226")
                                {
                                    TxtCurrnCountry.Text = "UGANDA";
                                }
                                else if (APP_COR_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "UNITED ARAB EMIRATES";
                                }
                                else if (APP_COR_CTRY == "229")
                                {
                                    TxtCurrnCountry.Text = "UNITED KINGDOM";
                                }
                                else if (APP_COR_CTRY == "232")
                                {
                                    TxtCurrnCountry.Text = "URUGUAY";
                                }
                                else if (APP_COR_CTRY == "230")
                                {
                                    TxtCurrnCountry.Text = "USA";
                                }
                                else if (APP_COR_CTRY == "234")
                                {
                                    TxtCurrnCountry.Text = "VANUATU";
                                }
                                else if (APP_COR_CTRY == "235")
                                {
                                    TxtCurrnCountry.Text = "VENEZUELA";
                                }
                                else if (APP_COR_CTRY == "236")
                                {
                                    TxtCurrnCountry.Text = "VIETNAM";
                                }
                                else if (APP_COR_CTRY == "241")
                                {
                                    TxtCurrnCountry.Text = "YEMEN";
                                }
                                else if (APP_COR_CTRY == "242")
                                {
                                    TxtCurrnCountry.Text = "ZAMBIA";
                                }
                                else if (APP_COR_CTRY == "243")
                                {
                                    TxtCurrnCountry.Text = "ZIMBABWE";
                                }
                                else if (APP_COR_CTRY == "002")
                                {
                                    TxtCurrnCountry.Text = "ALAND ISLANDS";
                                }
                                else if (APP_COR_CTRY == "005")
                                {
                                    TxtCurrnCountry.Text = "AMERICAN SAMOA";
                                }
                                else if (APP_COR_CTRY == "006")
                                {
                                    TxtCurrnCountry.Text = "ANDORRA";
                                }
                                else if (APP_COR_CTRY == "008")
                                {
                                    TxtCurrnCountry.Text = "ANGUILLA";
                                }
                                else if (APP_COR_CTRY == "009")
                                {
                                    TxtCurrnCountry.Text = "ANTARCTICA";
                                }
                                else if (APP_COR_CTRY == "010")
                                {
                                    TxtCurrnCountry.Text = "ANTIGUA AND BARBUDA";
                                }
                                else if (APP_COR_CTRY == "030")
                                {
                                    TxtCurrnCountry.Text = "BOUVET ISLAND";
                                }
                                else if (APP_COR_CTRY == "032")
                                {
                                    TxtCurrnCountry.Text = "BRITISH INDIAN OCEAN TERRITORY";
                                }
                                else if (APP_COR_CTRY == "046")
                                {
                                    TxtCurrnCountry.Text = "CHRISTMAS ISLAND";
                                }
                                else if (APP_COR_CTRY == "047")
                                {
                                    TxtCurrnCountry.Text = "COCOS (KEELING) ISLANDS";
                                }
                                else if (APP_COR_CTRY == "067")
                                {
                                    TxtCurrnCountry.Text = "ERITREA";
                                }
                                else if (APP_COR_CTRY == "070")
                                {
                                    TxtCurrnCountry.Text = "FALKLAND ISLANDS (MALVINAS)";
                                }
                                else if (APP_COR_CTRY == "071")
                                {
                                    TxtCurrnCountry.Text = "FAROE ISLANDS";
                                }
                                else if (APP_COR_CTRY == "077")
                                {
                                    TxtCurrnCountry.Text = "FRENCH SOUTHERN TERRITORIES";
                                }
                                else if (APP_COR_CTRY == "095")
                                {
                                    TxtCurrnCountry.Text = "HEARD ISLAND AND MCDONALD ISLANDS";
                                }
                                else if (APP_COR_CTRY == "096")
                                {
                                    TxtCurrnCountry.Text = "HOLY SEE (VATICAN CITY STATE)";
                                }
                                else if (APP_COR_CTRY == "099")
                                {
                                    TxtCurrnCountry.Text = "HUNGARY";
                                }
                                else if (APP_COR_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN, ISLAMIC REPUBLIC OF";
                                }
                                else if (APP_COR_CTRY == "106")
                                {
                                    TxtCurrnCountry.Text = "ISLE OF MAN";
                                }
                                else if (APP_COR_CTRY == "115")
                                {
                                    TxtCurrnCountry.Text = "KIRIBATI";
                                }
                                else if (APP_COR_CTRY == "120")
                                {
                                    TxtCurrnCountry.Text = "LAO PEOPLE'S DEMOCRATIC REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYAN ARAB JAMAHIRIYA";
                                }
                                else if (APP_COR_CTRY == "126")
                                {
                                    TxtCurrnCountry.Text = "LIECHTENSTEIN";
                                }
                                else if (APP_COR_CTRY == "137")
                                {
                                    TxtCurrnCountry.Text = "MARSHALL ISLANDS";
                                }
                                else if (APP_COR_CTRY == "138")
                                {
                                    TxtCurrnCountry.Text = "MARTINIQUE";
                                }
                                else if (APP_COR_CTRY == "141")
                                {
                                    TxtCurrnCountry.Text = "MAYOTTE";
                                }
                                else if (APP_COR_CTRY == "143")
                                {
                                    TxtCurrnCountry.Text = "MICRONESIA, FEDERATED STATES OF";
                                }
                                else if (APP_COR_CTRY == "145")
                                {
                                    TxtCurrnCountry.Text = "MONACO";
                                }
                                else if (APP_COR_CTRY == "152")
                                {
                                    TxtCurrnCountry.Text = "NAURU";
                                }
                                else if (APP_COR_CTRY == "161")
                                {
                                    TxtCurrnCountry.Text = "NIUE";
                                }
                                else if (APP_COR_CTRY == "162")
                                {
                                    TxtCurrnCountry.Text = "NORFOLK ISLAND";
                                }
                                else if (APP_COR_CTRY == "163")
                                {
                                    TxtCurrnCountry.Text = "NORTHERN MARIANA ISLANDS";
                                }
                                else if (APP_COR_CTRY == "167")
                                {
                                    TxtCurrnCountry.Text = "PALAU";
                                }
                                else if (APP_COR_CTRY == "168")
                                {
                                    TxtCurrnCountry.Text = "PALESTINIAN TERRITORY, OCCUPIED";
                                }
                                else if (APP_COR_CTRY == "174")
                                {
                                    TxtCurrnCountry.Text = "PITCAIRN";
                                }
                                else if (APP_COR_CTRY == "177")
                                {
                                    TxtCurrnCountry.Text = "PUERTO RICO";
                                }
                                else if (APP_COR_CTRY == "179")
                                {
                                    TxtCurrnCountry.Text = "REUNION";
                                }
                                else if (APP_COR_CTRY == "183")
                                {
                                    TxtCurrnCountry.Text = "SAINT HELENA";
                                }
                                else if (APP_COR_CTRY == "184")
                                {
                                    TxtCurrnCountry.Text = "SAINT KITTS AND NEVIS";
                                }
                                else if (APP_COR_CTRY == "185")
                                {
                                    TxtCurrnCountry.Text = "SAINT LUCIA";
                                }
                                else if (APP_COR_CTRY == "186")
                                {
                                    TxtCurrnCountry.Text = "SAINT PIERRE AND MIQUELON";
                                }
                                else if (APP_COR_CTRY == "187")
                                {
                                    TxtCurrnCountry.Text = "SAINT VINCENT AND THE GRENADINES";
                                }
                                else if (APP_COR_CTRY == "188")
                                {
                                    TxtCurrnCountry.Text = "SAMOA";
                                }
                                else if (APP_COR_CTRY == "189")
                                {
                                    TxtCurrnCountry.Text = "SAN MARINO";
                                }
                                else if (APP_COR_CTRY == "190")
                                {
                                    TxtCurrnCountry.Text = "SAO TOME AND PRINCIPE";
                                }
                                else if (APP_COR_CTRY == "193")
                                {
                                    TxtCurrnCountry.Text = "SERBIA AND MONTENEGRO";
                                }
                                else if (APP_COR_CTRY == "195")
                                {
                                    TxtCurrnCountry.Text = "SIERRA LEONE";
                                }
                                else if (APP_COR_CTRY == "202")
                                {
                                    TxtCurrnCountry.Text = "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS";
                                }
                                else if (APP_COR_CTRY == "207")
                                {
                                    TxtCurrnCountry.Text = "SVALBARD AND JAN MAYEN";
                                }
                                else if (APP_COR_CTRY == "211")
                                {
                                    TxtCurrnCountry.Text = "SYRIAN ARAB REPUBLIC";
                                }
                                else if (APP_COR_CTRY == "216")
                                {
                                    TxtCurrnCountry.Text = "TIMOR-LESTE";
                                }
                                else if (APP_COR_CTRY == "218")
                                {
                                    TxtCurrnCountry.Text = "TOKELAU";
                                }
                                else if (APP_COR_CTRY == "224")
                                {
                                    TxtCurrnCountry.Text = "TURKS AND CAICOS ISLANDS";
                                }
                                else if (APP_COR_CTRY == "225")
                                {
                                    TxtCurrnCountry.Text = "TUVALU";
                                }
                                else if (APP_COR_CTRY == "227")
                                {
                                    TxtCurrnCountry.Text = "UKRAINE";
                                }
                                else if (APP_COR_CTRY == "231")
                                {
                                    TxtCurrnCountry.Text = "UNITED STATES MINOR OUTLYING ISLANDS";
                                }
                                else if (APP_COR_CTRY == "233")
                                {
                                    TxtCurrnCountry.Text = "UZBEKISTAN";
                                }
                                else if (APP_COR_CTRY == "237")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, BRITISH";
                                }
                                else if (APP_COR_CTRY == "238")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, U.S.";
                                }
                                else if (APP_COR_CTRY == "239")
                                {
                                    TxtCurrnCountry.Text = "WALLIS AND FUTUNA";
                                }
                                else if (APP_COR_CTRY == "240")
                                {
                                    TxtCurrnCountry.Text = "WESTERN SAHARA";
                                }

                            }
                            else if (childNode.Name == "APP_OFF_NO")
                            {
                                TxtOffNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RES_NO")
                            {
                                TxtResNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_MOB_NO")
                            {
                                TxtMobNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FAX_NO")
                            {
                                TxtFaxNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_EMAIL")
                            {
                                TxtEmailId.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD_PROOF")
                            {
                                string APP_COR_ADD_PROOF = childNode.InnerText;

                                if (APP_COR_ADD_PROOF == "01")
                                {
                                    TxtCurAddrProof.Text = "Passport";
                                }
                                else if (APP_COR_ADD_PROOF == "06")
                                {
                                    TxtCurAddrProof.Text = "Voter Identity Card";
                                }
                                else if (APP_COR_ADD_PROOF == "07")
                                {
                                    TxtCurAddrProof.Text = "Ration Card";
                                }
                                else if (APP_COR_ADD_PROOF == "08")
                                {
                                    TxtCurAddrProof.Text = "Registered Lease / Sale Agreement of Residence";
                                }
                                else if (APP_COR_ADD_PROOF == "02")
                                {
                                    TxtCurAddrProof.Text = "Driving License";
                                }
                                else if (APP_COR_ADD_PROOF == "13")
                                {
                                    TxtCurAddrProof.Text = "Flat Maintenance Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "14")
                                {
                                    TxtCurAddrProof.Text = "Insurance copy";
                                }
                                else if (APP_COR_ADD_PROOF == "09")
                                {
                                    TxtCurAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "10")
                                {
                                    TxtCurAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "11")
                                {
                                    TxtCurAddrProof.Text = "Gas Bill";
                                }
                                else if (APP_COR_ADD_PROOF == "03")
                                {
                                    TxtCurAddrProof.Text = "Latest Bank Passbook";
                                }
                                else if (APP_COR_ADD_PROOF == "04")
                                {
                                    TxtCurAddrProof.Text = "Latest Bank Account Statement";
                                }
                                else if (APP_COR_ADD_PROOF == "15")
                                {
                                    TxtCurAddrProof.Text = "Self Declaration by High Court / Supreme Court Judge";
                                }
                                else if (APP_COR_ADD_PROOF == "17")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Scheduled Commercial Banks / Scheduled Co-operative Banks / Multinational Foreign banks.";
                                }
                                else if (APP_COR_ADD_PROOF == "22")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Gazetted Officer";
                                }
                                else if (APP_COR_ADD_PROOF == "21")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Notary Public";
                                }
                                else if (APP_COR_ADD_PROOF == "18")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Elected representatives to the Legislative Assembly";
                                }
                                else if (APP_COR_ADD_PROOF == "19")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by Parliament";
                                }
                                else if (APP_COR_ADD_PROOF == "12")
                                {
                                    TxtCurAddrProof.Text = "Registration Certificate issued under Shops and Establishments Act";
                                }
                                else if (APP_COR_ADD_PROOF == "20")
                                {
                                    TxtCurAddrProof.Text = "Proof of Address issued by any Government / Statutory Authority";
                                }
                                else if (APP_COR_ADD_PROOF == "23")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Central / State Government ";
                                }
                                else if (APP_COR_ADD_PROOF == "24")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Statutory / Regulatory Authorities";
                                }
                                else if (APP_COR_ADD_PROOF == "25")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Public Sector Undertakings";
                                }
                                else if (APP_COR_ADD_PROOF == "26")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Scheduled Commercial Banks";
                                }
                                else if (APP_COR_ADD_PROOF == "27")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Public Financial Institutions";
                                }
                                else if (APP_COR_ADD_PROOF == "28")
                                {
                                    TxtCurAddrProof.Text = "ID Card with address issued by Colleges affiliated to universities";
                                }
                                else if (APP_COR_ADD_PROOF == "29")
                                {
                                    TxtCurAddrProof.Text = "ID Card issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council, etc. to their Members";
                                }
                                else if (APP_COR_ADD_PROOF == "16")
                                {
                                    TxtCurAddrProof.Text = "Power of Attorney given by FII/sub-account to the Custodians (which are duly notarised and/or apostiled or consularised) giving registered address.";
                                }
                                else if (APP_COR_ADD_PROOF == "31")
                                {
                                    TxtCurAddrProof.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_COR_ADD_PROOF == "05")
                                {
                                    TxtCurAddrProof.Text = "Latest Demat Account Statement";
                                }

                            }
                            else if (childNode.Name == "APP_COR_ADD_REF")
                            {
                                TxtCurrAddrRef.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_COR_ADD_DT")
                            {
                                TxtCurrAddrDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD_FLAG")
                            {
                                string APP_PER_ADD_FLAG = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD1")
                            {
                                TxtPerAddr1.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD2")
                            {
                                TxtPerAddr2.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD3")
                            {
                                TxtPrtAddr3.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_CITY")
                            {
                                TxtPerCity.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_PINCD")
                            {
                                TxtPerAddrPincode.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_STATE")
                            {
                                string APP_PER_STATE = childNode.InnerText;

                                if (APP_PER_STATE == "035")
                                {
                                    TxtPerAddrState.Text = "Andaman & Nicobar Islands";
                                }
                                else if (APP_PER_STATE == "028")
                                {
                                    TxtPerAddrState.Text = "Andhra Pradesh";
                                }
                                else if (APP_PER_STATE == "012")
                                {
                                    TxtPerAddrState.Text = "Arunachal Pradesh";
                                }
                                else if (APP_PER_STATE == "013")
                                {
                                    TxtPerAddrState.Text = "Assam";
                                }
                                else if (APP_PER_STATE == "010")
                                {
                                    TxtPerAddrState.Text = "Bihar";
                                }
                                else if (APP_PER_STATE == "004")
                                {
                                    TxtPerAddrState.Text = "Chandigarh";
                                }
                                else if (APP_PER_STATE == "026")
                                {
                                    TxtPerAddrState.Text = "Dadra & Nagar Haveli";
                                }
                                else if (APP_PER_STATE == "025")
                                {
                                    TxtPerAddrState.Text = "Daman & Diu";
                                }
                                else if (APP_PER_STATE == "007")
                                {
                                    TxtPerAddrState.Text = "Delhi";
                                }
                                else if (APP_PER_STATE == "030")
                                {
                                    TxtPerAddrState.Text = "Goa";
                                }
                                else if (APP_PER_STATE == "024")
                                {
                                    TxtPerAddrState.Text = "Gujarat";
                                }
                                else if (APP_PER_STATE == "006")
                                {
                                    TxtPerAddrState.Text = "Haryana";
                                }
                                else if (APP_PER_STATE == "002")
                                {
                                    TxtPerAddrState.Text = "Himachal Pradesh";
                                }
                                else if (APP_PER_STATE == "001")
                                {
                                    TxtPerAddrState.Text = "Jammu & Kashmir";
                                }
                                else if (APP_PER_STATE == "029")
                                {
                                    TxtPerAddrState.Text = "Karnataka";
                                }
                                else if (APP_PER_STATE == "032")
                                {
                                    TxtPerAddrState.Text = "Kerala";
                                }
                                else if (APP_PER_STATE == "031")
                                {
                                    TxtPerAddrState.Text = "Lakhswadeep";
                                }
                                else if (APP_PER_STATE == "023")
                                {
                                    TxtPerAddrState.Text = "Madhya Pradesh";
                                }
                                else if (APP_PER_STATE == "027")
                                {
                                    TxtPerAddrState.Text = "Maharashtra";
                                }
                                else if (APP_PER_STATE == "014")
                                {
                                    TxtPerAddrState.Text = "Manipur";
                                }
                                else if (APP_PER_STATE == "017")
                                {
                                    TxtPerAddrState.Text = "Meghalaya";
                                }
                                else if (APP_PER_STATE == "015")
                                {
                                    TxtPerAddrState.Text = "Mizoram";
                                }
                                else if (APP_PER_STATE == "018")
                                {
                                    TxtPerAddrState.Text = "Nagaland";
                                }
                                else if (APP_PER_STATE == "021")
                                {
                                    TxtPerAddrState.Text = "Orissa";
                                }
                                else if (APP_PER_STATE == "034")
                                {
                                    TxtPerAddrState.Text = "Pondicherry";
                                }
                                else if (APP_PER_STATE == "003")
                                {
                                    TxtPerAddrState.Text = "Punjab";
                                }
                                else if (APP_PER_STATE == "008")
                                {
                                    TxtPerAddrState.Text = "Rajasthan";
                                }
                                else if (APP_PER_STATE == "011")
                                {
                                    TxtPerAddrState.Text = "Sikkim";
                                }
                                else if (APP_PER_STATE == "033")
                                {
                                    TxtPerAddrState.Text = "Tamil Nadu";
                                }
                                else if (APP_PER_STATE == "016")
                                {
                                    TxtPerAddrState.Text = "Tripura";
                                }
                                else if (APP_PER_STATE == "009")
                                {
                                    TxtPerAddrState.Text = "Uttar Pradesh";
                                }
                                else if (APP_PER_STATE == "019")
                                {
                                    TxtPerAddrState.Text = "West Bengal";
                                }
                                else if (APP_PER_STATE == "022")
                                {
                                    TxtPerAddrState.Text = "Chhattisgarh";
                                }
                                else if (APP_PER_STATE == "005")
                                {
                                    TxtPerAddrState.Text = "Uttaranchal";
                                }
                                else if (APP_PER_STATE == "020")
                                {
                                    TxtPerAddrState.Text = "Jharkhand";
                                }
                                else if (APP_PER_STATE == "037")
                                {
                                    TxtPerAddrState.Text = "Telangana";
                                }
                                else if (APP_PER_STATE == "099")
                                {
                                    TxtPerAddrState.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_PER_CTRY")
                            {
                                string APP_PER_CTRY = childNode.InnerText;

                                if (APP_PER_CTRY == "001")
                                {
                                    TxtCurrnCountry.Text = "AFGHANISTAN";
                                }
                                else if (APP_PER_CTRY == "003")
                                {
                                    TxtCurrnCountry.Text = "ALBENIA";
                                }
                                else if (APP_PER_CTRY == "004")
                                {
                                    TxtCurrnCountry.Text = "ALGERIA";
                                }
                                else if (APP_PER_CTRY == "007")
                                {
                                    TxtCurrnCountry.Text = "ANGOLA";
                                }
                                else if (APP_PER_CTRY == "011")
                                {
                                    TxtCurrnCountry.Text = "ARGENTINA";
                                }
                                else if (APP_PER_CTRY == "012")
                                {
                                    TxtCurrnCountry.Text = "ARMENIA";
                                }
                                else if (APP_PER_CTRY == "013")
                                {
                                    TxtCurrnCountry.Text = "ARUBA";
                                }
                                else if (APP_PER_CTRY == "014")
                                {
                                    TxtCurrnCountry.Text = "AUSTRALIA";
                                }
                                else if (APP_PER_CTRY == "015")
                                {
                                    TxtCurrnCountry.Text = "AUSTRIA";
                                }
                                else if (APP_PER_CTRY == "016")
                                {
                                    TxtCurrnCountry.Text = "AZARBAIJAN";
                                }
                                else if (APP_PER_CTRY == "017")
                                {
                                    TxtCurrnCountry.Text = "BAHAMAS";
                                }
                                else if (APP_PER_CTRY == "018")
                                {
                                    TxtCurrnCountry.Text = "BAHRAIN";
                                }
                                else if (APP_PER_CTRY == "019")
                                {
                                    TxtCurrnCountry.Text = "BANGLADESH";
                                }
                                else if (APP_PER_CTRY == "020")
                                {
                                    TxtCurrnCountry.Text = "BARBADOS";
                                }
                                else if (APP_PER_CTRY == "021")
                                {
                                    TxtCurrnCountry.Text = "BELARUS";
                                }
                                else if (APP_PER_CTRY == "022")
                                {
                                    TxtCurrnCountry.Text = "BELGIUM";
                                }
                                else if (APP_PER_CTRY == "023")
                                {
                                    TxtCurrnCountry.Text = "BELIZE";
                                }
                                else if (APP_PER_CTRY == "024")
                                {
                                    TxtCurrnCountry.Text = "BENIN";
                                }
                                else if (APP_PER_CTRY == "025")
                                {
                                    TxtCurrnCountry.Text = "BERMUDA";
                                }
                                else if (APP_PER_CTRY == "026")
                                {
                                    TxtCurrnCountry.Text = "BHUTAN";
                                }
                                else if (APP_PER_CTRY == "027")
                                {
                                    TxtCurrnCountry.Text = "BOLIVIAN";
                                }
                                else if (APP_PER_CTRY == "028")
                                {
                                    TxtCurrnCountry.Text = "BOSNIA-HERZEGOVINA";
                                }
                                else if (APP_PER_CTRY == "029")
                                {
                                    TxtCurrnCountry.Text = "BOTSWANA";
                                }
                                else if (APP_PER_CTRY == "031")
                                {
                                    TxtCurrnCountry.Text = "BRAZIL";
                                }
                                else if (APP_PER_CTRY == "25")
                                {
                                    TxtCurrnCountry.Text = "BRUNEI";
                                }
                                else if (APP_PER_CTRY == "034")
                                {
                                    TxtCurrnCountry.Text = "BULGARIA";
                                }
                                else if (APP_PER_CTRY == "035")
                                {
                                    TxtCurrnCountry.Text = "BURKINA FASO";
                                }
                                else if (APP_PER_CTRY == "036")
                                {
                                    TxtCurrnCountry.Text = "BURUNDI";
                                }
                                else if (APP_PER_CTRY == "")
                                {
                                    TxtCurrnCountry.Text = "CAMEROON REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "039")
                                {
                                    TxtCurrnCountry.Text = "CANADA";
                                }
                                else if (APP_PER_CTRY == "040")
                                {
                                    TxtCurrnCountry.Text = "CAPE VERDE";
                                }
                                else if (APP_PER_CTRY == "041")
                                {
                                    TxtCurrnCountry.Text = "CAYMAN ISLANDS";
                                }
                                else if (APP_PER_CTRY == "042")
                                {
                                    TxtCurrnCountry.Text = "CENTRAL AFRICAN REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "043")
                                {
                                    TxtCurrnCountry.Text = "CHAD";
                                }
                                else if (APP_PER_CTRY == "044")
                                {
                                    TxtCurrnCountry.Text = "CHILE";
                                }
                                else if (APP_PER_CTRY == "045")
                                {
                                    TxtCurrnCountry.Text = "CHINA";
                                }
                                else if (APP_PER_CTRY == "048")
                                {
                                    TxtCurrnCountry.Text = "COLOMBIA";
                                }
                                else if (APP_PER_CTRY == "037")
                                {
                                    TxtCurrnCountry.Text = "COMBODIA";
                                }
                                else if (APP_PER_CTRY == "038")
                                {
                                    TxtCurrnCountry.Text = "COMOROS";
                                }
                                else if (APP_PER_CTRY == "050")
                                {
                                    TxtCurrnCountry.Text = "CONGO";
                                }
                                else if (APP_PER_CTRY == "052")
                                {
                                    TxtCurrnCountry.Text = "COOK ISLANDS";
                                }
                                else if (APP_PER_CTRY == "053")
                                {
                                    TxtCurrnCountry.Text = "COSTA RICA";
                                }
                                else if (APP_PER_CTRY == "054")
                                {
                                    TxtCurrnCountry.Text = "COTE D'IVOIRE";
                                }
                                else if (APP_PER_CTRY == "055")
                                {
                                    TxtCurrnCountry.Text = "CROATIA";
                                }
                                else if (APP_PER_CTRY == "056")
                                {
                                    TxtCurrnCountry.Text = "CUBA";
                                }
                                else if (APP_PER_CTRY == "057")
                                {
                                    TxtCurrnCountry.Text = "CYPRUS";
                                }
                                else if (APP_PER_CTRY == "058")
                                {
                                    TxtCurrnCountry.Text = "CZECH REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "059")
                                {
                                    TxtCurrnCountry.Text = "DENMARK";
                                }
                                else if (APP_PER_CTRY == "060")
                                {
                                    TxtCurrnCountry.Text = "DJIBOUTI";
                                }
                                else if (APP_PER_CTRY == "061")
                                {
                                    TxtCurrnCountry.Text = "DOMINICA";
                                }
                                else if (APP_PER_CTRY == "062")
                                {
                                    TxtCurrnCountry.Text = "DOMINICAN REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "063")
                                {
                                    TxtCurrnCountry.Text = "ECUADOR";
                                }
                                else if (APP_PER_CTRY == "064")
                                {
                                    TxtCurrnCountry.Text = "EGYPT";
                                }
                                else if (APP_PER_CTRY == "065")
                                {
                                    TxtCurrnCountry.Text = "EL SALVADOR";
                                }
                                else if (APP_PER_CTRY == "066")
                                {
                                    TxtCurrnCountry.Text = "EQUATORIAL GUINEA";
                                }
                                else if (APP_PER_CTRY == "068")
                                {
                                    TxtCurrnCountry.Text = "ESTONIA";
                                }
                                else if (APP_PER_CTRY == "069")
                                {
                                    TxtCurrnCountry.Text = "ETHIOPIA";
                                }
                                else if (APP_PER_CTRY == "072")
                                {
                                    TxtCurrnCountry.Text = "FIJI";
                                }
                                else if (APP_PER_CTRY == "073")
                                {
                                    TxtCurrnCountry.Text = "FINLAND";
                                }
                                else if (APP_PER_CTRY == "074")
                                {
                                    TxtCurrnCountry.Text = "FRANCE";
                                }
                                else if (APP_PER_CTRY == "075")
                                {
                                    TxtCurrnCountry.Text = "FRENCH GUIANA";
                                }
                                else if (APP_PER_CTRY == "076")
                                {
                                    TxtCurrnCountry.Text = "FRENCH POLYNESIA";
                                }
                                else if (APP_PER_CTRY == "078")
                                {
                                    TxtCurrnCountry.Text = "GABON";
                                }
                                else if (APP_PER_CTRY == "079")
                                {
                                    TxtCurrnCountry.Text = "GAMBIA";
                                }
                                else if (APP_PER_CTRY == "080")
                                {
                                    TxtCurrnCountry.Text = "GEORGIA";
                                }
                                else if (APP_PER_CTRY == "081")
                                {
                                    TxtCurrnCountry.Text = "GERMANY";
                                }
                                else if (APP_PER_CTRY == "082")
                                {
                                    TxtCurrnCountry.Text = "GHANA";
                                }
                                else if (APP_PER_CTRY == "083")
                                {
                                    TxtCurrnCountry.Text = "GIBRALTOR";
                                }
                                else if (APP_PER_CTRY == "084")
                                {
                                    TxtCurrnCountry.Text = "GREECE";
                                }
                                else if (APP_PER_CTRY == "085")
                                {
                                    TxtCurrnCountry.Text = "GREENLAND";
                                }
                                else if (APP_PER_CTRY == "086")
                                {
                                    TxtCurrnCountry.Text = "GRENADA";
                                }
                                else if (APP_PER_CTRY == "087")
                                {
                                    TxtCurrnCountry.Text = "GUADELOUPE";
                                }
                                else if (APP_PER_CTRY == "088")
                                {
                                    TxtCurrnCountry.Text = "GUAM";
                                }
                                else if (APP_PER_CTRY == "089")
                                {
                                    TxtCurrnCountry.Text = "GUATEMALA";
                                }
                                else if (APP_PER_CTRY == "090")
                                {
                                    TxtCurrnCountry.Text = "GUERNSEY";
                                }
                                else if (APP_PER_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_PER_CTRY == "091")
                                {
                                    TxtCurrnCountry.Text = "GUINEA";
                                }
                                else if (APP_PER_CTRY == "092")
                                {
                                    TxtCurrnCountry.Text = "GUINEA-BISSAU";
                                }
                                else if (APP_PER_CTRY == "093")
                                {
                                    TxtCurrnCountry.Text = "GUYANA";
                                }
                                else if (APP_PER_CTRY == "094")
                                {
                                    TxtCurrnCountry.Text = "HAITI";
                                }
                                else if (APP_PER_CTRY == "097")
                                {
                                    TxtCurrnCountry.Text = "HONDURAS";
                                }
                                else if (APP_PER_CTRY == "098")
                                {
                                    TxtCurrnCountry.Text = "HONGKONG";
                                }
                                else if (APP_PER_CTRY == "100")
                                {
                                    TxtCurrnCountry.Text = "ICELAND";
                                }
                                else if (APP_PER_CTRY == "101")
                                {
                                    TxtCurrnCountry.Text = "INDIA";
                                }
                                else if (APP_PER_CTRY == "102")
                                {
                                    TxtCurrnCountry.Text = "INDONESIA";
                                }
                                else if (APP_PER_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN";
                                }
                                else if (APP_PER_CTRY == "104")
                                {
                                    TxtCurrnCountry.Text = "IRAQ";
                                }
                                else if (APP_PER_CTRY == "105")
                                {
                                    TxtCurrnCountry.Text = "IRELAND";
                                }
                                else if (APP_PER_CTRY == "107")
                                {
                                    TxtCurrnCountry.Text = "ISRAEL";
                                }
                                else if (APP_PER_CTRY == "108")
                                {
                                    TxtCurrnCountry.Text = "ITALY";
                                }
                                else if (APP_PER_CTRY == "109")
                                {
                                    TxtCurrnCountry.Text = "JAMAICA";
                                }
                                else if (APP_PER_CTRY == "110")
                                {
                                    TxtCurrnCountry.Text = "JAPAN";
                                }
                                else if (APP_PER_CTRY == "112")
                                {
                                    TxtCurrnCountry.Text = "JORDAN";
                                }
                                else if (APP_PER_CTRY == "113")
                                {
                                    TxtCurrnCountry.Text = "KAZAKSTAN";
                                }
                                else if (APP_PER_CTRY == "114")
                                {
                                    TxtCurrnCountry.Text = "KENYA";
                                }
                                else if (APP_PER_CTRY == "118")
                                {
                                    TxtCurrnCountry.Text = "KUWAIT";
                                }
                                else if (APP_PER_CTRY == "119")
                                {
                                    TxtCurrnCountry.Text = "KYRGYZSTAN";
                                }
                                else if (APP_PER_CTRY == "121")
                                {
                                    TxtCurrnCountry.Text = "LATVIA";
                                }
                                else if (APP_PER_CTRY == "122")
                                {
                                    TxtCurrnCountry.Text = "LEBANON";
                                }
                                else if (APP_PER_CTRY == "123")
                                {
                                    TxtCurrnCountry.Text = "LESOTHO";
                                }
                                else if (APP_PER_CTRY == "124")
                                {
                                    TxtCurrnCountry.Text = "LIBERIA";
                                }
                                else if (APP_PER_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYA";
                                }
                                else if (APP_PER_CTRY == "127")
                                {
                                    TxtCurrnCountry.Text = "LITHUANIA";
                                }
                                else if (APP_PER_CTRY == "128")
                                {
                                    TxtCurrnCountry.Text = "LUXEMBOURG";
                                }
                                else if (APP_PER_CTRY == "129")
                                {
                                    TxtCurrnCountry.Text = "MACAU";
                                }
                                else if (APP_PER_CTRY == "130")
                                {
                                    TxtCurrnCountry.Text = "MACEDONIA";
                                }
                                else if (APP_PER_CTRY == "131")
                                {
                                    TxtCurrnCountry.Text = "MADAGASCAR";
                                }
                                else if (APP_PER_CTRY == "132")
                                {
                                    TxtCurrnCountry.Text = "MALAWI";
                                }
                                else if (APP_PER_CTRY == "133")
                                {
                                    TxtCurrnCountry.Text = "MALAYSIA";
                                }
                                else if (APP_PER_CTRY == "134")
                                {
                                    TxtCurrnCountry.Text = "MALDIVES";
                                }
                                else if (APP_PER_CTRY == "135")
                                {
                                    TxtCurrnCountry.Text = "MALI";
                                }
                                else if (APP_PER_CTRY == "136")
                                {
                                    TxtCurrnCountry.Text = "MALTA";
                                }
                                else if (APP_PER_CTRY == "139")
                                {
                                    TxtCurrnCountry.Text = "MAURITANIA";
                                }
                                else if (APP_PER_CTRY == "140")
                                {
                                    TxtCurrnCountry.Text = "MAURITIUS";
                                }
                                else if (APP_PER_CTRY == "142")
                                {
                                    TxtCurrnCountry.Text = "MEXICO";
                                }
                                else if (APP_PER_CTRY == "144")
                                {
                                    TxtCurrnCountry.Text = "MOLDOVA";
                                }
                                else if (APP_PER_CTRY == "146")
                                {
                                    TxtCurrnCountry.Text = "MONGOLIA";
                                }
                                else if (APP_PER_CTRY == "147")
                                {
                                    TxtCurrnCountry.Text = "MONTSERRAT";
                                }
                                else if (APP_PER_CTRY == "148")
                                {
                                    TxtCurrnCountry.Text = "MOROCCA";
                                }
                                else if (APP_PER_CTRY == "149")
                                {
                                    TxtCurrnCountry.Text = "MOZAMBIQUE";
                                }
                                else if (APP_PER_CTRY == "150")
                                {
                                    TxtCurrnCountry.Text = "MYANMAR";
                                }
                                else if (APP_PER_CTRY == "151")
                                {
                                    TxtCurrnCountry.Text = "NAMIBIA";
                                }
                                else if (APP_PER_CTRY == "153")
                                {
                                    TxtCurrnCountry.Text = "NEPAL";
                                }
                                else if (APP_PER_CTRY == "154")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS";
                                }
                                else if (APP_PER_CTRY == "155")
                                {
                                    TxtCurrnCountry.Text = "NETHERLANDS ANTILLES";
                                }
                                else if (APP_PER_CTRY == "156")
                                {
                                    TxtCurrnCountry.Text = "NEW CALEDONIA";
                                }
                                else if (APP_PER_CTRY == "157")
                                {
                                    TxtCurrnCountry.Text = "NEW ZEALAND";
                                }
                                else if (APP_PER_CTRY == "158")
                                {
                                    TxtCurrnCountry.Text = "NICARAGUA";
                                }
                                else if (APP_PER_CTRY == "159")
                                {
                                    TxtCurrnCountry.Text = "NIGER";
                                }
                                else if (APP_PER_CTRY == "160")
                                {
                                    TxtCurrnCountry.Text = "NIGERIA";
                                }
                                else if (APP_PER_CTRY == "116")
                                {
                                    TxtCurrnCountry.Text = "NORTH KOREA";
                                }
                                else if (APP_PER_CTRY == "164")
                                {
                                    TxtCurrnCountry.Text = "NORWAY";
                                }
                                else if (APP_PER_CTRY == "165")
                                {
                                    TxtCurrnCountry.Text = "OMAN";
                                }
                                else if (APP_PER_CTRY == "166")
                                {
                                    TxtCurrnCountry.Text = "PAKISTAN";
                                }
                                else if (APP_PER_CTRY == "169")
                                {
                                    TxtCurrnCountry.Text = "PANAMA";
                                }
                                else if (APP_PER_CTRY == "170")
                                {
                                    TxtCurrnCountry.Text = "PAPUA NEW GUINEA";
                                }
                                else if (APP_PER_CTRY == "171")
                                {
                                    TxtCurrnCountry.Text = "PARAGUAY";
                                }
                                else if (APP_PER_CTRY == "172")
                                {
                                    TxtCurrnCountry.Text = "PERU";
                                }
                                else if (APP_PER_CTRY == "173")
                                {
                                    TxtCurrnCountry.Text = "PHILIPPINES";
                                }
                                else if (APP_PER_CTRY == "175")
                                {
                                    TxtCurrnCountry.Text = "POLAND";
                                }
                                else if (APP_PER_CTRY == "176")
                                {
                                    TxtCurrnCountry.Text = "PORTUGAL";
                                }
                                else if (APP_PER_CTRY == "178")
                                {
                                    TxtCurrnCountry.Text = "QATAR";
                                }
                                else if (APP_PER_CTRY == "180")
                                {
                                    TxtCurrnCountry.Text = "ROMANIA";
                                }
                                else if (APP_PER_CTRY == "181")
                                {
                                    TxtCurrnCountry.Text = "RUSSIA";
                                }
                                else if (APP_PER_CTRY == "182")
                                {
                                    TxtCurrnCountry.Text = "RWANDA";
                                }
                                else if (APP_PER_CTRY == "191")
                                {
                                    TxtCurrnCountry.Text = "SAUDI ARABIA";
                                }
                                else if (APP_PER_CTRY == "192")
                                {
                                    TxtCurrnCountry.Text = "SENEGAL";
                                }
                                else if (APP_PER_CTRY == "194")
                                {
                                    TxtCurrnCountry.Text = "SEYCHELLES";
                                }
                                else if (APP_PER_CTRY == "196")
                                {
                                    TxtCurrnCountry.Text = "SINGAPORE";
                                }
                                else if (APP_PER_CTRY == "197")
                                {
                                    TxtCurrnCountry.Text = "SLOVAKIA";
                                }
                                else if (APP_PER_CTRY == "198")
                                {
                                    TxtCurrnCountry.Text = "SLOVENIA";
                                }
                                else if (APP_PER_CTRY == "199")
                                {
                                    TxtCurrnCountry.Text = "SOLOMON ISLANDS";
                                }
                                else if (APP_PER_CTRY == "200")
                                {
                                    TxtCurrnCountry.Text = "SOMALIA";
                                }
                                else if (APP_PER_CTRY == "201")
                                {
                                    TxtCurrnCountry.Text = "SOUTH AFRICA";
                                }
                                else if (APP_PER_CTRY == "117")
                                {
                                    TxtCurrnCountry.Text = "SOUTH KOREA";
                                }
                                else if (APP_PER_CTRY == "203")
                                {
                                    TxtCurrnCountry.Text = "SPAIN";
                                }
                                else if (APP_PER_CTRY == "204")
                                {
                                    TxtCurrnCountry.Text = "SRI LANKA";
                                }
                                else if (APP_PER_CTRY == "205")
                                {
                                    TxtCurrnCountry.Text = "SUDAN";
                                }
                                else if (APP_PER_CTRY == "206")
                                {
                                    TxtCurrnCountry.Text = "SURINAME";
                                }
                                else if (APP_PER_CTRY == "208")
                                {
                                    TxtCurrnCountry.Text = "SWAZILAND";
                                }
                                else if (APP_PER_CTRY == "209")
                                {
                                    TxtCurrnCountry.Text = "SWEDEN";
                                }
                                else if (APP_PER_CTRY == "210")
                                {
                                    TxtCurrnCountry.Text = "SWITZERLAND";
                                }
                                else if (APP_PER_CTRY == "212")
                                {
                                    TxtCurrnCountry.Text = "TAIWAN";
                                }
                                else if (APP_PER_CTRY == "213")
                                {
                                    TxtCurrnCountry.Text = "TAJIKISTHAN";
                                }
                                else if (APP_PER_CTRY == "214")
                                {
                                    TxtCurrnCountry.Text = "TANZANIA";
                                }
                                else if (APP_PER_CTRY == "215")
                                {
                                    TxtCurrnCountry.Text = "THAILAND";
                                }
                                else if (APP_PER_CTRY == "217")
                                {
                                    TxtCurrnCountry.Text = "TOGO REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "219")
                                {
                                    TxtCurrnCountry.Text = "TONGA";
                                }
                                else if (APP_PER_CTRY == "220")
                                {
                                    TxtCurrnCountry.Text = "TRINIDAD AND TOBAGO";
                                }
                                else if (APP_PER_CTRY == "221")
                                {
                                    TxtCurrnCountry.Text = "TUNISIA";
                                }
                                else if (APP_PER_CTRY == "222")
                                {
                                    TxtCurrnCountry.Text = "TURKEY";
                                }
                                else if (APP_PER_CTRY == "223")
                                {
                                    TxtCurrnCountry.Text = "TURKMENISTAN";
                                }
                                else if (APP_PER_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "U A E";
                                }
                                else if (APP_PER_CTRY == "226")
                                {
                                    TxtCurrnCountry.Text = "UGANDA";
                                }
                                else if (APP_PER_CTRY == "228")
                                {
                                    TxtCurrnCountry.Text = "UNITED ARAB EMIRATES";
                                }
                                else if (APP_PER_CTRY == "229")
                                {
                                    TxtCurrnCountry.Text = "UNITED KINGDOM";
                                }
                                else if (APP_PER_CTRY == "232")
                                {
                                    TxtCurrnCountry.Text = "URUGUAY";
                                }
                                else if (APP_PER_CTRY == "230")
                                {
                                    TxtCurrnCountry.Text = "USA";
                                }
                                else if (APP_PER_CTRY == "234")
                                {
                                    TxtCurrnCountry.Text = "VANUATU";
                                }
                                else if (APP_PER_CTRY == "235")
                                {
                                    TxtCurrnCountry.Text = "VENEZUELA";
                                }
                                else if (APP_PER_CTRY == "236")
                                {
                                    TxtCurrnCountry.Text = "VIETNAM";
                                }
                                else if (APP_PER_CTRY == "241")
                                {
                                    TxtCurrnCountry.Text = "YEMEN";
                                }
                                else if (APP_PER_CTRY == "242")
                                {
                                    TxtCurrnCountry.Text = "ZAMBIA";
                                }
                                else if (APP_PER_CTRY == "243")
                                {
                                    TxtCurrnCountry.Text = "ZIMBABWE";
                                }
                                else if (APP_PER_CTRY == "002")
                                {
                                    TxtCurrnCountry.Text = "ALAND ISLANDS";
                                }
                                else if (APP_PER_CTRY == "005")
                                {
                                    TxtCurrnCountry.Text = "AMERICAN SAMOA";
                                }
                                else if (APP_PER_CTRY == "006")
                                {
                                    TxtCurrnCountry.Text = "ANDORRA";
                                }
                                else if (APP_PER_CTRY == "008")
                                {
                                    TxtCurrnCountry.Text = "ANGUILLA";
                                }
                                else if (APP_PER_CTRY == "009")
                                {
                                    TxtCurrnCountry.Text = "ANTARCTICA";
                                }
                                else if (APP_PER_CTRY == "010")
                                {
                                    TxtCurrnCountry.Text = "ANTIGUA AND BARBUDA";
                                }
                                else if (APP_PER_CTRY == "030")
                                {
                                    TxtCurrnCountry.Text = "BOUVET ISLAND";
                                }
                                else if (APP_PER_CTRY == "032")
                                {
                                    TxtCurrnCountry.Text = "BRITISH INDIAN OCEAN TERRITORY";
                                }
                                else if (APP_PER_CTRY == "046")
                                {
                                    TxtCurrnCountry.Text = "CHRISTMAS ISLAND";
                                }
                                else if (APP_PER_CTRY == "047")
                                {
                                    TxtCurrnCountry.Text = "COCOS (KEELING) ISLANDS";
                                }
                                else if (APP_PER_CTRY == "067")
                                {
                                    TxtCurrnCountry.Text = "ERITREA";
                                }
                                else if (APP_PER_CTRY == "070")
                                {
                                    TxtCurrnCountry.Text = "FALKLAND ISLANDS (MALVINAS)";
                                }
                                else if (APP_PER_CTRY == "071")
                                {
                                    TxtCurrnCountry.Text = "FAROE ISLANDS";
                                }
                                else if (APP_PER_CTRY == "077")
                                {
                                    TxtCurrnCountry.Text = "FRENCH SOUTHERN TERRITORIES";
                                }
                                else if (APP_PER_CTRY == "095")
                                {
                                    TxtCurrnCountry.Text = "HEARD ISLAND AND MCDONALD ISLANDS";
                                }
                                else if (APP_PER_CTRY == "096")
                                {
                                    TxtCurrnCountry.Text = "HOLY SEE (VATICAN CITY STATE)";
                                }
                                else if (APP_PER_CTRY == "099")
                                {
                                    TxtCurrnCountry.Text = "HUNGARY";
                                }
                                else if (APP_PER_CTRY == "103")
                                {
                                    TxtCurrnCountry.Text = "IRAN, ISLAMIC REPUBLIC OF";
                                }
                                else if (APP_PER_CTRY == "106")
                                {
                                    TxtCurrnCountry.Text = "ISLE OF MAN";
                                }
                                else if (APP_PER_CTRY == "115")
                                {
                                    TxtCurrnCountry.Text = "KIRIBATI";
                                }
                                else if (APP_PER_CTRY == "120")
                                {
                                    TxtCurrnCountry.Text = "LAO PEOPLE'S DEMOCRATIC REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "125")
                                {
                                    TxtCurrnCountry.Text = "LIBYAN ARAB JAMAHIRIYA";
                                }
                                else if (APP_PER_CTRY == "126")
                                {
                                    TxtCurrnCountry.Text = "LIECHTENSTEIN";
                                }
                                else if (APP_PER_CTRY == "137")
                                {
                                    TxtCurrnCountry.Text = "MARSHALL ISLANDS";
                                }
                                else if (APP_PER_CTRY == "138")
                                {
                                    TxtCurrnCountry.Text = "MARTINIQUE";
                                }
                                else if (APP_PER_CTRY == "141")
                                {
                                    TxtCurrnCountry.Text = "MAYOTTE";
                                }
                                else if (APP_PER_CTRY == "143")
                                {
                                    TxtCurrnCountry.Text = "MICRONESIA, FEDERATED STATES OF";
                                }
                                else if (APP_PER_CTRY == "145")
                                {
                                    TxtCurrnCountry.Text = "MONACO";
                                }
                                else if (APP_PER_CTRY == "152")
                                {
                                    TxtCurrnCountry.Text = "NAURU";
                                }
                                else if (APP_PER_CTRY == "161")
                                {
                                    TxtCurrnCountry.Text = "NIUE";
                                }
                                else if (APP_PER_CTRY == "162")
                                {
                                    TxtCurrnCountry.Text = "NORFOLK ISLAND";
                                }
                                else if (APP_PER_CTRY == "163")
                                {
                                    TxtCurrnCountry.Text = "NORTHERN MARIANA ISLANDS";
                                }
                                else if (APP_PER_CTRY == "167")
                                {
                                    TxtCurrnCountry.Text = "PALAU";
                                }
                                else if (APP_PER_CTRY == "168")
                                {
                                    TxtCurrnCountry.Text = "PALESTINIAN TERRITORY, OCCUPIED";
                                }
                                else if (APP_PER_CTRY == "174")
                                {
                                    TxtCurrnCountry.Text = "PITCAIRN";
                                }
                                else if (APP_PER_CTRY == "177")
                                {
                                    TxtCurrnCountry.Text = "PUERTO RICO";
                                }
                                else if (APP_PER_CTRY == "179")
                                {
                                    TxtCurrnCountry.Text = "REUNION";
                                }
                                else if (APP_PER_CTRY == "183")
                                {
                                    TxtCurrnCountry.Text = "SAINT HELENA";
                                }
                                else if (APP_PER_CTRY == "184")
                                {
                                    TxtCurrnCountry.Text = "SAINT KITTS AND NEVIS";
                                }
                                else if (APP_PER_CTRY == "185")
                                {
                                    TxtCurrnCountry.Text = "SAINT LUCIA";
                                }
                                else if (APP_PER_CTRY == "186")
                                {
                                    TxtCurrnCountry.Text = "SAINT PIERRE AND MIQUELON";
                                }
                                else if (APP_PER_CTRY == "187")
                                {
                                    TxtCurrnCountry.Text = "SAINT VINCENT AND THE GRENADINES";
                                }
                                else if (APP_PER_CTRY == "188")
                                {
                                    TxtCurrnCountry.Text = "SAMOA";
                                }
                                else if (APP_PER_CTRY == "189")
                                {
                                    TxtCurrnCountry.Text = "SAN MARINO";
                                }
                                else if (APP_PER_CTRY == "190")
                                {
                                    TxtCurrnCountry.Text = "SAO TOME AND PRINCIPE";
                                }
                                else if (APP_PER_CTRY == "193")
                                {
                                    TxtCurrnCountry.Text = "SERBIA AND MONTENEGRO";
                                }
                                else if (APP_PER_CTRY == "195")
                                {
                                    TxtCurrnCountry.Text = "SIERRA LEONE";
                                }
                                else if (APP_PER_CTRY == "202")
                                {
                                    TxtCurrnCountry.Text = "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS";
                                }
                                else if (APP_PER_CTRY == "207")
                                {
                                    TxtCurrnCountry.Text = "SVALBARD AND JAN MAYEN";
                                }
                                else if (APP_PER_CTRY == "211")
                                {
                                    TxtCurrnCountry.Text = "SYRIAN ARAB REPUBLIC";
                                }
                                else if (APP_PER_CTRY == "216")
                                {
                                    TxtCurrnCountry.Text = "TIMOR-LESTE";
                                }
                                else if (APP_PER_CTRY == "218")
                                {
                                    TxtCurrnCountry.Text = "TOKELAU";
                                }
                                else if (APP_PER_CTRY == "224")
                                {
                                    TxtCurrnCountry.Text = "TURKS AND CAICOS ISLANDS";
                                }
                                else if (APP_PER_CTRY == "225")
                                {
                                    TxtCurrnCountry.Text = "TUVALU";
                                }
                                else if (APP_PER_CTRY == "227")
                                {
                                    TxtCurrnCountry.Text = "UKRAINE";
                                }
                                else if (APP_PER_CTRY == "231")
                                {
                                    TxtCurrnCountry.Text = "UNITED STATES MINOR OUTLYING ISLANDS";
                                }
                                else if (APP_PER_CTRY == "233")
                                {
                                    TxtCurrnCountry.Text = "UZBEKISTAN";
                                }
                                else if (APP_PER_CTRY == "237")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, BRITISH";
                                }
                                else if (APP_PER_CTRY == "238")
                                {
                                    TxtCurrnCountry.Text = "VIRGIN ISLANDS, U.S.";
                                }
                                else if (APP_PER_CTRY == "239")
                                {
                                    TxtCurrnCountry.Text = "WALLIS AND FUTUNA";
                                }
                                else if (APP_PER_CTRY == "240")
                                {
                                    TxtCurrnCountry.Text = "WESTERN SAHARA";
                                }
                            }
                            else if (childNode.Name == "APP_PER_ADD_PROOF")
                            {
                                string APP_PER_ADD_PROOF = childNode.InnerText;

                                if (APP_PER_ADD_PROOF == "01")
                                {
                                    TxtPrmAddrProof.Text = "Passport";
                                }
                                else if (APP_PER_ADD_PROOF == "06")
                                {
                                    TxtPrmAddrProof.Text = "Voter Identity Card";
                                }
                                else if (APP_PER_ADD_PROOF == "07")
                                {
                                    TxtPrmAddrProof.Text = "Ration Card";
                                }
                                else if (APP_PER_ADD_PROOF == "08")
                                {
                                    TxtPrmAddrProof.Text = "Registered Lease / Sale Agreement of Residence";
                                }
                                else if (APP_PER_ADD_PROOF == "02")
                                {
                                    TxtPrmAddrProof.Text = "Driving License";
                                }
                                else if (APP_PER_ADD_PROOF == "13")
                                {
                                    TxtPrmAddrProof.Text = "Flat Maintenance Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "14")
                                {
                                    TxtPrmAddrProof.Text = "Insurance copy";
                                }
                                else if (APP_PER_ADD_PROOF == "09")
                                {
                                    TxtPrmAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "10")
                                {
                                    TxtPrmAddrProof.Text = "Latest Land Line Telephone Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "11")
                                {
                                    TxtPrmAddrProof.Text = "Gas Bill";
                                }
                                else if (APP_PER_ADD_PROOF == "03")
                                {
                                    TxtPrmAddrProof.Text = "Latest Bank Passbook";
                                }
                                else if (APP_PER_ADD_PROOF == "04")
                                {
                                    TxtPrmAddrProof.Text = "Latest Bank Account Statement";
                                }
                                else if (APP_PER_ADD_PROOF == "15")
                                {
                                    TxtPrmAddrProof.Text = "Self Declaration by High Court / Supreme Court Judge";
                                }
                                else if (APP_PER_ADD_PROOF == "17")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Scheduled Commercial Banks / Scheduled Co-operative Banks / Multinational Foreign banks.";
                                }
                                else if (APP_PER_ADD_PROOF == "22")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Gazetted Officer";
                                }
                                else if (APP_PER_ADD_PROOF == "21")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Notary Public";
                                }
                                else if (APP_PER_ADD_PROOF == "18")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Elected representatives to the Legislative Assembly";
                                }
                                else if (APP_PER_ADD_PROOF == "19")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by Parliament";
                                }
                                else if (APP_PER_ADD_PROOF == "12")
                                {
                                    TxtPrmAddrProof.Text = "Registration Certificate issued under Shops and Establishments Act";
                                }
                                else if (APP_PER_ADD_PROOF == "20")
                                {
                                    TxtPrmAddrProof.Text = "Proof of Address issued by any Government / Statutory Authority";
                                }
                                else if (APP_PER_ADD_PROOF == "23")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Central / State Government ";
                                }
                                else if (APP_PER_ADD_PROOF == "24")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Statutory / Regulatory Authorities";
                                }
                                else if (APP_PER_ADD_PROOF == "25")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Public Sector Undertakings";
                                }
                                else if (APP_PER_ADD_PROOF == "26")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Scheduled Commercial Banks";
                                }
                                else if (APP_PER_ADD_PROOF == "27")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Public Financial Institutions";
                                }
                                else if (APP_PER_ADD_PROOF == "28")
                                {
                                    TxtPrmAddrProof.Text = "ID Card with address issued by Colleges affiliated to universities";
                                }
                                else if (APP_PER_ADD_PROOF == "29")
                                {
                                    TxtPrmAddrProof.Text = "ID Card issued by Professional Bodies such as ICAI, ICWAI, ICSI, Bar Council, etc. to their Members";
                                }
                                else if (APP_PER_ADD_PROOF == "16")
                                {
                                    TxtPrmAddrProof.Text = "Power of Attorney given by FII/sub-account to the Custodians (which are duly notarised and/or apostiled or consularised) giving registered address.";
                                }
                                else if (APP_PER_ADD_PROOF == "31")
                                {
                                    TxtPrmAddrProof.Text = "Unique Identification Number (UID) (Aadhaar)";
                                }
                                else if (APP_PER_ADD_PROOF == "05")
                                {
                                    TxtPrmAddrProof.Text = "Latest Demat Account Statement";
                                }


                            }
                            else if (childNode.Name == "APP_PER_ADD_REF")
                            {
                                TxtPrmAddrRef.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_PER_ADD_DT")
                            {
                                TxtPerAddrDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INCOME")
                            {
                                string APP_INCOME = childNode.InnerText;

                                if (APP_INCOME == "01")
                                {
                                    TxtIncome.Text = "Below Rs. 1  Lac";
                                }
                                else if (APP_INCOME == "02")
                                {
                                    TxtIncome.Text = "Btw Rs. 1 to Rs. 5 Lacs";
                                }
                                else if (APP_INCOME == "03")
                                {
                                    TxtIncome.Text = "Btw Rs. 5 to Rs. 10 Lacs";
                                }
                                else if (APP_INCOME == "04")
                                {
                                    TxtIncome.Text = "Btw Rs. 10 to Rs. 25 Lacs";
                                }
                                else if (APP_INCOME == "05")
                                {
                                    TxtIncome.Text = "More than Rs. 25 Lacs";
                                }

                            }
                            else if (childNode.Name == "APP_OCC")
                            {
                                string APP_OCC = childNode.InnerText;

                                if (APP_OCC == "02")
                                {
                                    TxtOcc.Text = "Public Sector";
                                }
                                else if (APP_OCC == "01")
                                {
                                    TxtOcc.Text = "Private Sector";
                                }
                                else if (APP_OCC == "10")
                                {
                                    TxtOcc.Text = "Government Service";
                                }
                                else if (APP_OCC == "03")
                                {
                                    TxtOcc.Text = "Business";
                                }
                                else if (APP_OCC == "04")
                                {
                                    TxtOcc.Text = "Professional";
                                }
                                else if (APP_OCC == "05")
                                {
                                    TxtOcc.Text = "Agriculturist";
                                }
                                else if (APP_OCC == "06")
                                {
                                    TxtOcc.Text = "Retired";
                                }
                                else if (APP_OCC == "07")
                                {
                                    TxtOcc.Text = "Housewife";
                                }
                                else if (APP_OCC == "08")
                                {
                                    TxtOcc.Text = "Student";
                                }
                                else if (APP_OCC == "99")
                                {
                                    TxtOcc.Text = "Others (please specify)";
                                }
                            }
                            else if (childNode.Name == "APP_OTH_OCC")
                            {
                                TxtOthOcc.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_POL_CONN")
                            {
                                string APP_POL_CONN = childNode.InnerText;

                                if (APP_POL_CONN == "00" || APP_POL_CONN == "NA")
                                {
                                    TxtPoExposedPerson.Text = "Not Applicable";
                                }
                                else if (APP_POL_CONN == "01" || APP_POL_CONN == "RPEP")
                                {
                                    TxtPoExposedPerson.Text = "PEP";
                                }
                                else if (APP_POL_CONN == "02" || APP_POL_CONN == "PEP")
                                {
                                    TxtPoExposedPerson.Text = " Related to a PEP";
                                }

                            }
                            else if (childNode.Name == "APP_DOC_PROOF")
                            {
                                string APP_DOC_PROOF = childNode.InnerText;

                                if (APP_DOC_PROOF == "S")
                                {
                                    TxtDocProof.Text = "Self Certified Copies Submitted (Originals Verified)";
                                }
                                else if (APP_DOC_PROOF == "T")
                                {
                                    TxtDocProof.Text = "True Copies of Documents Received";
                                }
                            }
                            else if (childNode.Name == "APP_INTERNAL_REF")
                            {
                                TxtInternalRef.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_BRANCH_CODE")
                            {
                                TxtBranchCode.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_MAR_STATUS")
                            {
                                string APP_MAR_STATUS = childNode.InnerText;

                                if (APP_MAR_STATUS == "01")
                                {
                                    TxtMaritalStatus.Text = "Married";
                                }
                                else if (APP_MAR_STATUS == "02")
                                {
                                    TxtMaritalStatus.Text = "Unmarried";
                                }
                            }
                            else if (childNode.Name == "APP_NETWRTH")
                            {
                                TxtNetWrth.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_NETWORTH_DT")
                            {
                                TxtNetWrtDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_INCORP_PLC")
                            {
                                TxtIncorpPlc.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHERINFO")
                            {
                                TxtOthrInfo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_REMARKS")
                            {
                                TxtRemarks.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER1")
                            {
                                TxtFiller1.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER2")
                            {
                                TxtFiller2.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_FILLER3")
                            {
                                TxtFiller3.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_STATUS")
                            {
                                string APP_STATUS = childNode.InnerText;

                                if (APP_STATUS == "01" || APP_STATUS == "11")
                                {
                                    TxtStatus.Text = " UNDER_PROCESS";
                                }
                                else if (APP_STATUS == "02" || APP_STATUS == "12")
                                {
                                    TxtStatus.Text = " KYC REGISTERED ";
                                }
                                else if (APP_STATUS == "03" || APP_STATUS == "13")
                                {
                                    TxtStatus.Text = " ON HOLD ";
                                }
                                else if (APP_STATUS == "04")
                                {
                                    TxtStatus.Text = " KYC REJECTED";
                                }
                                else if (APP_STATUS == "05")
                                {
                                    TxtStatus.Text = "NOT AVAILABLE";
                                }
                                else if (APP_STATUS == "06")
                                {
                                    TxtStatus.Text = "Deactivate";
                                }
                                else if (APP_STATUS == "14")
                                {
                                    TxtStatus.Text = "KYC REJECTED ";
                                }
                                else if (APP_STATUS == "21")
                                {
                                    TxtStatus.Text = "Mutual Fund under process";
                                }
                                else if (APP_STATUS == "22")
                                {
                                    TxtStatus.Text = "Mutual Fund verified";
                                }
                            }
                            else if (childNode.Name == "APP_STATUSDT")
                            {
                                TxtStatusDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_ERROR_DESC")
                            {
                                TxtErrorDesc.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_DUMP_TYPE")
                            {
                                string APP_DUMP_TYPE = childNode.InnerText;

                                if (APP_DUMP_TYPE == "I")
                                {
                                    TxtDumpType.Text = "Incremental";
                                }
                                else if (APP_DUMP_TYPE == "F")
                                {
                                    TxtDumpType.Text = "Full Download";
                                }
                                else if (APP_DUMP_TYPE == "P")
                                {
                                    TxtDumpType.Text = "Partial Download";
                                }
                                else if (APP_DUMP_TYPE == "E")
                                {
                                    TxtDumpType.Text = "EOD";
                                }
                                else if (APP_DUMP_TYPE == "S")
                                {
                                    TxtDumpType.Text = "Solicited";
                                }
                                else if (APP_DUMP_TYPE == "U")
                                {
                                    TxtDumpType.Text = "Unsolicited";
                                }
                            }
                            else if (childNode.Name == "APP_DNLDDT")
                            {
                                TxtDnldDt.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_KRA_INFO")
                            {
                                TxtKraInfo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_KYC_MODE")
                            {
                                string APP_KYC_MODE = childNode.InnerText;
                                if (APP_KYC_MODE == "01" || APP_KYC_MODE == "1")
                                {
                                    TxtKycMode.Text = "New";
                                }
                                else if (APP_KYC_MODE == "02" || APP_KYC_MODE == "2")
                                {
                                    TxtKycMode.Text = "Modify with documents";
                                }
                                else if (APP_KYC_MODE == "03" || APP_KYC_MODE == "3")
                                {
                                    TxtKycMode.Text = "Modify without documents";
                                }
                                else if (APP_KYC_MODE == "04" || APP_KYC_MODE == "4")
                                {
                                    TxtKycMode.Text = "Delete";
                                }
                            }
                            else if (childNode.Name == "APP_VAULT_REF")
                            {
                                string APP_VAULT_REF = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_UID_TOKEN")
                            {
                                string APP_UID_TOKEN = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_VER_NO")
                            {
                                string APP_VER_NO = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_SIGN")
                            {
                                string APP_SIGN = childNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode xmlNode_Summary in OutPut_doc.GetElementsByTagName("APP_SUMM_REC"))
                    {
                        foreach (XmlNode childNode in xmlNode_Summary.ChildNodes)
                        {
                            if (childNode.Name == "APP_REQ_DATE")
                            {
                                string APP_REQ_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHKRA_BATCH")
                            {
                                string APP_OTHKRA_BATCH = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHKRA_CODE")
                            {
                                string APP_OTHKRA_CODE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RESPONSE_DATE")
                            {
                                string APP_RESPONSE_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_TOTAL_REC")
                            {
                                string APP_TOTAL_REC = childNode.InnerText;
                            }
                        }
                    }

                    //Added by Mohan on 29-11-2021
                    foreach (XmlNode xmlNode_Summary in OutPut_doc.GetElementsByTagName("APP_PAN_SUMM"))
                    {
                        foreach (XmlNode childNode in xmlNode_Summary.ChildNodes)
                        {
                            if (childNode.Name == "APP_REQ_DATE")
                            {
                                string APP_REQ_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHKRA_BATCH")
                            {
                                string APP_OTHKRA_BATCH = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_OTHKRA_CODE")
                            {
                                string APP_OTHKRA_CODE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_RESPONSE_DATE")
                            {
                                string APP_RESPONSE_DATE = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_TOTAL_REC")
                            {
                                string APP_TOTAL_REC = childNode.InnerText;
                            }
                        }
                    }

                    foreach (XmlNode xmlNode_Error in OutPut_doc.GetElementsByTagName("ERROR"))
                    {
                        foreach (XmlNode childNode in xmlNode_Error.ChildNodes)
                        {
                            if (childNode.Name == "ERROR_CODE")
                            {
                                string ErrorCode = childNode.InnerText;
                            }
                            else if (childNode.Name == "ERROR_MSG")
                            {
                                ErrorMsg = childNode.InnerText;
                            }
                        }
                    }
                    if (ErrorMsg != null)
                    {
                        MessageBox.Show(ErrorMsg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label25_Click(object sender, System.EventArgs e)
        {

        }

        private void frmGetPanDetailsAll_Load(object sender, System.EventArgs e)
        {
            try
            {


                CboKraCode.Items.Clear();
                CboKraCode.Items.Add("CVLKRA");
                CboKraCode.Items.Add("NDML");
                CboKraCode.Items.Add("DOTEX");
                CboKraCode.Items.Add("CAMS");
                CboKraCode.Items.Add("KARVY");

                CboFetchType.Items.Clear();
                CboFetchType.Items.Add("Data And Image");
                CboFetchType.Items.Add("Only Data");
                CboFetchType.Items.Add("Only Image");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      

        private void label15_Click(object sender, System.EventArgs e)
        {

        }     

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

    }
}

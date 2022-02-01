using DevExpress.Web.Office;
using System;
using System.Collections.Generic;
using System.Web;

namespace FBG.Market.Web.Identity
{
    public class RichEditHelper
    {
        private static string OpenedCanceledDocumentIDsSessionKey = "OpenedCanceledDocumentIDsSessionKey";
        private static List<string> OpenedCanceledDocumentIDs
        {
            get
            {
                if (HttpContext.Current.Session[OpenedCanceledDocumentIDsSessionKey] == null)
                {
                    HttpContext.Current.Session[OpenedCanceledDocumentIDsSessionKey] = new List<string>();
                }
                return (List<string>)HttpContext.Current.Session[OpenedCanceledDocumentIDsSessionKey];
            }
            set { HttpContext.Current.Session[OpenedCanceledDocumentIDsSessionKey] = value; }
        }
        public static string GetDocumentID(string keyValue)
        {
            string documentID;
            //TODO: For per-user editing, construct the DocumentID using the row's key plus user info, 
            //for example, System.Web.UI.User.Identity.Name. 
            //Then, close the document for editing by this DocumentID for this user only.
            if (keyValue == "")
                documentID = Guid.NewGuid().ToString();
            else
                documentID = keyValue;

            if (!OpenedCanceledDocumentIDs.Contains(documentID))
            {
                OpenedCanceledDocumentIDs.Add(documentID);
            }
            return documentID;
        }
        public static void CloseUnnecessaryDocuments()
        {
            foreach (string id in OpenedCanceledDocumentIDs)
                DocumentManager.CloseDocument(id);

            OpenedCanceledDocumentIDs.Clear();
        }
    }
}
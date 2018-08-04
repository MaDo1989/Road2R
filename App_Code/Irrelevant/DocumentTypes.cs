using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DocumentTypes
/// </summary>
public class DocumentTypes
{
    int documentTypeID;
    string documentType;

    public int DocumentTypeID
    {
        get
        {
            return documentTypeID;
        }

        set
        {
            documentTypeID = value;
        }
    }

    public string DocumentType
    {
        get
        {
            return documentType;
        }

        set
        {
            documentType = value;
        }
    }

    public DocumentTypes()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DocumentTypes(int documentTypeID, string documentType)
    {
        DocumentTypeID = documentTypeID;
        DocumentType = documentType;
    }

    
            public List<DocumentTypes> getDocumentTypesList()
    {
        #region DB functions
        string query = "select * from DocumentTypes order by DocumentType";

        List<DocumentTypes> list = new List<DocumentTypes>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            DocumentTypes tmp = new DocumentTypes((int)dr["DocumentTypeID"], dr["DocumentType"].ToString());
            list.Add(tmp);
        }
        #endregion

        return list;

    }
}
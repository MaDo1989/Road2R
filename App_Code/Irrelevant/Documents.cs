using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Documents
/// </summary>
public class Documents
{
    int documentID;
    string documentName;
    string url;
    string active;
    float totalPrice;
    string comments;
    string containerID;
    DateTime date;
    DocumentTypes docType;
    Drivers sendBy;
    Orders relatedOrder;
    int imgID;

    public int DocumentID
    {
        get
        {
            return documentID;
        }

        set
        {
            documentID = value;
        }
    }

    public string DocumentName
    {
        get
        {
            return documentName;
        }

        set
        {
            documentName = value;
        }
    }

    public string Url
    {
        get
        {
            return url;
        }

        set
        {
            url = value;
        }
    }

    public string Active
    {
        get
        {
            return active;
        }

        set
        {
            active = value;
        }
    }

    public float TotalPrice
    {
        get
        {
            return totalPrice;
        }

        set
        {
            totalPrice = value;
        }
    }

    public string Comments
    {
        get
        {
            return comments;
        }

        set
        {
            comments = value;
        }
    }

    public DateTime Date
    {
        get
        {
            return date;
        }

        set
        {
            date = value;
        }
    }

    public DocumentTypes DocType
    {
        get
        {
            return docType;
        }

        set
        {
            docType = value;
        }
    }

    public Drivers SendBy
    {
        get
        {
            return sendBy;
        }

        set
        {
            sendBy = value;
        }
    }

    public string ContainerID
    {
        get
        {
            return containerID;
        }

        set
        {
            containerID = value;
        }
    }

    public Orders RelatedOrder
    {
        get
        {
            return relatedOrder;
        }

        set
        {
            relatedOrder = value;
        }
    }

    public int ImgID
    {
        get
        {
            return imgID;
        }

        set
        {
            imgID = value;
        }
    }

    //missing the orderline!!

    public Documents()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Documents(int documentID, string documentName, string url, string active, float totalPrice, string comments, string containerID, DateTime date, DocumentTypes docType, Drivers sendBy, Orders relatedOrder)
    {
        DocumentID = documentID;
        DocumentName = documentName;
        Url = url;
        Active = active;
        TotalPrice = totalPrice;
        Comments = comments;
        ContainerID = containerID;
        Date = date;
        DocType = docType;
        SendBy = sendBy;
        RelatedOrder = relatedOrder;
    }

    public Documents(int documentID, string documentName, string url, float totalPrice, string comments, string containerID, DateTime date, DocumentTypes docType, Drivers sendBy, Orders relatedOrder)
    {
        DocumentID = documentID;
        DocumentName = documentName;
        Url = url;
        TotalPrice = totalPrice;
        Comments = comments;
        ContainerID = containerID;
        Date = date;
        DocType = docType;
        SendBy = sendBy;
        RelatedOrder = relatedOrder;
    }

    public Documents(int documentID, string documentName, float totalPrice, string comments, string containerID, DateTime date, DocumentTypes docType, Drivers sendBy, Orders relatedOrder, int imgID)
    {
        DocumentID = documentID;
        DocumentName = documentName;
        TotalPrice = totalPrice;
        Comments = comments;
        ContainerID = containerID;
        Date = date;
        DocType = docType;
        SendBy = sendBy;
        RelatedOrder = relatedOrder;
        ImgID = imgID;
    }

    public List<Documents> getDocumentsList(bool active, int selectedDocumentTypeID, DateTime startDate, DateTime endDate)
    {


        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndDate = endDate.ToString("yyyy-MM-dd");
        

        string query = "select d.DocumentID, d.DocumentName, dt.DocumentTypeID, dt.DocumentType, d.Active, d.DriverID, dr.FirstName, dr.LastName, d.HasImg, d.ImgID, d.TotalPrice, d.Comments, d.Date, d.ContainerID, o.OrderID, o.OrderName, c.CustomerID, c.CustomerName"
            + " from Documents d"
            + " inner join DocumentTypes dt on d.DocumentTypeID = dt.DocumentTypeID"
            + " inner join Orders o on o.OrderID = d.OrderID"
            + " inner join Customers c on o.CustomerID = c.CustomerID"
            + " inner join Drivers dr on d.DriverID = dr.DriverID";
        if (active == true)
        {
            query += " where d.Active = 'Y'";
        }
        else
        {
            query += " where 1=1";
        }


        if (selectedDocumentTypeID != -1)
        {
            query += " and dt.DocumentTypeID =" + selectedDocumentTypeID;
        }
        if (startDate.Year != 1)
        {
            query += " and d.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and d.Date <= '" + sqlEndDate + "'";
            }
        }

        query += " order by d.Date desc";

        List<Documents> list = new List<Documents>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Documents tmp = new Documents();
            tmp.DocumentID = (int)dr["DocumentID"];
            tmp.DocumentName = dr["DocumentName"].ToString();
            tmp.Active = dr["Active"].ToString();
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.Comments = dr["Comments"].ToString();
            tmp.ContainerID = dr["ContainerID"].ToString();
            tmp.Date = (DateTime)dr["Date"];
            tmp.DocType = new DocumentTypes((int)dr["DocumentTypeID"], dr["DocumentType"].ToString());
            if (dr["HasImg"].ToString() == "Y")
            {
                tmp.ImgID = (int)dr["ImgID"];
            }
            Drivers d = new Drivers();
            d.DriverID = (int)dr["DriverID"];
            d.FirstName = dr["FirstName"].ToString();
            d.LastName = dr["LastName"].ToString();
            //tmp.SendBy = d.getDriver();
            tmp.SendBy = d;
            Orders o = new Orders();
            o.OrderID = (int)dr["OrderID"];
            o.OrderName = dr["OrderName"].ToString();
            o.Customer = new Customers();
            o.Customer.CustomerID = (int)dr["CustomerID"];
            o.Customer.CustomerName = dr["CustomerName"].ToString();
            tmp.RelatedOrder = o;

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public Documents getDocument()
    {
        #region DB functions
        string query = "select * from Documents d inner join DocumentTypes dt on d.DocumentTypeID = dt.DocumentTypeID where DocumentID =" + DocumentID + "";

        Documents d = new Documents();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            d.DocumentID = (int)dr["DocumentID"];
            d.DocumentName = dr["DocumentName"].ToString();

            if (dr["HasImg"].ToString() == "Y")
            {
                d.ImgID = (int)dr["ImgID"];
            }

            d.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            d.Comments = dr["Comments"].ToString();
            d.ContainerID = dr["ContainerID"].ToString();
            d.Active = dr["Active"].ToString();
            d.Date = (DateTime)dr["Date"];
            d.DocType = new DocumentTypes((int)dr["DocumentTypeID"], dr["DocumentType"].ToString());

            Drivers driver = new Drivers();
            driver.DriverID = (int)dr["DriverID"];
            d.SendBy = driver.getDriver();

            Orders o = new Orders();
            o.OrderID = (int)dr["OrderID"];
            d.RelatedOrder = o.getOrder();

        }
        #endregion

        return d;
    }

    public void setDocument(string func)
    {

        string sqlFormattedDate = Date.ToString("yyyy-MM-dd");
        
        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Documents SET DocumentName = '" + DocumentName + "', DocumentTypeID = '" + DocType.DocumentTypeID + "', TotalPrice = " + TotalPrice + ", Comments = '" + Comments + "', Date = '" + sqlFormattedDate + "', DriverID = " + SendBy.DriverID + ", ContainerID = '" + ContainerID + "', OrderID = " + RelatedOrder.OrderID + " WHERE DocumentID = " + DocumentID;
        }
        else if (func == "new")
        {
            query = "insert into Documents values ('" + DocumentName + "'," + DocType.DocumentTypeID + "," + TotalPrice + ",'" + Comments + "','" + sqlFormattedDate + "','Y'," + SendBy.DriverID + ", '" + ContainerID + "'," + RelatedOrder.OrderID + ", NULL , 'N')";
        }
        db.ExecuteQuery(query);
    }

    public void setDocumentApp(string func)
    {
        string sqlFormattedDate = Date.ToString("yyyy-MM-dd");

        DbService db = new DbService();
        string query = "";
        if (func == "edit")
        {
            query = "UPDATE Documents SET DocumentName = '" + DocumentName + "', DocumentTypeID = '" + DocType.DocumentTypeID + "', ImgID = " + ImgID + ", HasImg = 'Y', TotalPrice = " + TotalPrice + ", Comments = '" + Comments + "', Date = '" + sqlFormattedDate + "', DriverID = " + SendBy.DriverID + ", ContainerID = '" + ContainerID + "', OrderID = " + RelatedOrder.OrderID + " WHERE DocumentID = " + DocumentID;
        }
        else if (func == "new")
        {
            query = "insert into Documents values ('" + DocumentName + "'," + DocType.DocumentTypeID + "," + TotalPrice + ",'" + Comments + "','" + sqlFormattedDate + "','Y'," + SendBy.DriverID + ", '" + ContainerID + "'," + RelatedOrder.OrderID + ","+ImgID+",'Y')";
        }
        db.ExecuteQuery(query);

        if (DocType.DocumentTypeID == 1)
        {
            DbService db1 = new DbService();
            string query1 = "UPDATE Orders SET OrderStatusID = 6 where OrderID = " + RelatedOrder.OrderID;
            db1.ExecuteQuery(query1);
        }
    }


    public void deactivateDocument(string active)
    {
        DbService db = new DbService();
        db.ExecuteQuery("UPDATE Documents SET Active='" + active + "' WHERE DocumentID=" + DocumentID);
    }

    public List<Documents> getDriverDocumentsList(int driverID, int selectedDocumentTypeID, DateTime startDate, DateTime endDate)
    {

        #region DB functions
        string sqlStartDate = startDate.ToString("yyyy-MM-dd");
        string sqlEndDate = endDate.ToString("yyyy-MM-dd");

        string query = "select d.DocumentID, d.DocumentName, dt.DocumentTypeID, dt.DocumentType, d.Url, d.TotalPrice, d.Comments, d.Date, d.ContainerID, o.OrderID, o.OrderName, c.CustomerID, c.CustomerName"
            + " from Documents d"
            + " inner join DocumentTypes dt on d.DocumentTypeID = dt.DocumentTypeID"
            + " inner join Orders o on o.OrderID = d.OrderID"
            + " inner join Customers c on o.CustomerID = c.CustomerID"
            + " where d.Active = 'Y' and d.DriverID =" + driverID ;
       

        if (selectedDocumentTypeID != -1)
        {
            query += " and dt.DocumentTypeID =" + selectedDocumentTypeID;
        }
        if (startDate.Year != 1)
        {
            query += " and d.Date >= '" + sqlStartDate + "'";
            if (endDate.Year != 1)
            {
                query += " and d.Date <= '" + sqlEndDate + "'";
            }
        }

        query += " order by d.Date desc";

        List<Documents> list = new List<Documents>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Documents tmp = new Documents();
            tmp.DocumentID = (int)dr["DocumentID"];
            tmp.DocumentName = dr["DocumentName"].ToString();
            //tmp.Url = dr["Url"].ToString();
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.Comments = dr["Comments"].ToString();
            tmp.ContainerID = dr["ContainerID"].ToString();
            tmp.Date = (DateTime)dr["Date"];
            tmp.DocType = new DocumentTypes((int)dr["DocumentTypeID"], dr["DocumentType"].ToString());

            Orders o = new Orders();
            o.OrderID = (int)dr["OrderID"];
            o.OrderName = dr["OrderName"].ToString();
            o.Customer = new Customers();
            o.Customer.CustomerID = (int)dr["CustomerID"];
            o.Customer.CustomerName = dr["CustomerName"].ToString();
            tmp.RelatedOrder = o;

            list.Add(tmp);
        }
        #endregion

        return list;

    }

    public List<Documents> getDriverDocumentsPeriodList(int driverID, int func)
    {

        #region DB functions

        string query = "select d.DocumentID, d.DocumentName, dt.DocumentTypeID, dt.DocumentType, d.ImgID,d.HasImg, d.TotalPrice, d.Comments, d.Date, d.ContainerID, o.OrderID, o.OrderName, c.CustomerID, c.CustomerName"
            + " from Documents d"
            + " inner join DocumentTypes dt on d.DocumentTypeID = dt.DocumentTypeID"
            + " inner join Orders o on o.OrderID = d.OrderID"
            + " inner join Customers c on o.CustomerID = c.CustomerID"
            + " where d.Active = 'Y' and d.DriverID =" + driverID;

        #region selecting Period

        if (func == 1)
        {
            query += " and d.Date = convert(date, GETDATE())";
        }

        if (func == 2)
        {
            query += " and d.Date >= convert(date,DATEADD(day, -7, GETDATE()))";
        }

        if (func == 3)
        {
            query += " and d.Date >= convert(date,DATEADD(day, -30, GETDATE())) ";
        }

        #endregion

        query += " order by d.Date desc";

        List<Documents> list = new List<Documents>();
        DbService db = new DbService();
        DataSet ds = db.GetDataSetByQuery(query);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {

            Documents tmp = new Documents();
            tmp.DocumentID = (int)dr["DocumentID"];
            tmp.DocumentName = dr["DocumentName"].ToString();
            if (dr["HasImg"].ToString() == "Y")
            {
                tmp.ImgID = (int)dr["ImgID"];
            }
            tmp.TotalPrice = Convert.ToSingle(dr["TotalPrice"]);
            tmp.Comments = dr["Comments"].ToString();
            tmp.ContainerID = dr["ContainerID"].ToString();
            tmp.Date = (DateTime)dr["Date"];
            tmp.DocType = new DocumentTypes((int)dr["DocumentTypeID"], dr["DocumentType"].ToString());

            Orders o = new Orders();
            o.OrderID = (int)dr["OrderID"];
            o.OrderName = dr["OrderName"].ToString();
            o.Customer = new Customers();
            o.Customer.CustomerID = (int)dr["CustomerID"];
            o.Customer.CustomerName = dr["CustomerName"].ToString();
            tmp.RelatedOrder = o;

            list.Add(tmp);
        }
        #endregion

        return list;

    }
}
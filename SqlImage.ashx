<%@ WebHandler Language="C#" Class="SqlImage" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;

public class SqlImage : IHttpHandler {

    public void ProcessRequest (HttpContext context) {


        //get image width to be resized from querystring
        string imgwidth = context.Request.QueryString["width"];
        //get image height to be resized from querystring
        string imgHeight = context.Request.QueryString["height"];
        //id for the image
        string imgId = context.Request.QueryString["id"];
        //id for the image
        string imgFilter = context.Request.QueryString["filter"];
        byte[] b = null;

        DbService db = new DbService();

        b= (byte[])db.GetObjectScalarByQuery(@"select ImageUrl from Images where ImageID="+imgId);
       
        context.Response.ContentType = "image/jpg";
        //send contents of byte array as response to client (browser)
        context.Response.BinaryWrite(ResizeImage(int.Parse(imgwidth), int.Parse(imgHeight), b));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }



    private byte[] ResizeImage(int newWidth, int newHeight, byte[] myBytes)
    {
        System.IO.MemoryStream myMemStream = new System.IO.MemoryStream(myBytes);
        System.Drawing.Image fullsizeImage = System.Drawing.Image.FromStream(myMemStream);
        System.Drawing.Image newImage = SetQuality(fullsizeImage, newWidth, newHeight);
        System.IO.MemoryStream myResult = new System.IO.MemoryStream();
        newImage.Save(myResult, System.Drawing.Imaging.ImageFormat.Jpeg);  //Or whatever format you want.
        return myResult.ToArray();  //Returns a new byte array.
    }

    private Image SetQuality(Image img, int maxWidth, int maxHeight)
    {
        //if (img.Height < maxHeight && img.Width < maxWidth) return img;
        using (img)
        {
            Double xRatio = (double)img.Width / maxWidth;
            Double yRatio = (double)img.Height / maxHeight;
            Double ratio = Math.Max(xRatio, yRatio);
            int nnx = (int)Math.Floor(img.Width / ratio);
            int nny = (int)Math.Floor(img.Height / ratio);
            Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(cpy))
            {
                gr.Clear(Color.Transparent);

                // This is said to give best quality when resizing images
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                gr.DrawImage(img,
                    new Rectangle(0, 0, nnx, nny),
                    new Rectangle(0, 0, img.Width, img.Height),
                    GraphicsUnit.Pixel);
            }
            return cpy;
        }

    }



}
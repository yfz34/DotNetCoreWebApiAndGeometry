using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCore3.Models
{
    public class MapJson
    {
        public class DataResponse
        {
            public int ResultCode { get; set; }         
            public string Message { get; set; }         
            public List<WKTData> Data { get; set; }         
            public int InsertID { get; set; }                 
        }
        public class WKTData
        {
            public string wkt { get; set; }              
            public string title { get; set; }
            public string body { get; set; }
        }

        public class DataRequest
        {
            public string cmd { get; set; }
            public string wkt { get; set; }              
            public string title { get; set; }        
            public string body { get; set; }    
            public int id { get; set; }              
        }
    }
}
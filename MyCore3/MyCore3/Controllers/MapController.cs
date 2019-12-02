using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCore3.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using MyCore3.Handle;

namespace MyCore3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapController : ControllerBase
    {
        private readonly IConfiguration _config;
        public MapController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public MapJson.DataResponse Get()
        {
             MapJson.DataResponse res = new MapJson.DataResponse
            {
                ResultCode = 50,
            };
            Debug.WriteLine("撈取空間資料");
            var constr = _config["Databases:Local"];
            SqlConnection con = new SqlConnection(constr);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select Map.GeomCol1.STAsText() as data, Map.Title, Map.Body from Map", con);

                cmd.Parameters.Clear();

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                cmd.ExecuteNonQuery();
                con.Close();
                Debug.WriteLine("SQL連線關閉");

                List<MapJson.WKTData> wktData = new List<MapJson.WKTData>();

                //輪詢
                foreach (DataRow row in dt.Rows)
                {
                    wktData.Add(new MapJson.WKTData
                    {
                        wkt = dt.Rows[dt.Rows.IndexOf(row)]["data"].ToString(),
                        title = dt.Rows[dt.Rows.IndexOf(row)]["Title"].ToString(),
                        body = dt.Rows[dt.Rows.IndexOf(row)]["Body"].ToString(),
                    });
                }
                res.ResultCode = 10;
                res.Data = wktData;
                return res;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return res;
        }

        [HttpPost]
        public MapJson.DataResponse Post([FromBody]MapJson.DataRequest req)
        {
            MapJson.DataResponse res = new MapJson.DataResponse
            {
                ResultCode = 50,
            };

            string constr = _config["Databases:Local"];
            switch (req.cmd)
            {
               case "INSERT":
                    res = MapMethod.InsertRows(req, res, constr);
                    break;
                case "UPDATE":
                    res = MapMethod.UpdateRows(req, res, constr);
                    break;
                case "DELETE":
                    res = MapMethod.DeleteRows(req.id, res, constr);
                    break;
                default:
                    Debug.WriteLine("無此指令");
                    res.ResultCode = 55;
                    res.Message = "無此指令";
                    break;
            }
            return res;
        }
    }
}

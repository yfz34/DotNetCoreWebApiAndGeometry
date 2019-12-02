using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCore3.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MyCore3.Handle
{
    public class MapMethod
    {
        public static MapJson.DataResponse InsertRows(MapJson.DataRequest req, MapJson.DataResponse res, string constr)
        {
            Console.WriteLine("新增資料");
            string[] sqlparams = new string[] { "GeomCol1", "Title", "Body" };
            String setParams = String.Empty;
            String valParams = String.Empty;
            if (!string.IsNullOrEmpty(req.wkt)) { setParams += sqlparams[0] + ","; valParams += "@" + sqlparams[0] + ","; }
            if (!string.IsNullOrEmpty(req.title)) { setParams += sqlparams[1] + ","; valParams += "@" + sqlparams[1] + ","; }
            if (!string.IsNullOrEmpty(req.body)) { setParams += sqlparams[2] + ","; valParams += "@" + sqlparams[2] + ","; }

            setParams = setParams.Remove((setParams.Length - 1), 1); //去除最後一個逗號
            valParams = valParams.Remove((valParams.Length - 1), 1); //去除最後一個逗號
            Console.WriteLine(setParams);
            Console.WriteLine(valParams);

            // string constr = _config["Databases:Local"];
            SqlConnection con = new SqlConnection(constr);
            try
            {
                con.Open();
                string query = string.Intern("INSERT INTO Map (") + setParams + string.Intern(") output INSERTED.ID VALUES (") + valParams + string.Intern(")");
                Console.WriteLine(query);

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Clear();

                if (!string.IsNullOrEmpty(req.wkt)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[0], req.wkt)); }
                if (!string.IsNullOrEmpty(req.title)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[1], req.title)); }
                if (!string.IsNullOrEmpty(req.body)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[2], req.body)); }

                int modified = (int)cmd.ExecuteScalar();
                con.Close();
                Console.WriteLine("SQL連線關閉");

                if (modified >= 0)
                {
                    res.ResultCode = 10;
                    res.InsertID = modified;
                }
                else
                    res.ResultCode = 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                res.Message = ex.ToString();
            }
            return res;
        }

        public static MapJson.DataResponse UpdateRows(MapJson.DataRequest req, MapJson.DataResponse res, string constr)
        {
            Console.WriteLine("更改資料");
            string[] sqlparams = new string[] { "GeomCol1", "Title", "Body" };
            String setParams = String.Empty; 
            String valParams = String.Empty;
            if (!string.IsNullOrEmpty(req.wkt)) { setParams += sqlparams[0]+"=@"+ sqlparams[0] + ",";  }
            if (!string.IsNullOrEmpty(req.title)) { setParams += sqlparams[1] + "=@" + sqlparams[1] + ","; }
            if (!string.IsNullOrEmpty(req.body)) { setParams += sqlparams[2] + "=@" + sqlparams[2] + ","; }
            setParams = setParams.Remove((setParams.Length - 1), 1); //去除最後一個逗號
            Console.WriteLine(setParams);

            SqlConnection con = new SqlConnection(constr);
            try
            {
                con.Open();
                string query = string.Intern("UPDATE Map SET ") + setParams + string.Intern(" WHERE id=@id;");
                //string query = string.Intern("UPDATE Members (") + setParams + string.Intern(") VALUES (") + valParams + string.Intern(")");
                Console.WriteLine(query);

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Clear();
                if (req.id!=0) { cmd.Parameters.Add(new SqlParameter("@id", req.id)); }
                if (!string.IsNullOrEmpty(req.wkt)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[0], req.wkt)); }
                if (!string.IsNullOrEmpty(req.title)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[1], req.title)); }
                if (!string.IsNullOrEmpty(req.body)) { cmd.Parameters.Add(new SqlParameter("@" + sqlparams[2], req.body)); }

                int i = cmd.ExecuteNonQuery();
                con.Close();
                Console.WriteLine("SQL連線關閉");

                if (i == 1)
                    res.ResultCode = 10;
                else
                    res.ResultCode = 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                res.Message = ex.ToString();
            }
            return res;
        }

        public static MapJson.DataResponse DeleteRows(int id, MapJson.DataResponse res, string constr)
        {
            Debug.WriteLine("刪除資料ID:", id);

            // string constr = _config["Databases:Local"];
            SqlConnection con = new SqlConnection(constr);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Map WHERE id = @id", con);
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@id", id));

                int i = cmd.ExecuteNonQuery();
                con.Close();
                Debug.WriteLine("SQL連線關閉");

                if (i == 1)
                    res.ResultCode = 10;
                else
                    res.ResultCode = 20;
            }
            catch (Exception ex)
            {
                res.ResultCode = 40;
                res.Message = ex.ToString();
            }
            return res;
        }
    }
}
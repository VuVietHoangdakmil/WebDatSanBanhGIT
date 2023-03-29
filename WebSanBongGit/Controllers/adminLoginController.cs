using WebSanBongGit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DO_AN_CHUYEN_NGANH.Controllers
{
    public class adminLoginController : Controller
    {
        DATA_SAN_BONGDataContext data = new DATA_SAN_BONGDataContext();
        // GET: adminLogin
        public static string GetMD5(string str)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }

            return sbHash.ToString();

        }
        public ActionResult formlogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKyAdmin(string TEN_DN, string MK_DN)
        {
            try
            {
                Boolean IsSuccess;
                var TK = new ADMIN();
                var TK_ADM = data.ADMINs.FirstOrDefault(n => n.TEN_DANG_NHAP_ADM == TEN_DN);
                if (TK_ADM == null)
                {
                   
                    IsSuccess = true;
                    TK.TEN_DANG_NHAP_ADM = TEN_DN;
                    TK.MAT_KHAU_ADM = GetMD5(MK_DN);
                    TK.MA_CHUC_VU = 1;
                    data.ADMINs.InsertOnSubmit(TK);
                    data.SubmitChanges();
                }
                else
                {
                    IsSuccess = false;
                }

                return Json(new { code = 200, IsSuccess = IsSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult DangNhapAdmin(string TEN_DN, string MK_DN)
        {
            try
            {
                Boolean IsSuccess;
                //int machucVU = 1;
                //ADMIN TkAdm = data.ADMINs.Single(n => n.TEN_DANG_NHAP_ADM == TEN_DN && n.MAT_KHAU_ADM == GetMD5(MK_DN));
                //int machucVU = TkAdm.MA_CHUC_VU;
                var TkAdm = (from ds in data.ADMINs
                             .Where(n=>n.TEN_DANG_NHAP_ADM == TEN_DN && n.MAT_KHAU_ADM == GetMD5(MK_DN))
                             select new
                             {
                                 ID_TK_ADMIN = ds.STT,
                                 MA_CHUC_VU = ds.MA_CHUC_VU,
                                 TEN_MK_DN = ds.TEN_DANG_NHAP_ADM,
                             }).FirstOrDefault();
                              
                if (TkAdm != null)
                {
                    
                    IsSuccess = true;
                    
                }
                else
                {
                    IsSuccess = false;
                }

                return Json(new { code = 200, IsSuccess = IsSuccess, TkAdm  = TkAdm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
    }
}
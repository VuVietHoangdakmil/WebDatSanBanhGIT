using WebSanBongGit.Models;
//using mvcDangNhap.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DO_AN_CHUYEN_NGANH.Controllers
{
    public class UserController : Controller
    {

        // GET: User
        DATA_SAN_BONGDataContext data = new DATA_SAN_BONGDataContext();
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
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACH_HANG kh)
        {

            var SDT = collection["SDT"];
            var Name = collection["Name"];
            var email = collection["EMAIL"];
            var password = collection["password"];
            var again_PASSWORD = collection["again-PASSWORD"];
            KHACH_HANG kh1 = data.KHACH_HANGs.FirstOrDefault(n => n.SDT == SDT);
            if (kh1 == null)
            {
                kh.SDT = SDT;
                kh.TEN_KH = Name;
                kh.EMAIL = email;
                kh.MAT_KHAU = GetMD5(password);
                kh.TEN_DN = SDT;
                kh.TINH_TRANG_XOA = false;
                data.KHACH_HANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                Session["TaiKhoan"] = kh;
                KHACH_HANG khSession = (KHACH_HANG)Session["Taikhoan"];
                if (Session["TaiKhoan"] != null)
                {
                    Session["ten-khach-hang"] = khSession.TEN_KH;
                    Session["MaKH"] = khSession.MA_KH;
                    Session["AvatarKH"] = khSession.AVATAR;
                    //Session["AvatarKH"] khSession = kh1.HINH_ANH;

                }
            }
            else
            {
                Session["isKT"] = true;

            }
            return RedirectToAction("ListSan", "WebSanBong");
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["SDT-login"];
            var mk = collection["PASSWORD"];
            KHACH_HANG kh = data.KHACH_HANGs.SingleOrDefault(n => n.TEN_DN == tendn && n.MAT_KHAU == GetMD5(mk) && n.TINH_TRANG_XOA == false);
            if (kh != null)
            {
                Session["TaiKhoan"] = kh;
                KHACH_HANG khSession = (KHACH_HANG)Session["Taikhoan"];
                if (Session["TaiKhoan"] != null)
                {
                    Session["ten-khach-hang"] = khSession.TEN_KH;
                    Session["MaKH"] = khSession.MA_KH;
                    Session["AvatarKH"] = khSession.AVATAR;
                    //Session["AvatarKH"] khSession = kh1.HINH_ANH;
                }
            }
            else
            {
                Session["isKT2"] = true;
            }
            return RedirectToAction("ListSan", "WebSanBong");
        }


        public ActionResult hidemesage()
        {
            Session["isKT"] = null;
            Session["isKT2"] = null;
            Session["isKT3"] = null;
            return RedirectToAction("ListSan", "WebSanBong");
        }
        public ActionResult DangXuat()
        {

            Session["TaiKhoan"] = null;
            Session["ten-khach-hang"] = null;
            Session["AvatarKH"] = null;
            Session["MaKH"] = null;
            return RedirectToAction("ListSan", "WebSanBong");
        }
        public ActionResult quaylai()
        {
            return RedirectToAction("ListSan", "WebSanBong");
        }
        [HttpGet]
        public ActionResult TTKH(int id)
        {
            if (Session["MaKH"] == null)
            {
                return RedirectToAction("ListSan", "WebSanBong");
            }
            else
            {
                var TTKH = from kh in data.KHACH_HANGs where kh.MA_KH == id select kh;
                return View(TTKH.SingleOrDefault());
            }

        }
        [HttpPost]
        public ActionResult TTKH(int id, FormCollection f, HttpPostedFileBase FILEIMG_iNf)
        {


            KHACH_HANG sp = data.KHACH_HANGs.SingleOrDefault(n => n.MA_KH == id);


            ////Kiem tra duong dan file


            if (ModelState.IsValid)
            {

                if (FILEIMG_iNf != null)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(FILEIMG_iNf.FileName);

                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("/img"), fileName);

                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        FILEIMG_iNf.SaveAs(path);

                    }
                    sp.AVATAR = fileName;
                }
                sp.TEN_KH = f["NAME_iNf"];
                sp.EMAIL = f["EMAIL_iNf"];
                sp.SDT = f["SDT_iNf"];

                //Luu vao CSDL
                UpdateModel(sp);
                data.SubmitChanges();
                Session["AvatarKH"] = sp.AVATAR;
                Session["ten-khach-hang"] = sp.TEN_KH;
            }
            return RedirectToAction("ListSan", "WebSanBong");
        }

        [HttpPost]
        public JsonResult capnhatMKQUEN(string SDT, string EMAIL, string mkrandom)
        {
            try
            {
                Boolean isTK;
                var ListSan = data.KHACH_HANGs.First(n => n.SDT == SDT && n.EMAIL == EMAIL);
                if (ListSan != null)
                {
                    isTK = true;
                    ListSan.MAT_KHAU = GetMD5(mkrandom);
                    UpdateModel(ListSan);
                    data.SubmitChanges();

                    // gui mail
                    string content = System.IO.File.ReadAllText(Server.MapPath("~/content/temlate/mailquenmk.html"));
                    content = content.Replace("{{SDT}}", SDT);
                    content = content.Replace("{{MK_NEW}}", mkrandom);
                    //new MailHelper().SendMail(EMAIL, "Thay đổi mật khẩu thành công từ Sân Bóng AE", content);

                }
                else
                {
                    isTK = false;
                }
                return Json(new { code = 200, isTK = isTK }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
    }
}
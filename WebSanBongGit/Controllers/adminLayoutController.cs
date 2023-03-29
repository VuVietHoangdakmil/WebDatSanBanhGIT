using WebSanBongGit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace WebSanBongGit.Controllers
{
    public class adminLayoutController : Controller
    {
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
        DATA_SAN_BONGDataContext data = new DATA_SAN_BONGDataContext();
        // GET: adminLayout
        // ham tinh so trang
        public static int SoTrang(int countData, int PageSize)
        {
            int sotrang = countData % PageSize == 0 ? countData / PageSize : countData / PageSize + 1;
            return sotrang;
        }

        private List<DON_DAT_SAN> laydonmoi(int count)
        {
            return data.DON_DAT_SANs.OrderByDescending(n => n.MA_DS).Take(count).ToList();
        }
        public ActionResult Dashboard()
        {
            var sumtien = from sumT in data.CT_DAT_SANs where sumT.TINH_TRANG_THANH_TOAN == true select sumT;
            var user = from users in data.KHACH_HANGs select users;
            var slUser = user.Count();
            var s = sumtien.Sum(n => n.TONG_TIEN);
            if(s!=null)
            {
                ViewBag.TONGTIENS = s;
            }
            else
            {
                ViewBag.TONGTIENS = 0;
            }
           
            ViewBag.SLuser = slUser;
            var listDDS = laydonmoi(10);
            ViewBag.listDDS = listDDS.Count();
            return View(listDDS);
        }
        
        
       // TABLE TÀI KHOẢN....................................
        public ActionResult THONTINUSER()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DsUser(int Trang,string SearchSDT, string SearchName)
        {
            try
            {
                var dsuser = (from ds in data.KHACH_HANGs
                              where ds.TEN_KH.ToUpper().Contains(SearchName) && ds.SDT.Contains(SearchSDT) && ds.TINH_TRANG_XOA != true
                              select new
                              {
                                  MA_KH = ds.MA_KH,
                                  TEN_KH = ds.TEN_KH,
                                  SDT = ds.SDT,
                                  TEN_DN= ds.TEN_DN,
                                  MAT_KHAU = ds.MAT_KHAU,
                                  EMAIL = ds.EMAIL,
                                  AVATAR = ds.AVATAR
                              }).OrderByDescending(n=> n.MA_KH).ToList();

                var PageZise = 5;
                var soTrang = SoTrang(dsuser.Count(), PageZise);
                var dsUserPT = dsuser.Skip((Trang - 1) * PageZise).Take(PageZise).ToList();
                return Json(new { code = 200, dsuser = dsUserPT , soTrang  = soTrang}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpGet]
        public JsonResult DetailUser(int id)
        {
            try
            {
                var dsuser = (from ds in data.KHACH_HANGs
                              where ds.MA_KH == id
                              select new
                              {
                                  MA_KH = ds.MA_KH,
                                  TEN_KH = ds.TEN_KH,
                                  SDT = ds.SDT,
                                  TEN_DN = ds.TEN_DN,
                                  MAT_KHAU = ds.MAT_KHAU,
                                  EMAIL = ds.EMAIL,
                                  AVATAR = ds.AVATAR
                              }).Single();


                return Json(new { code = 200, dsuser = dsuser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var dsuser = data.KHACH_HANGs.Where(n => n.MA_KH == id).First();
                dsuser.TINH_TRANG_XOA = true;
                UpdateModel(dsuser);
                data.SubmitChanges();

                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        // TABLE ĐƠN HÀNG.....................................
        public ActionResult ThongTinDH()
        {
            ViewBag.hourtStart_DH = new SelectList(data.SO_GIOs.ToList().OrderBy(n => n.SO_GIO1), "SO_GIO1", "SO_GIO1");
            ViewBag.minuteEnd_DH = new SelectList(data.SO_PHUTs.ToList().OrderBy(n => n.SO_PHUT1), "SO_PHUT1", "SO_PHUT1");
            return View();
        }

        [HttpGet]
        public JsonResult DsDonDatSan(string SDT, string TENSAN, string NGAYDA, string TimeEnd,string trangThai,int Trang, string IsThanhToan)
        {
            try
            {
                var dsDon1 = data.DON_DAT_SANs
                    
                    .Join(data.CT_DAT_SANs
                    , ds => ds.MA_DS,
                    ctds => ctds.MA_DS,
                    (ds, ctds) => new
                    {
                        MA_DS = ds.MA_DS,
                        SDT_KH = ds.KHACH_HANG.SDT,
                        NGAY_DA = ds.NGAY_DAT,
                        GIO_BD = ds.GIO_BAT_DAU,
                        GIO_KT = ds.GIO_KET_THUC,
                        TEN_SAN = ds.SAN.TEN_SAN,
                        TONG_TIEN = ds.TONG_TIEN,
                        TONG_GIO_THUE = ds.TONG_GIO_THUE,
                        GIA_SAN = ds.GIA_SAN,
                        NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                        TINH_TRANG_DON = ds.TINH_TRANG_DON,
                        TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                        TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                    }).OrderByDescending(n => n.MA_DS).ToList();


                //var dsDon = (from ds in data.DON_DAT_SANs

                //             select new
                //             {
                //                 MA_DS = ds.MA_DS,
                //                 SDT_KH = ds.KHACH_HANG.SDT,
                //                 NGAY_DA = ds.NGAY_DAT,
                //                 GIO_BD = ds.GIO_BAT_DAU,
                //                 GIO_KT = ds.GIO_KET_THUC,
                //                 TEN_SAN = ds.SAN.TEN_SAN,
                //                 TONG_TIEN = ds.TONG_TIEN,
                //                 TONG_GIO_THUE = ds.TONG_GIO_THUE,
                //                 GIA_SAN = ds.GIA_SAN,
                //                 NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                //                 TINH_TRANG_DON = ds.TINH_TRANG_DON,
                //                 TRANG_THAI_XOA = ds.TRANG_THAI_XOA,

                //             }).OrderByDescending(n => n.MA_DS).ToList();
                if (IsThanhToan != "" && NGAYDA != "")
                {
                    dsDon1 = data.DON_DAT_SANs
                  .Join(data.CT_DAT_SANs
                  , ds => ds.MA_DS,
                  ctds => ctds.MA_DS,
                  (ds, ctds) => new
                  {
                      MA_DS = ds.MA_DS,
                      SDT_KH = ds.KHACH_HANG.SDT,
                      NGAY_DA = ds.NGAY_DAT,
                      GIO_BD = ds.GIO_BAT_DAU,
                      GIO_KT = ds.GIO_KET_THUC,
                      TEN_SAN = ds.SAN.TEN_SAN,
                      TONG_TIEN = ds.TONG_TIEN,
                      TONG_GIO_THUE = ds.TONG_GIO_THUE,
                      GIA_SAN = ds.GIA_SAN,
                      NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                      TINH_TRANG_DON = ds.TINH_TRANG_DON,
                      TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                      TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                  }).Where(n => n.NGAY_DA == DateTime.Parse(NGAYDA) && n.TINH_TRANG_THANH_TOAN == Boolean.Parse(IsThanhToan) && n.TRANG_THAI_XOA == false)
                  .OrderByDescending(n => n.MA_DS)
                  .ToList();
                }
                else if (IsThanhToan != "")
                {
                    dsDon1 = data.DON_DAT_SANs
                 .Join(data.CT_DAT_SANs
                 , ds => ds.MA_DS,
                 ctds => ctds.MA_DS,
                 (ds, ctds) => new
                 {
                     MA_DS = ds.MA_DS,
                     SDT_KH = ds.KHACH_HANG.SDT,
                     NGAY_DA = ds.NGAY_DAT,
                     GIO_BD = ds.GIO_BAT_DAU,
                     GIO_KT = ds.GIO_KET_THUC,
                     TEN_SAN = ds.SAN.TEN_SAN,
                     TONG_TIEN = ds.TONG_TIEN,
                     TONG_GIO_THUE = ds.TONG_GIO_THUE,
                     GIA_SAN = ds.GIA_SAN,
                     NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                     TINH_TRANG_DON = ds.TINH_TRANG_DON,
                     TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                     TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                 }).Where(n => n.TINH_TRANG_THANH_TOAN == Boolean.Parse(IsThanhToan) && n.TRANG_THAI_XOA == false)
                 .OrderByDescending(n => n.MA_DS)
                 .ToList();
                }
                else if (NGAYDA == "" && trangThai.Trim()=="")
                {
                    
                    //dsDon = (from ds in data.DON_DAT_SANs
                    //        .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,

                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                    dsDon1 = data.DON_DAT_SANs
                   .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TRANG_THAI_XOA == false)
                    .Join(data.CT_DAT_SANs
                    , ds => ds.MA_DS,
                    ctds => ctds.MA_DS,
                    (ds, ctds) => new
                    {
                        MA_DS = ds.MA_DS,
                        SDT_KH = ds.KHACH_HANG.SDT,
                        NGAY_DA = ds.NGAY_DAT,
                        GIO_BD = ds.GIO_BAT_DAU,
                        GIO_KT = ds.GIO_KET_THUC,
                        TEN_SAN = ds.SAN.TEN_SAN,
                        TONG_TIEN = ds.TONG_TIEN,
                        TONG_GIO_THUE = ds.TONG_GIO_THUE,
                        GIA_SAN = ds.GIA_SAN,
                        NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                        TINH_TRANG_DON = ds.TINH_TRANG_DON,
                        TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                        TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                    }).OrderByDescending(n => n.MA_DS).ToList();
                }
                else if (trangThai.Trim() != "" && trangThai.Trim() != "null" && NGAYDA != "")
                {
                    //dsDon = (from ds in data.DON_DAT_SANs
                    //        .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == Boolean.Parse(trangThai) && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                   dsDon1 = data.DON_DAT_SANs
                  .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == Boolean.Parse(trangThai) && n.TRANG_THAI_XOA == false)
                   .Join(data.CT_DAT_SANs
                   , ds => ds.MA_DS,
                   ctds => ctds.MA_DS,
                   (ds, ctds) => new
                   {
                       MA_DS = ds.MA_DS,
                       SDT_KH = ds.KHACH_HANG.SDT,
                       NGAY_DA = ds.NGAY_DAT,
                       GIO_BD = ds.GIO_BAT_DAU,
                       GIO_KT = ds.GIO_KET_THUC,
                       TEN_SAN = ds.SAN.TEN_SAN,
                       TONG_TIEN = ds.TONG_TIEN,
                       TONG_GIO_THUE = ds.TONG_GIO_THUE,
                       GIA_SAN = ds.GIA_SAN,
                       NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                       TINH_TRANG_DON = ds.TINH_TRANG_DON,
                       TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                       TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                   }).OrderByDescending(n => n.MA_DS).ToList();
                }
                else if (trangThai.Trim() == "null" && NGAYDA != "")
                {
                    //dsDon = (from ds in data.DON_DAT_SANs
                    //        .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == null && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                   dsDon1 = data.DON_DAT_SANs
                    .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == null && n.TRANG_THAI_XOA == false)
                   .Join(data.CT_DAT_SANs
                   , ds => ds.MA_DS,
                   ctds => ctds.MA_DS,
                   (ds, ctds) => new
                   {
                       MA_DS = ds.MA_DS,
                       SDT_KH = ds.KHACH_HANG.SDT,
                       NGAY_DA = ds.NGAY_DAT,
                       GIO_BD = ds.GIO_BAT_DAU,
                       GIO_KT = ds.GIO_KET_THUC,
                       TEN_SAN = ds.SAN.TEN_SAN,
                       TONG_TIEN = ds.TONG_TIEN,
                       TONG_GIO_THUE = ds.TONG_GIO_THUE,
                       GIA_SAN = ds.GIA_SAN,
                       NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                       TINH_TRANG_DON = ds.TINH_TRANG_DON,
                       TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                       TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                   }).OrderByDescending(n => n.MA_DS).ToList();
                }
                else if (trangThai.Trim() == "null")
                {
                    //dsDon = (from ds in data.DON_DAT_SANs
                    //        .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == null && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                    dsDon1 = data.DON_DAT_SANs
                    .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == null && n.TRANG_THAI_XOA == false)
                   .Join(data.CT_DAT_SANs
                   , ds => ds.MA_DS,
                   ctds => ctds.MA_DS,
                   (ds, ctds) => new
                   {
                       MA_DS = ds.MA_DS,
                       SDT_KH = ds.KHACH_HANG.SDT,
                       NGAY_DA = ds.NGAY_DAT,
                       GIO_BD = ds.GIO_BAT_DAU,
                       GIO_KT = ds.GIO_KET_THUC,
                       TEN_SAN = ds.SAN.TEN_SAN,
                       TONG_TIEN = ds.TONG_TIEN,
                       TONG_GIO_THUE = ds.TONG_GIO_THUE,
                       GIA_SAN = ds.GIA_SAN,
                       NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                       TINH_TRANG_DON = ds.TINH_TRANG_DON,
                       TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                       TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                   }).OrderByDescending(n => n.MA_DS).ToList();
                }
                else if (trangThai.Trim() != "")
                {
                    //dsDon = (from ds in data.DON_DAT_SANs
                    //        .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == Boolean.Parse(trangThai) && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                    dsDon1 = data.DON_DAT_SANs
                     .Where(n => n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TINH_TRANG_DON == Boolean.Parse(trangThai) && n.TRANG_THAI_XOA == false)
                  .Join(data.CT_DAT_SANs
                  , ds => ds.MA_DS,
                  ctds => ctds.MA_DS,
                  (ds, ctds) => new
                  {
                      MA_DS = ds.MA_DS,
                      SDT_KH = ds.KHACH_HANG.SDT,
                      NGAY_DA = ds.NGAY_DAT,
                      GIO_BD = ds.GIO_BAT_DAU,
                      GIO_KT = ds.GIO_KET_THUC,
                      TEN_SAN = ds.SAN.TEN_SAN,
                      TONG_TIEN = ds.TONG_TIEN,
                      TONG_GIO_THUE = ds.TONG_GIO_THUE,
                      GIA_SAN = ds.GIA_SAN,
                      NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                      TINH_TRANG_DON = ds.TINH_TRANG_DON,
                      TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                      TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                  }).OrderByDescending(n => n.MA_DS).ToList();
                }
                else 
                {

                    //dsDon = (from ds in data.DON_DAT_SANs
                    //         .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TRANG_THAI_XOA == false)
                    //         select new
                    //         {
                    //             MA_DS = ds.MA_DS,
                    //             SDT_KH = ds.KHACH_HANG.SDT,
                    //             NGAY_DA = ds.NGAY_DAT,
                    //             GIO_BD = ds.GIO_BAT_DAU,
                    //             GIO_KT = ds.GIO_KET_THUC,
                    //             TEN_SAN = ds.SAN.TEN_SAN,
                    //             TONG_TIEN = ds.TONG_TIEN,
                    //             TONG_GIO_THUE = ds.TONG_GIO_THUE,
                    //             GIA_SAN = ds.GIA_SAN,
                    //             NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                    //             TINH_TRANG_DON = ds.TINH_TRANG_DON,
                    //             TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                                 
                    //         }).OrderByDescending(n => n.MA_DS).ToList();

                    dsDon1 = data.DON_DAT_SANs
                      .Where(n => n.NGAY_DAT == DateTime.Parse(NGAYDA) && n.KHACH_HANG.SDT.Contains(SDT) && n.SAN.TEN_SAN.ToUpper().Contains(TENSAN.ToUpper()) && n.GIO_KET_THUC_TEXT.Contains(TimeEnd) && n.TRANG_THAI_XOA == false)
                  .Join(data.CT_DAT_SANs
                  , ds => ds.MA_DS,
                  ctds => ctds.MA_DS,
                  (ds, ctds) => new
                  {
                      MA_DS = ds.MA_DS,
                      SDT_KH = ds.KHACH_HANG.SDT,
                      NGAY_DA = ds.NGAY_DAT,
                      GIO_BD = ds.GIO_BAT_DAU,
                      GIO_KT = ds.GIO_KET_THUC,
                      TEN_SAN = ds.SAN.TEN_SAN,
                      TONG_TIEN = ds.TONG_TIEN,
                      TONG_GIO_THUE = ds.TONG_GIO_THUE,
                      GIA_SAN = ds.GIA_SAN,
                      NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                      TINH_TRANG_DON = ds.TINH_TRANG_DON,
                      TRANG_THAI_XOA = ds.TRANG_THAI_XOA,
                      TINH_TRANG_THANH_TOAN = ctds.TINH_TRANG_THANH_TOAN,
                  }).OrderByDescending(n => n.MA_DS).ToList();
                }
               var PageZise = 8;
               var soTrang = SoTrang(dsDon1.Count(), PageZise);
               var dsDonPT = dsDon1.Skip((Trang - 1) * PageZise).Take(PageZise).ToList(); 
                return Json(new { code = 200,SoTrang = soTrang,trangthai = trangThai, dsDon = dsDonPT, msg = "lay thanh cong" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpGet]
        public JsonResult detailDDS(int id)
        {
            
            try
            {
                var dsDondtail = (from ds in data.DON_DAT_SANs
                                  where ds.MA_DS == id
                                  select new
                                  {
                                      MA_DS = ds.MA_DS,
                                      SDT_KH = ds.KHACH_HANG.SDT,
                                      TEN_KH = ds.KHACH_HANG.TEN_KH,
                                      NGAY_DA = ds.NGAY_DAT,
                                      GIO_BD = ds.GIO_BAT_DAU,
                                      GIO_KT = ds.GIO_KET_THUC,
                                      TEN_SAN = ds.SAN.TEN_SAN,
                                      TONG_TIEN = ds.TONG_TIEN,
                                      TONG_GIO_THUE = ds.TONG_GIO_THUE,
                                      GIA_SAN = ds.GIA_SAN,
                                      NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                                      TINH_TRANG_DON = ds.TINH_TRANG_DON,
                                  }).Single();

                return Json(new { code = 200, dsDondtail = dsDondtail, msg = "Chi Tiết Đơn" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpGet]
        public JsonResult editDDS(int id)
        {

            try
            {
                var dsDondtail = (from ds in data.DON_DAT_SANs
                                  where ds.MA_DS == id
                                  select new
                                  {
                                      MA_DS = ds.MA_DS,
                                      SDT_KH = ds.KHACH_HANG.SDT,
                                      NGAY_DA = ds.NGAY_DAT,
                                      GIO_BD = ds.GIO_BAT_DAU,
                                      GIO_KT = ds.GIO_KET_THUC,
                                      TEN_SAN = ds.SAN.TEN_SAN,
                                      TONG_TIEN = ds.TONG_TIEN,
                                      TONG_GIO_THUE = ds.TONG_GIO_THUE,
                                      GIA_SAN = ds.GIA_SAN,
                                      NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                                      TINH_TRANG_DON = ds.TINH_TRANG_DON,
                                  }).Single();

                return Json(new { code = 200, dsDondtail = dsDondtail, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult editDDSchuada(int id)
        {

            try
            {
                var dsDondtail = data.DON_DAT_SANs.First(n => n.MA_DS == id);
                dsDondtail.TINH_TRANG_DON = null;
                UpdateModel(dsDondtail);
                data.SubmitChanges();
               
                return Json(new { code = 200, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult editDDSDaXong(int id)
        {

            try
            {
                var dsDondtail = data.DON_DAT_SANs.First(n => n.MA_DS == id);
                var NGAY_DA = dsDondtail.NGAY_DAT;
                var GIO_KT = dsDondtail.GIO_KET_THUC;
                dsDondtail.TINH_TRANG_DON = true;
                UpdateModel(dsDondtail);
                data.SubmitChanges();

                return Json(new { code = 200,NGAY_DA = NGAY_DA,GIO_KT=GIO_KT, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult editDDSDangDa(int id)
        {

            try
            {
                var dsDondtail = data.DON_DAT_SANs.First(n => n.MA_DS == id);
                var NGAY_DA = dsDondtail.NGAY_DAT;
                var GIO_KT = dsDondtail.GIO_KET_THUC;
                dsDondtail.TINH_TRANG_DON = false;
                UpdateModel(dsDondtail);
                data.SubmitChanges();

                return Json(new { code = 200, NGAY_DA = NGAY_DA, GIO_KT = GIO_KT, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult deleteDDSDaXong(int id)
        {

            try
            {
                var dsDondtail = data.DON_DAT_SANs.First(n => n.MA_DS == id);
               
                dsDondtail.TRANG_THAI_XOA = true;
                UpdateModel(dsDondtail);
                data.SubmitChanges();

                return Json(new { code = 200, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpGet]
        public JsonResult ShowTTtinhTien(int id)
        {

            try
            {
                var dsDondtail = data.CT_DAT_SANs.First(n => n.MA_DS == id);

                var TINH_TRANG_THANH_TOAN = dsDondtail.TINH_TRANG_THANH_TOAN;

                return Json(new { code = 200, TINH_TRANG_THANH_TOAN= TINH_TRANG_THANH_TOAN,  msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult EditDaThanhToan(int id)
        {

            try
            {
                var dsDondtail = data.CT_DAT_SANs.First(n => n.MA_DS == id);

                dsDondtail.TINH_TRANG_THANH_TOAN = true;
                UpdateModel(dsDondtail);
                data.SubmitChanges();
                return Json(new { code = 200, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult EditChuaThanhToan(int id)
        {

            try
            {
                var dsDondtail = data.CT_DAT_SANs.First(n => n.MA_DS == id);

                dsDondtail.TINH_TRANG_THANH_TOAN = false;
                UpdateModel(dsDondtail);
                data.SubmitChanges();
                return Json(new { code = 200, msg = "Cập Nhật Thành Công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


       // TABLE VIEW SÂN.......................................
        public ActionResult ViewSan()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DsTIMEDA()
        {
            try
            {
               
                var ListTime = (from ds in data.SO_GIOs
                                .Where(n=>n.MA_SO_GIO != 16)
                                select new
                                {
                                    SO_GIO = ds.SO_GIO1
                                }).ToList();

                return Json(new { code = 200, ListTime = ListTime}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult DsListSan()
        {
            try
            {

                var ListSan = (from ds in data.SANs
                                .Where(n=>n.TINH_TRANG_XOA== false)
                                select new
                                {
                                    MA_SAN = ds.MA_SAN,
                                    TEN_SAN= ds.TEN_SAN,
                                }).ToList();

                return Json(new { code = 200, ListSan = ListSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult DsDonDatSanchild(string TK_DATE, string SDT)
        {
            try
            {
               

                var dsDon = (from ds in data.DON_DAT_SANs
                              .Where(n => n.NGAY_DAT == DateTime.Parse(TK_DATE) && n.TRANG_THAI_XOA == false && n.KHACH_HANG.SDT.Contains(SDT) )
                             select new
                             {
                                 MA_DS = ds.MA_DS,
                                 MA_SAN = ds.MA_SAN,
                                 SDT_KH = ds.KHACH_HANG.SDT,
                                 NGAY_DA = ds.NGAY_DAT,
                                 GIO_BD =  ds.GIO_BAT_DAU,
                                 GIO_KT =  ds.GIO_KET_THUC,
                                 TEN_SAN = ds.SAN.TEN_SAN,
                                 TONG_TIEN = ds.TONG_TIEN,
                                 TONG_GIO_THUE = ds.TONG_GIO_THUE,
                                 GIA_SAN = ds.GIA_SAN,
                                 NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                                 TINH_TRANG_DON = ds.TINH_TRANG_DON,
                                 TRANG_THAI_XOA = ds.TRANG_THAI_XOA,

                             }).OrderBy(n => n.GIO_BD).ToList();
                if(SDT == "")
                {
                    dsDon = (from ds in data.DON_DAT_SANs
                              .Where(n => n.NGAY_DAT == DateTime.Parse(TK_DATE) && n.TRANG_THAI_XOA == false )
                             select new
                             {
                                 MA_DS = ds.MA_DS,
                                 MA_SAN = ds.MA_SAN,
                                 SDT_KH = ds.KHACH_HANG.SDT,
                                 NGAY_DA = ds.NGAY_DAT,
                                 GIO_BD =  ds.GIO_BAT_DAU,
                                 GIO_KT = ds.GIO_KET_THUC,
                                 TEN_SAN = ds.SAN.TEN_SAN,
                                 TONG_TIEN = ds.TONG_TIEN,
                                 TONG_GIO_THUE = ds.TONG_GIO_THUE,
                                 GIA_SAN = ds.GIA_SAN,
                                 NGAY_LAP_PHIEU = ds.NGAY_LAP_PHIEU,
                                 TINH_TRANG_DON = ds.TINH_TRANG_DON,
                                 TRANG_THAI_XOA = ds.TRANG_THAI_XOA,

                             }).OrderBy(n => n.GIO_BD).ToList();
                }    

                var TimeFirs = (from ds in data.SO_GIOs
                              .Where(n => n.MA_SO_GIO == 17)
                                select new
                                {
                                    SO_GIO = ds.SO_GIO1
                                }).Single();

                return Json(new { code = 200, TimeFirst=TimeFirs, dsDon = dsDon, msg = "lay thanh cong" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        [HttpPost]
        public JsonResult UPDATEALLDAXONG( int TimeHT, string dateHT)
        {
            try
            {

                var ListTime = from ds in data.DON_DAT_SANs
                                    .Where(n => n.NGAY_DAT < DateTime.Parse(dateHT) || n.NGAY_DAT == DateTime.Parse(dateHT) && n.TIME_END < TimeHT)
                                select ds;

                ListTime.ToList().ForEach(item =>
                {
                    item.TINH_TRANG_DON = true;
                });
                UpdateModel(ListTime);
                data.SubmitChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult UPDATEALLCHUADA(int TimeHT, string dateHT)
        {
            try
            {

                var ListTime = from ds in data.DON_DAT_SANs
                                    .Where(n => n.NGAY_DAT > DateTime.Parse(dateHT) || n.NGAY_DAT == DateTime.Parse(dateHT) && n.TIME_START > TimeHT)
                               select ds;

                ListTime.ToList().ForEach(item =>
                {
                    item.TINH_TRANG_DON = null;
                });
                UpdateModel(ListTime);
                data.SubmitChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult UPDATEALLDANGDA(int TimeHT, string dateHT)
        {
            try
            {

                var ListTime = from ds in data.DON_DAT_SANs
                                    .Where(n => n.NGAY_DAT == DateTime.Parse(dateHT) && TimeHT>= n.TIME_START &&  TimeHT <= n.TIME_END)
                               select ds;

                ListTime.ToList().ForEach(item =>
                {
                    item.TINH_TRANG_DON = false;
                });
                UpdateModel(ListTime);
                data.SubmitChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        
        [HttpGet]
        public JsonResult CAYYJJ()
        {
            try
            {

                var ListTimer = (from ds in data.SO_GIOs
                                   
                               select new { MA = ds.SO_GIO1}).ToList();

                
                
                return Json(new { code = 200, ListTime1 = ListTimer }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        // LIST SÂN.......................................
        public ActionResult ViewListSan()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadListSan(int Trang,string TENSAN)
        {
            try
            {

                var ListSan = (from litsan in data.SANs 
                               .Where(n=>n.TEN_SAN.ToLower().Contains(TENSAN.ToLower()) && n.TINH_TRANG_XOA == false)
                               select new { 
                                   MA_SAN= litsan.MA_SAN,
                                   TEN_LOAI = litsan.LOAI_SAN.TEN_LOAI,
                                   GIA_SAN = litsan.GIA_SAN,
                                   TEN_SAN = litsan.TEN_SAN,
                }).OrderByDescending(n => n.MA_SAN).ToList();
                var PageZise = 4;
                var soTrang = SoTrang(ListSan.Count(), PageZise);
                var dsListSanPT = ListSan.Skip((Trang - 1) * PageZise).Take(PageZise).ToList();

                return Json(new { code = 200, dsListSanPT= dsListSanPT,soTrang= soTrang }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult LoadoptionLOAISAN()
        {
            try
            {

                var ListLOAISAN = (from litsan in data.LOAI_SANs
                               
                               select new
                               {
                                   MA_LOAI = litsan.MA_LOAI,
                                   TEN_LOAI = litsan.TEN_LOAI,
                                   
                               }).OrderByDescending(n => n.MA_LOAI).ToList();
                

                return Json(new { code = 200, ListLOAISAN=ListLOAISAN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult HIENGIASAN(int id)
        {
            try
            {

                var ListLOAISAN = (from litsan in data.LOAI_SANs
                                   .Where(n=>n.MA_LOAI == id)
                                   select new
                                   {
                                       MA_LOAI = litsan.MA_LOAI,
                                       TEN_LOAI = litsan.TEN_LOAI,
                                       GIA_SAN = litsan.GIA_LOAI,

                                   }).Single();


                return Json(new { code = 200, ListLOAISAN = ListLOAISAN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult ADDSAN(string MA_LOAI, string GIA_SAN, string TEN_SAN)
        {
            try
            {
                var san = new SAN();
                san.TEN_SAN = TEN_SAN;
                san.GIA_SAN = decimal.Parse(GIA_SAN);
                san.MA_LOAI =int.Parse(MA_LOAI);
                san.TINH_TRANG_XOA = false;

                data.SANs.InsertOnSubmit(san);
                data.SubmitChanges();

                return Json(new { code = 200}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult LoadListSanDetail(int id)
        {
            try
            {

                var ListSan = (from litsan in data.SANs
                               .Where(n => n.MA_SAN == id)
                               select new
                               {
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_LOAI = litsan.LOAI_SAN.TEN_LOAI,
                                   MA_LOAI  = litsan.MA_LOAI,
                                   GIA_SAN = litsan.GIA_SAN,
                                   TEN_SAN = litsan.TEN_SAN,
                               }).Single();
               

                return Json(new { code = 200, SanDetail = ListSan}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult EditListSan(int id, string MA_LOAI, string GIA_SAN,  string TEN_SAN)
        {
            try
            {

                var ListSan = data.SANs.First(n=> n.MA_SAN == id);
                ListSan.GIA_SAN = decimal.Parse(GIA_SAN);
                ListSan.TEN_SAN = TEN_SAN;
                ListSan.MA_LOAI = int.Parse(MA_LOAI);
                UpdateModel(ListSan);
                data.SubmitChanges();
                return Json(new { code = 200, GIA_SAN = GIA_SAN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult DELETESAN(int id)
        {
            try
            {
                var ListSan = data.SANs.First(n => n.MA_SAN == id);
                ListSan.TINH_TRANG_XOA = true;
                UpdateModel(ListSan);
                data.SubmitChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        //THỐNG KÊ

        public ActionResult THONGKE()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadCTSAN_THONGKE(string date_TK, string LOAI_SAN, string NGAY_BD, string NGAY_KT, string MA_SAN)
        {
            try
            {
                var ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n=>n.TINH_TRANG_THANH_TOAN == true )
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                if(NGAY_KT == "" && NGAY_BD == "" && LOAI_SAN == "" && date_TK == ""&& MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                              .Where(n => n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (NGAY_BD != "" && NGAY_KT == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (NGAY_BD == "" && NGAY_KT != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                              .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }

                if (date_TK != "" && LOAI_SAN != "" && MA_SAN !="")
                {
                     ListSan = (from litsan in data.CT_DAT_SANs
                                   .Where(n => n.NGAY_DAT == DateTime.Parse(date_TK)&& n.TINH_TRANG_THANH_TOAN==true && n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.MA_SAN == int.Parse(MA_SAN))
                                   select new
                                   {
                                       MA_DS = litsan.MA_DS,
                                       MA_SAN = litsan.MA_SAN,
                                       TEN_SAN = litsan.SAN.TEN_SAN,
                                       TONG_TIEN = litsan.TONG_TIEN,
                                       MA_LOAI = litsan.SAN.MA_LOAI,
                                       NGAY_DAT = litsan.NGAY_DAT,
                                       TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                                   }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if(date_TK != "" && LOAI_SAN == "" && MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                   .Where(n => n.NGAY_DAT == DateTime.Parse(date_TK) && n.TINH_TRANG_THANH_TOAN == true )
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (date_TK != "" && LOAI_SAN != "" && MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                   .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.NGAY_DAT == DateTime.Parse(date_TK) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (date_TK != "" && LOAI_SAN == "" && MA_SAN != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                   .Where(n => n.MA_SAN == int.Parse(MA_SAN) &&   n.NGAY_DAT == DateTime.Parse(date_TK) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }

                if (date_TK == "" && LOAI_SAN != "" && MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                 .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (date_TK == "" && LOAI_SAN == "" && MA_SAN != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                 .Where(n => n.MA_SAN == int.Parse(MA_SAN) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (date_TK == "" && LOAI_SAN != "" && MA_SAN != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                  .Where(n =>  n.TINH_TRANG_THANH_TOAN == true && n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.MA_SAN == int.Parse(MA_SAN))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }

                if (NGAY_BD !="" && NGAY_KT != "" && LOAI_SAN != "" && MA_SAN !="")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                .Where(n => n.MA_SAN == int.Parse(MA_SAN) && n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (NGAY_BD != "" && NGAY_KT != "" && LOAI_SAN == "" && MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                .Where(n =>  n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (NGAY_BD != "" && NGAY_KT != "" && LOAI_SAN == "" && MA_SAN != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                .Where(n => n.MA_SAN == int.Parse(MA_SAN)  && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
                if (NGAY_BD != "" && NGAY_KT != "" && LOAI_SAN != "" && MA_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                                .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT) && n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }

                return Json(new { code = 200, ListThongKe = ListSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult THONGKE_MIN_MAX(string NGAY_BD, string NGAY_KT, string LOAI_SAN,string date_TK)
        {
            try
            {
                var ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                if(NGAY_KT == ""&& NGAY_BD==""&& LOAI_SAN == "" && date_TK == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
               if(NGAY_BD!=""&& NGAY_KT == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
               if(NGAY_BD == "" && NGAY_KT != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                              .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
               if(NGAY_BD != "" && NGAY_KT != "" && LOAI_SAN != ""){

                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
               if (NGAY_BD != "" && NGAY_KT != "" && LOAI_SAN == "")
                {

                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }
               if (NGAY_BD == "" && NGAY_KT == "" && LOAI_SAN != "")
               {

                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.TINH_TRANG_THANH_TOAN == true )
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
               }
               if(date_TK != "" && LOAI_SAN != "")
               {
                    ListSan = (from litsan in data.CT_DAT_SANs
                              .Where(n => n.SAN.MA_LOAI == int.Parse(LOAI_SAN) && n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT == DateTime.Parse(date_TK))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
               }
               if(date_TK != "" && LOAI_SAN == "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                              .Where(n =>  n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT == DateTime.Parse(date_TK))
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderByDescending(n => n.MA_DS).ToList();
                }

                return Json(new { code = 200, ListThongKe = ListSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult LoadCTSAN_THONGKE_bieudo_cot(string NGAY_BD, string NGAY_KT)
        {
            try
            {
                var ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true)
                               select new
                               {
                                   MA_DS = litsan.MA_DS,
                                   MA_SAN = litsan.MA_SAN,
                                   TEN_SAN = litsan.SAN.TEN_SAN,
                                   TONG_TIEN = litsan.TONG_TIEN,
                                   MA_LOAI = litsan.SAN.MA_LOAI,
                                   NGAY_DAT = litsan.NGAY_DAT,
                                   TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                               }).OrderBy(n => n.NGAY_DAT).ToList();
                
                if(NGAY_BD !="" && NGAY_KT != "")
                {
                    ListSan = (from litsan in data.CT_DAT_SANs
                               .Where(n => n.TINH_TRANG_THANH_TOAN == true && n.NGAY_DAT >= DateTime.Parse(NGAY_BD)&& n.NGAY_DAT<= DateTime.Parse(NGAY_KT))
                                   select new
                                   {
                                       MA_DS = litsan.MA_DS,
                                       MA_SAN = litsan.MA_SAN,
                                       TEN_SAN = litsan.SAN.TEN_SAN,
                                       TONG_TIEN = litsan.TONG_TIEN,
                                       MA_LOAI = litsan.SAN.MA_LOAI,
                                       NGAY_DAT = litsan.NGAY_DAT,
                                       TEN_LOAI = litsan.SAN.LOAI_SAN.TEN_LOAI,
                                   }).OrderBy(n => n.NGAY_DAT).ToList();
                }

                return Json(new { code = 200, ListThongKe = ListSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        public ActionResult QLTK_ADMIN()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ListTKADM(string TEN_DN,  string QUYEN)
        {
            try
            {

                var ListADMIN = (from ds in data.ADMINs

                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                 }).OrderByDescending(n => n.MA_TK_ADM).ToList();

                if(TEN_DN != "" && QUYEN !="")
                {
                    ListADMIN = (from ds in data.ADMINs
                                .Where(n => n.TEN_DANG_NHAP_ADM.ToLower().Contains(TEN_DN.ToLower()) && n.MA_CHUC_VU == int.Parse(QUYEN) )
                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                 }).OrderByDescending(n => n.MA_TK_ADM).ToList();
                }
                else if (TEN_DN != "")
                {
                    ListADMIN = (from ds in data.ADMINs
                                .Where(n => n.TEN_DANG_NHAP_ADM.ToLower().Contains(TEN_DN.ToLower()) )
                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                 }).OrderByDescending(n => n.MA_TK_ADM).ToList();
                }
                else if(QUYEN != "")
                {
                    ListADMIN = (from ds in data.ADMINs
                                .Where(n => n.MA_CHUC_VU == int.Parse(QUYEN))
                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                 }).OrderByDescending(n => n.MA_TK_ADM).ToList();
                }
                else
                {
                    ListADMIN = (from ds in data.ADMINs
                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                 }).OrderByDescending(n => n.MA_TK_ADM).ToList();
                }
                

                return Json(new { code = 200, ListADMIN = ListADMIN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult ListQuyen()
        {
            try
            {

                var ListQuyen = (from ds in data.CHUC_VU_ADMs

                                 select new
                                 {
                                    MA_QUYEN = ds.MA_CHUC_VU,
                                    TEN_QUYEN = ds.TEN_CHUC_VU,
                                 }).ToList();

                return Json(new { code = 200, ListQuyen = ListQuyen}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult ADD_Admin(string TEN_DN, string MK_DN,string QUYEN)
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
                    TK.MA_CHUC_VU = int.Parse(QUYEN);
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
        public JsonResult TKADM_DETAIL(int id)
        {
            try
            {

                var ListADMIN = (from ds in data.ADMINs
                                 .Where(n=>n.STT == id)
                                 select new
                                 {
                                     MA_TK_ADM = ds.STT,
                                     TEN_DN_ADM = ds.TEN_DANG_NHAP_ADM,
                                     MK_ADM = ds.MAT_KHAU_ADM,
                                     TEN_QUYEN = ds.CHUC_VU_ADM.TEN_CHUC_VU,
                                     MA_QUYEN = ds.MA_CHUC_VU,
                                 }).Single();

                return Json(new { code = 200, ADMIN_detail = ListADMIN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult editTK_ADM(int id, string TEN_DN, string MK_DN, string QUYEN)
        {
            try
            {
                Boolean IsSuccess;
                var IsTK_ADM = data.ADMINs.FirstOrDefault(n => n.TEN_DANG_NHAP_ADM == TEN_DN && n.STT != id);
                var ListADMIN = data.ADMINs.Where(n => n.STT == id ).Single();
                if (IsTK_ADM == null)
                {
                    IsSuccess = true;
                    ListADMIN.MAT_KHAU_ADM = GetMD5(MK_DN);
                    ListADMIN.TEN_DANG_NHAP_ADM = TEN_DN;
                    ListADMIN.MA_CHUC_VU = int.Parse(QUYEN);
                    UpdateModel(ListADMIN);
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
        public JsonResult delete_ADM(int id)
        {
            try
            {
                
                var ListADMIN = data.ADMINs.Where(n => n.STT == id).Single();
                data.ADMINs.DeleteOnSubmit(ListADMIN);
                data.SubmitChanges();


                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        public ActionResult ListLoaiSan()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ListLoaiSan(string TenLoai, string GiaLoai)
        {
            try
            {

                var ListLoaiSan = (from ds in data.LOAI_SANs
                                   
                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI =ds.GIA_LOAI

                                   }).OrderByDescending(n => n.MA_LOAI).ToList();

              if(TenLoai != "" && GiaLoai !="")
                {
                    ListLoaiSan = (from ds in data.LOAI_SANs
                                   .Where(n=>n.TEN_LOAI.ToLower().Contains(TenLoai.ToLower())&& n.GIA_LOAI ==Decimal.Parse(GiaLoai))
                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI = ds.GIA_LOAI

                                   }).OrderByDescending(n => n.MA_LOAI).ToList();
                }
              else if(TenLoai != "")
                {
                    ListLoaiSan = (from ds in data.LOAI_SANs
                                  .Where(n => n.TEN_LOAI.ToLower().Contains(TenLoai.ToLower()))
                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI = ds.GIA_LOAI

                                   }).OrderByDescending(n => n.MA_LOAI).ToList();
                }
              else if(GiaLoai != "")
                {
                    ListLoaiSan = (from ds in data.LOAI_SANs
                                  .Where(n => n.GIA_LOAI == Decimal.Parse(GiaLoai))
                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI = ds.GIA_LOAI

                                   }).OrderByDescending(n => n.MA_LOAI).ToList();
                }
              else
                {
                    ListLoaiSan = (from ds in data.LOAI_SANs

                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI = ds.GIA_LOAI

                                   }).OrderByDescending(n => n.MA_LOAI).ToList();

                }


                return Json(new { code = 200, ListLoaiSan = ListLoaiSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult ADD_LOAI_SAN(string TEN_LOAI, string GIA_LOAI)
        {
            try
            {

                var LoaiSan = new LOAI_SAN();
                LoaiSan.TEN_LOAI = TEN_LOAI;
                LoaiSan.GIA_LOAI = Decimal.Parse(GIA_LOAI);
                data.LOAI_SANs.InsertOnSubmit(LoaiSan);
                data.SubmitChanges();
               

                return Json(new { code = 200,  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult edit_loai_san(int id, string TEN_LOAI, string GIA_LOAI)
        {
            try
            {
                var LS = data.LOAI_SANs.Where(n => n.MA_LOAI == id).Single(); ;
                LS.TEN_LOAI = TEN_LOAI;
                LS.GIA_LOAI = Decimal.Parse(GIA_LOAI);
                UpdateModel(LS);
                data.SubmitChanges();


                return Json(new { code = 200,  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult delete_LOAI_SAN(int id)
        {
            try
            {

                var ListLOAI = data.LOAI_SANs.Where(n => n.MA_LOAI== id).Single();
                data.LOAI_SANs.DeleteOnSubmit(ListLOAI);
                data.SubmitChanges();


                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public JsonResult LOAI_DETAIL(int id)
        {
            try
            {

                var LOAI_DETAIL = (from ds in data.LOAI_SANs
                                 .Where(n => n.MA_LOAI == id)
                                   select new
                                   {
                                       MA_LOAI = ds.MA_LOAI,
                                       TEN_LOAI = ds.TEN_LOAI,
                                       GIA_LOAI = ds.GIA_LOAI,
                                   }).Single();

                return Json(new { code = 200, LOAI_DETAIL = LOAI_DETAIL }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
    }
}
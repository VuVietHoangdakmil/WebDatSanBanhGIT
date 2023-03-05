using WebSanBongGit.Models;
//using mvcDangNhap.common;
//using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace DO_AN_CHUYEN_NGANH.Controllers
{
    public class WebSanBongController : Controller
    {
        DATA_SAN_BONGDataContext data = new DATA_SAN_BONGDataContext();
        // GET: WebSanBong
        private List<SAN> allSan()
        {
            return data.SANs.Where(n => n.TINH_TRANG_XOA == false).OrderByDescending(n => n.MA_SAN).ToList();
        }
        public ActionResult ListSan(int? page, string Search = "")
        {
            int pagesize = 8;
            int pagenum = (page ?? 1);
            if (Search != "")
            {

                var tkSDT = from ctd in data.SANs where ctd.TEN_SAN.ToUpper().Trim().Contains(Search.ToUpper().Trim()) select ctd;

                if (tkSDT.Count() == 0)
                {
                    ViewBag.ISKQtk = true;
                }
                else
                {
                    ViewBag.ISKQtk = false;
                }
                return View(tkSDT.OrderByDescending(n => n.MA_SAN).ToList().ToPagedList(pagenum, pagesize));
            }
            else
            {

                var listSan = allSan();
                return View(listSan.ToPagedList(pagenum, pagesize));
            }
        }
        public ActionResult parttialviewLOAISAN()
        {
            var loaiSAN = from LS in data.LOAI_SANs select LS;
            return PartialView(loaiSAN);
        }
        public ActionResult SanTheoLoai(int id, int? page)
        {
            int pagesize = 8;
            int pagenum = (page ?? 1);
            var santheoloai = from sanTL in data.SANs where sanTL.MA_LOAI == id select sanTL;
            var tenloai = from san in data.LOAI_SANs where san.MA_LOAI == id select san.TEN_LOAI;
            ViewBag.TENLOAI = tenloai.Single();
            return View(santheoloai.OrderByDescending(n => n.MA_SAN).ToPagedList(pagenum, pagesize));

        }
        public ActionResult DatsanCHITIET(int id, string dateTKK = "")
        {

            if (Session["TaiKhoan"] == null)
            {
                Session["isKT3"] = true;
                return RedirectToAction("ListSan", "WebSanBong");
            }
            else
            {
                Session["MA_SAN1"] = id;
                var sangia = from san1 in data.SANs where san1.MA_SAN == id select san1.GIA_SAN;
                ViewBag.GIASAN = sangia.Single();
                ViewBag.hourtEnd_DH = new SelectList(data.SO_GIOs.ToList().OrderBy(n => n.SO_GIO1), "SO_GIO1", "SO_GIO1");
                ViewBag.hourtStart_DH = new SelectList(data.SO_GIOs.ToList().OrderBy(n => n.SO_GIO1), "SO_GIO1", "SO_GIO1");
                ViewBag.minuteEnd_DH = new SelectList(data.SO_PHUTs.ToList().OrderBy(n => n.SO_PHUT1), "SO_PHUT1", "SO_PHUT1");
                ViewBag.minute_DH = new SelectList(data.SO_PHUTs.ToList().OrderBy(n => n.SO_PHUT1), "SO_PHUT1", "SO_PHUT1");
                if (dateTKK == "")
                {
                    var tkSDT = from datsan in data.DON_DAT_SANs where datsan.MA_DS == -1239861272987 select datsan;
                    return View(tkSDT);
                }
                else
                {

                    ViewBag.timeDS = dateTKK;
                    var tkSDT = from datsan in data.DON_DAT_SANs where datsan.NGAY_DAT == DateTime.Parse(dateTKK) && datsan.MA_SAN == id && datsan.TRANG_THAI_XOA == false select datsan;

                    return View(tkSDT.OrderBy(n => n.GIO_BAT_DAU));
                }

            }
        }
        [HttpPost]
        public ActionResult DatsanCHITIET2(FormCollection collection, DON_DAT_SAN dds, CT_DAT_SAN ctds)
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("ListSan", "WebSanBong");
            }
            else
            {
                KHACH_HANG khSession = (KHACH_HANG)Session["Taikhoan"];
                var KH = from kh4 in data.KHACH_HANGs where kh4.MA_KH == khSession.MA_KH select kh4;

                var masan = collection["MASAN_DH"];
                var san = from san1 in data.SANs where san1.MA_SAN == int.Parse(masan) select san1.GIA_SAN;
                var ngayday = String.Format("{0:MM/dd/yyyy}", collection["DATE_DH"]).Trim();

                var giobd = collection["hourtStart_DH"];
                var phutbd = collection["minute_DH"];
                var Sgiobd = giobd + ":" + phutbd;
                var checkSgiobd = giobd.Trim() + "" + phutbd.Trim();

                var gioKT = collection["hourtEnd_DH"];
                var phutKT = collection["minuteEnd_DH"];
                var Sgiokt = gioKT + ":" + phutKT;
                var checkSgiokt = gioKT.Trim() + "" + phutKT.Trim();
                var Sgiobd2 = gioKT.Trim() + ":" + phutKT.Trim();

                // tinh tong tien nha
                int sumGio;
                int sumPHUT;




                sumPHUT = int.Parse(phutKT) - int.Parse(phutbd);
                if (int.Parse(phutKT) - int.Parse(phutbd) == -30)
                {
                    sumGio = (int.Parse(gioKT) - int.Parse(giobd)) - 1;
                }
                else
                {
                    sumGio = int.Parse(gioKT) - int.Parse(giobd);
                }

                var moneyGIO = (decimal)0;
                var tiensan = san.Single();

                moneyGIO = sumGio * (decimal)tiensan;

                var moneyphut = (decimal)0;
                if (Math.Abs(sumPHUT) == 30)
                {
                    moneyphut = (decimal)tiensan / 2;

                    if (int.Parse(checkSgiobd) >= 1800 || int.Parse(checkSgiokt) > 1800)
                    {
                        dds.TONG_TIEN = (decimal)moneyGIO + moneyphut + tiensan;

                    }
                    else
                    {
                        dds.TONG_TIEN = (decimal)moneyGIO + moneyphut;
                    }


                    if (sumGio < 10 || Math.Abs(sumPHUT) < 10)
                    {
                        if (sumGio < 10 && Math.Abs(sumPHUT) < 10)
                        {
                            dds.TONG_GIO_THUE = "0" + sumGio + ":" + "0" + Math.Abs(sumPHUT);
                        }
                        else if (sumGio < 10)
                        {
                            dds.TONG_GIO_THUE = "0" + sumGio + ":" + Math.Abs(sumPHUT);
                        }
                        else
                        {
                            dds.TONG_GIO_THUE = sumGio + ":" + "0" + Math.Abs(sumPHUT);
                        }
                    }
                    else
                    {
                        dds.TONG_GIO_THUE = sumGio + ":" + Math.Abs(sumPHUT);
                    }
                }
                else
                {
                    if (int.Parse(checkSgiobd) >= 1800 || int.Parse(checkSgiokt) > 1800)
                    {
                        dds.TONG_TIEN = (decimal)moneyGIO + moneyphut + tiensan;
                    }
                    else
                    {
                        dds.TONG_TIEN = (decimal)moneyGIO + moneyphut;
                    }

                    if (sumGio < 10 || Math.Abs(sumPHUT) < 10)
                    {
                        if (sumGio < 10 && Math.Abs(sumPHUT) < 10)
                        {
                            dds.TONG_GIO_THUE = "0" + sumGio + ":" + "0" + Math.Abs(sumPHUT);
                        }
                        else if (sumGio < 10)
                        {
                            dds.TONG_GIO_THUE = "0" + sumGio + ":" + Math.Abs(sumPHUT);
                        }
                        else
                        {
                            dds.TONG_GIO_THUE = sumGio + ":" + "0" + Math.Abs(sumPHUT);
                        }
                    }
                    else
                    {
                        dds.TONG_GIO_THUE = sumGio + ":" + Math.Abs(sumPHUT);
                    }
                }


                dds.MA_KH = khSession.MA_KH;
                dds.NGAY_DAT = DateTime.Parse(ngayday);
                dds.GIO_BAT_DAU = DateTime.Parse(String.Format("{0:HH:mm}", Sgiobd));
                dds.GIO_KET_THUC = DateTime.Parse(String.Format("{0:HH:mm}", Sgiokt));
                dds.GIO_KET_THUC_TEXT = Sgiobd2;
                dds.MA_SAN = int.Parse(masan);
                dds.GIA_SAN = tiensan;
                dds.TIME_START = int.Parse(checkSgiobd);
                dds.TIME_END = int.Parse(checkSgiokt);
                dds.TRANG_THAI_XOA = false;
                dds.NGAY_LAP_PHIEU = DateTime.Now;
                data.DON_DAT_SANs.InsertOnSubmit(dds);
                data.SubmitChanges();

                // them csld chitiet
                var DDS = from dds1 in data.DON_DAT_SANs select dds1;
                ctds.MA_DS = DDS.ToList().Last().MA_DS;
                ctds.MA_SAN = DDS.ToList().Last().MA_SAN;
                ctds.NGAY_DAT = DDS.ToList().Last().NGAY_DAT;
                ctds.TONG_TIEN = DDS.ToList().Last().TONG_TIEN;
                ctds.TINH_TRANG_THANH_TOAN = false;
                data.CT_DAT_SANs.InsertOnSubmit(ctds);
                data.SubmitChanges();

                // gui mail
                string content = System.IO.File.ReadAllText(Server.MapPath("~/content/temlate/mailcontent.html"));
                content = content.Replace("{{CustomerName}}", KH.Single().TEN_KH);
                content = content.Replace("{{Phone}}", KH.Single().SDT);
                content = content.Replace("{{Email}}", KH.Single().EMAIL);
                content = content.Replace("{{NGAY_DAT}}", String.Format("{0:dd/MM/yyyy}", dds.NGAY_DAT));
                content = content.Replace("{{TIME_DAT}}", String.Format("{0:HH:mm}", dds.GIO_BAT_DAU) + '-' + String.Format("{0:HH:mm}", dds.GIO_KET_THUC));
                content = content.Replace("{{Total}}", String.Format("{0:0,0}", dds.TONG_TIEN));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                //new MailHelper().SendMail(KH.Single().EMAIL, "Sân mới đặt từ Sân Bóng AE", content);
                //new MailHelper().SendMail(toEmail, "Sân mới đặt từ Sân Bóng AE", content);

            }
            return RedirectToAction("ListSan", "WebSanBong");
        }
        [HttpPost]
        public JsonResult TienHanhDatSan(string MA_SAN, string DATE_DA, string HoursStart, string minuteStart, string HoursEnd, string minuteEnd, DON_DAT_SAN dds, CT_DAT_SAN ctds)
        {
            try
            {
                Boolean DN;
                if (Session["TaiKhoan"] == null)
                {
                    //return RedirectToAction("ListSan", "WebSanBong");
                    DN = false;
                }
                else
                {
                    DN = true;
                    KHACH_HANG khSession = (KHACH_HANG)Session["Taikhoan"];
                    var KH = from kh4 in data.KHACH_HANGs where kh4.MA_KH == khSession.MA_KH select kh4;

                    var masan = MA_SAN;
                    var san = from san1 in data.SANs where san1.MA_SAN == int.Parse(masan) select san1.GIA_SAN;
                    var ngayday = String.Format("{0:MM/dd/yyyy}", DATE_DA).Trim();

                    var giobd = HoursStart;
                    var phutbd = minuteStart;
                    var Sgiobd = giobd + ":" + phutbd;
                    var checkSgiobd = giobd.Trim() + "" + phutbd.Trim();

                    var gioKT = HoursEnd;
                    var phutKT = minuteEnd;
                    var Sgiokt = gioKT + ":" + phutKT;
                    var checkSgiokt = gioKT.Trim() + "" + phutKT.Trim();
                    var Sgiobd2 = gioKT.Trim() + ":" + phutKT.Trim();

                    // tinh tong tien nha
                    int sumGio;
                    int sumPHUT;




                    sumPHUT = int.Parse(phutKT) - int.Parse(phutbd);
                    if (int.Parse(phutKT) - int.Parse(phutbd) == -30)
                    {
                        sumGio = (int.Parse(gioKT) - int.Parse(giobd)) - 1;
                    }
                    else
                    {
                        sumGio = int.Parse(gioKT) - int.Parse(giobd);
                    }

                    var moneyGIO = (decimal)0;
                    var tiensan = san.Single();

                    moneyGIO = sumGio * (decimal)tiensan;

                    var moneyphut = (decimal)0;
                    if (Math.Abs(sumPHUT) == 30)
                    {
                        moneyphut = (decimal)tiensan / 2;

                        if (int.Parse(checkSgiobd) >= 1800 || int.Parse(checkSgiokt) > 1800)
                        {
                            dds.TONG_TIEN = (decimal)moneyGIO + moneyphut + tiensan;

                        }
                        else
                        {
                            dds.TONG_TIEN = (decimal)moneyGIO + moneyphut;
                        }


                        if (sumGio < 10 || Math.Abs(sumPHUT) < 10)
                        {
                            if (sumGio < 10 && Math.Abs(sumPHUT) < 10)
                            {
                                dds.TONG_GIO_THUE = "0" + sumGio + ":" + "0" + Math.Abs(sumPHUT);
                            }
                            else if (sumGio < 10)
                            {
                                dds.TONG_GIO_THUE = "0" + sumGio + ":" + Math.Abs(sumPHUT);
                            }
                            else
                            {
                                dds.TONG_GIO_THUE = sumGio + ":" + "0" + Math.Abs(sumPHUT);
                            }
                        }
                        else
                        {
                            dds.TONG_GIO_THUE = sumGio + ":" + Math.Abs(sumPHUT);
                        }
                    }
                    else
                    {
                        if (int.Parse(checkSgiobd) >= 1800 || int.Parse(checkSgiokt) > 1800)
                        {
                            dds.TONG_TIEN = (decimal)moneyGIO + moneyphut + tiensan;
                        }
                        else
                        {
                            dds.TONG_TIEN = (decimal)moneyGIO + moneyphut;
                        }

                        if (sumGio < 10 || Math.Abs(sumPHUT) < 10)
                        {
                            if (sumGio < 10 && Math.Abs(sumPHUT) < 10)
                            {
                                dds.TONG_GIO_THUE = "0" + sumGio + ":" + "0" + Math.Abs(sumPHUT);
                            }
                            else if (sumGio < 10)
                            {
                                dds.TONG_GIO_THUE = "0" + sumGio + ":" + Math.Abs(sumPHUT);
                            }
                            else
                            {
                                dds.TONG_GIO_THUE = sumGio + ":" + "0" + Math.Abs(sumPHUT);
                            }
                        }
                        else
                        {
                            dds.TONG_GIO_THUE = sumGio + ":" + Math.Abs(sumPHUT);
                        }

                    }


                    dds.MA_KH = khSession.MA_KH;
                    dds.NGAY_DAT = DateTime.Parse(ngayday);
                    dds.GIO_BAT_DAU = DateTime.Parse(String.Format("{0:HH:mm}", Sgiobd));
                    dds.GIO_KET_THUC = DateTime.Parse(String.Format("{0:HH:mm}", Sgiokt));
                    dds.GIO_KET_THUC_TEXT = Sgiobd2;
                    dds.MA_SAN = int.Parse(masan);
                    dds.GIA_SAN = tiensan;
                    dds.TIME_START = int.Parse(checkSgiobd);
                    dds.TIME_END = int.Parse(checkSgiokt);
                    dds.TRANG_THAI_XOA = false;
                    dds.NGAY_LAP_PHIEU = DateTime.Now;
                    data.DON_DAT_SANs.InsertOnSubmit(dds);
                    data.SubmitChanges();

                    // them csld chitiet
                    var DDS = from dds1 in data.DON_DAT_SANs select dds1;
                    ctds.MA_DS = DDS.ToList().Last().MA_DS;
                    ctds.MA_SAN = DDS.ToList().Last().MA_SAN;
                    ctds.NGAY_DAT = DDS.ToList().Last().NGAY_DAT;
                    ctds.TONG_TIEN = DDS.ToList().Last().TONG_TIEN;
                    ctds.TINH_TRANG_THANH_TOAN = false;
                    data.CT_DAT_SANs.InsertOnSubmit(ctds);
                    data.SubmitChanges();

                    // gui mail
                    string content = System.IO.File.ReadAllText(Server.MapPath("~/content/temlate/mailcontent.html"));
                    content = content.Replace("{{CustomerName}}", KH.Single().TEN_KH);
                    content = content.Replace("{{Phone}}", KH.Single().SDT);
                    content = content.Replace("{{Email}}", KH.Single().EMAIL);
                    content = content.Replace("{{NGAY_DAT}}", String.Format("{0:dd/MM/yyyy}", dds.NGAY_DAT));
                    content = content.Replace("{{TIME_DAT}}", String.Format("{0:HH:mm}", dds.GIO_BAT_DAU) + '-' + String.Format("{0:HH:mm}", dds.GIO_KET_THUC));
                    content = content.Replace("{{Total}}", String.Format("{0:0,0}", dds.TONG_TIEN));
                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    //new MailHelper().SendMail(KH.Single().EMAIL, "Sân mới đặt từ Sân Bóng AE", content);
                    //new MailHelper().SendMail(toEmail, "Sân mới đặt từ Sân Bóng AE", content);

                }


                return Json(new { code = 200, ISlogin = DN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        public ActionResult ViewTimKiemKhungGio()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ListDDS(string NGAY_BD, string NGAY_KT)
        {

            try
            {

                var dsDon = (from ds in data.DON_DAT_SANs
                           .Where(n => n.TRANG_THAI_XOA == false && n.NGAY_DAT >= DateTime.Parse(NGAY_BD) && n.NGAY_DAT <= DateTime.Parse(NGAY_KT))
                             select new
                             {
                                 MA_DS = ds.MA_DS,
                                 GIO_BD = ds.GIO_BAT_DAU,
                                 MA_SAN = ds.MA_SAN,
                                 NGAY_DA = ds.NGAY_DAT,
                                 TEN_SAN = ds.SAN.TEN_SAN,
                                 GIA_SAN = ds.GIA_SAN,
                                 TIME_START = ds.TIME_START,
                                 TIME_END = ds.TIME_END,
                                 LOAI_SAN = ds.SAN.LOAI_SAN.TEN_LOAI,

                             }).OrderBy(n => n.GIO_BD).ToList();


                return Json(new { code = 200, dsDon = dsDon }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult ListSAN1()
        {

            try
            {

                var dsSan = (from ds in data.SANs
                           .Where(n => n.TINH_TRANG_XOA == false)
                             select new
                             {
                                 MA_SAN = ds.MA_SAN,
                                 MA_LOAI = ds.MA_LOAI,
                                 GIA_SAN = ds.GIA_SAN,
                                 TEN_SAN = ds.TEN_SAN,
                                 LOAI_SAN = ds.LOAI_SAN.TEN_LOAI

                             }).OrderBy(n => n.MA_SAN).ToList();

                return Json(new { code = 200, dsSan = dsSan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult ListTime()
        {
            try
            {
                var dsTime = (from ds in data.SO_GIOs

                              select new
                              {
                                  MA_SO_GIO = ds.MA_SO_GIO,
                                  SO_GIO = ds.SO_GIO1

                              }).ToList();


                return Json(new { code = 200, dsTime = dsTime }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpGet]
        public JsonResult ListTimephut()
        {
            try
            {
                var dsphut = (from ds in data.SO_PHUTs

                              select new
                              {
                                  SO_PHUT = ds.SO_PHUT1

                              }).ToList();


                return Json(new { code = 200, dsphut = dsphut }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lay ko thanh cong" + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }


        public ActionResult TT_DAT_SAN(int id, int? page)
        {
            int pagesize = 8;
            int pagenum = (page ?? 1);
            var dsDon1 = data.DON_DAT_SANs.Where(n => n.TRANG_THAI_XOA == false && n.MA_KH == id).ToList();
            return View(dsDon1.OrderByDescending(n => n.MA_DS).ToPagedList(pagenum, pagesize));
        }


    }
}
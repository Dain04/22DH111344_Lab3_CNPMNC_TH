using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models;
using DemoDB2.ViewModels;

namespace DemoDB2.Controllers
{
    public class DichVuController : Controller
    {
        QLKSEntities database = new QLKSEntities();

        public ActionResult AddMonAn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMonAn(MonAn monAn)
        {
            try
            {
                var existingMonAn = database.MonAn.FirstOrDefault(x => x.MonAnID == monAn.MonAnID);
                if (existingMonAn != null)
                {
                    ModelState.AddModelError(string.Empty, "MonAnID đã tồn tại.");
                    return View(monAn);
                }

                if (monAn.UploadImage != null && monAn.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(monAn.UploadImage.FileName);
                    string extension = Path.GetExtension(monAn.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    monAn.ImageMonAn = "~/Content/images/" + filename;
                    monAn.UploadImage.SaveAs(path);
                }

                database.MonAn.Add(monAn);
                database.SaveChanges();
                return RedirectToAction("ViewMonAn");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(monAn);
            }
        }
        public ActionResult ViewMonAnKH(string searchString)
        {
            var monAn = database.MonAn.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                monAn = monAn.Where(s => s.TenMon.Contains(searchString));
            }

            return View(monAn.ToList());
        }
        public ActionResult ViewMonAnKHWithPrice(string searchString, decimal? minPrice, decimal? maxPrice)
        {
            var monAn = database.MonAn.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                monAn = monAn.Where(s => s.TenMon.Contains(searchString));
            }

            if (minPrice.HasValue)
            {
                monAn = monAn.Where(s => s.GiaMon >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                monAn = monAn.Where(s => s.GiaMon <= maxPrice.Value);
            }

            return View(monAn.ToList());
        }
        public ActionResult EditMonAn(int id)
        {
            var monAn = database.MonAn.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMonAn(int id, MonAn monAn)
        {
            if (ModelState.IsValid)
            {
                if (monAn.UploadImage != null && monAn.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(monAn.UploadImage.FileName);
                    string extension = Path.GetExtension(monAn.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    monAn.ImageMonAn = "~/Content/images/" + filename;
                    monAn.UploadImage.SaveAs(path);
                }

                database.Entry(monAn).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("ViewMonAn");
            }
            return View(monAn);
        }

        public ActionResult DeleteMonAn(int id)
        {
            var monAn = database.MonAn.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        [HttpPost, ActionName("DeleteMonAn")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMonAnConfirmed(int id)
        {
            var monAn = database.MonAn.Find(id);
            if (monAn != null)
            {
                // Xóa file ảnh cũ nếu có
                if (!string.IsNullOrEmpty(monAn.ImageMonAn))
                {
                    string fullPath = Server.MapPath(monAn.ImageMonAn);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                database.MonAn.Remove(monAn);
                database.SaveChanges();
            }
            return RedirectToAction("ViewMonAn");
        }

        public ActionResult ViewMonAn(string _name)
        {
            if (_name == null)
                return View(database.MonAn.ToList());
            else
                return View(database.MonAn.Where(s => s.TenMon.Contains(_name)).ToList());
        }
        /////////////////////////////////////////////////////////////////////////Xe////////
        public ActionResult CreateXe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateXe(Xe xe)
        {
            try
            {
                var existingXe = database.Xe.FirstOrDefault(x => x.XeID == xe.XeID);
                if (existingXe != null)
                {
                    ModelState.AddModelError(string.Empty, "XeID đã tồn tại.");
                    return View(xe);
                }

                if (xe.UploadImage != null && xe.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(xe.UploadImage.FileName);
                    string extension = Path.GetExtension(xe.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    xe.ImageXe = "~/Content/images/" + filename;
                    xe.UploadImage.SaveAs(path);
                }

                database.Xe.Add(xe);
                database.SaveChanges();
                return RedirectToAction("ViewXe");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(xe);
            }
        }

        public ActionResult EditXe(int id)
        {
            var xe = database.Xe.Find(id);
            if (xe == null)
            {
                return HttpNotFound();
            }
            return View(xe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditXe(int id, Xe xe)
        {
            if (ModelState.IsValid)
            {
                database.Entry(xe).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("ViewXe");
            }
            return View(xe);
        }

        public ActionResult DeleteXe(int id)
        {
            var xe = database.Xe.Find(id);
            if (xe == null)
            {
                return HttpNotFound();
            }
            return View(xe);
        }

        [HttpPost, ActionName("DeleteXe")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteXeConfirmed(int id)
        {
            var xe = database.Xe.Find(id);
            database.Xe.Remove(xe);
            database.SaveChanges();
            return RedirectToAction("ViewXe");
        }

        public ActionResult ViewXe(string _name)
        {
            if (_name == null)
                return View(database.Xe.ToList());
            else
                return View(database.Xe.Where(s => s.HieuXe.Contains(_name)).ToList());
        }
        public ActionResult ViewXeKH(string searchString)
        {
            var xe = database.Xe.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                xe = xe.Where(s => s.HieuXe.Contains(searchString) || s.BienSoXe.Contains(searchString));
            }

            return View(xe.ToList());
        }
        public ActionResult ViewXeKHWithPrice(string searchString, decimal? minPrice, decimal? maxPrice)
        {
            var xe = database.Xe.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                xe = xe.Where(s => s.HieuXe.Contains(searchString) || s.BienSoXe.Contains(searchString));
            }

            if (minPrice.HasValue)
            {
                xe = xe.Where(s => s.GiaXe >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                xe = xe.Where(s => s.GiaXe <= maxPrice.Value);
            }

            return View(xe.ToList());
        }
        [HttpPost]
        public ActionResult DatMon(int monAnId, string tenMon, decimal giaMon)
        {
            var gioHang = Session["GioHang"] as List<GioHangViewModel> ?? new List<GioHangViewModel>();
            var monAnTrongGio = gioHang.FirstOrDefault(m => m.MonAnId == monAnId);
            if (monAnTrongGio != null)
            {
                monAnTrongGio.SoLuong++;
            }
            else
            {
                gioHang.Add(new GioHangViewModel
                {
                    MonAnId = monAnId,
                    TenMon = tenMon,
                    GiaMon = giaMon,
                    SoLuong = 1
                });
            }
            Session["GioHang"] = gioHang;
            return Json(new { success = true, message = "Đã thêm món vào giỏ hàng", gioHangCount = gioHang.Sum(m => m.SoLuong) });
        }
        public ActionResult XemGioHang()
        {
            var gioHang = Session["GioHang"] as List<GioHangViewModel> ?? new List<GioHangViewModel>();
            return View(gioHang);
        }
        [HttpPost]
        public ActionResult CapNhatGioHang(int monAnId, int soLuong)
        {
            var gioHang = Session["GioHang"] as List<GioHangViewModel> ?? new List<GioHangViewModel>();
            var monAnTrongGio = gioHang.FirstOrDefault(m => m.MonAnId == monAnId);
            if (monAnTrongGio != null)
            {
                if (soLuong > 0)
                {
                    monAnTrongGio.SoLuong = soLuong;
                }
                else
                {
                    gioHang.Remove(monAnTrongGio);
                }
            }
            Session["GioHang"] = gioHang;
            return Json(new { success = true, gioHangCount = gioHang.Sum(m => m.SoLuong) });
        }
        public ActionResult ThanhToan()
        {
            var viewModel = new ChiTietHoaDonViewModel();

            var gioHang = Session["GioHang"] as List<GioHangViewModel> ?? new List<GioHangViewModel>();
            var chiTietHoaDon = new ChiTietHoaDonViewModel
            {
                MonAn = gioHang,
                TongTien = gioHang.Sum(m => m.GiaMon * m.SoLuong)
            };
            viewModel.QRCodeData = "https://example.com/payment?id=";

            return View(chiTietHoaDon);
        }

        [HttpPost]
        public ActionResult XacNhanThanhToan(string phuongThucThanhToan)
        {
            if (phuongThucThanhToan == "ChuyenKhoan" || phuongThucThanhToan == "The")
            {
                // Giả lập tạo mã QR
                string maQR = "https://example.com/qr/" + Guid.NewGuid().ToString();
                return Json(new { success = true, requiresQR = true, qrCode = maQR });
            }
            else
            {
                // Xử lý thanh toán tiền mặt
                Session["GioHang"] = null;
                TempData["ThongBaoThanhCong"] = "Thanh toán thành công!";
                return Json(new { success = true, requiresQR = false, redirectUrl = Url.Action("ViewMonAnKH") });
            }
        }

        [HttpPost]
        public ActionResult HoanTatThanhToan()
        {
            // Xử lý hoàn tất thanh toán
            Session["GioHang"] = null;
            TempData["ThongBaoThanhCong"] = "Thanh toán thành công!";
            return Json(new { success = true, redirectUrl = Url.Action("ViewMonAnKH") });
        }
        [HttpPost]
        public ActionResult DatXe(int xeId, string hieuXe, decimal giaXe)
        {
            var gioHang = Session["GioHangXe"] as List<GioHangXeViewModel> ?? new List<GioHangXeViewModel>();
            var xeTrongGio = gioHang.FirstOrDefault(x => x.XeId == xeId);
            if (xeTrongGio != null)
            {
                xeTrongGio.SoLuong++;
            }
            else
            {
                gioHang.Add(new GioHangXeViewModel
                {
                    XeId = xeId,
                    HieuXe = hieuXe,
                    GiaXe = giaXe,
                    SoLuong = 1
                });
            }
            Session["GioHangXe"] = gioHang;
            return Json(new { success = true, message = "Chọn xe thành công", gioHangCount = gioHang.Sum(x => x.SoLuong) });
        }

        public ActionResult ThanhToanXe()
        {
            var gioHang = Session["GioHangXe"] as List<GioHangXeViewModel> ?? new List<GioHangXeViewModel>();
            var chiTietHoaDon = new ChiTietHoaDonXeViewModel
            {
                Xe = gioHang,
                TongTien = gioHang.Sum(x => x.GiaXe * x.SoLuong)
            };
            return View(chiTietHoaDon);
        }

        [HttpPost]
        public ActionResult XacNhanThanhToanXe(string phuongThucThanhToan)
        {
            try
            {
                // Xử lý thanh toán ở đây
                Session["GioHangXe"] = null;
                TempData["ThongBaoThanhCong"] = "Đặt xe thành công! Tài xế sẽ liên lạc lại với bạn.";
                return Json(new { success = true, redirectUrl = Url.Action("ViewXeKH") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult AddSpa()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpa(Spa spa)
        {
            try
            {
                // Handle image upload
                if (spa.UploadImage != null && spa.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(spa.UploadImage.FileName);
                    string extension = Path.GetExtension(spa.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    spa.ImageSpa = "~/Content/images/" + filename;
                    spa.UploadImage.SaveAs(path);
                }

                // Save spa details
                database.Spa.Add(spa);
                database.SaveChanges();
                return RedirectToAction("ViewSpa");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(spa);
            }
        }

        public ActionResult ViewSpa()
        {
            var spas = database.Spa.ToList();
            return View(spas);
        }
        public ActionResult ViewSpaKH()
        {
            var spas = database.Spa.ToList();
            return View(spas);
        }
    }
}
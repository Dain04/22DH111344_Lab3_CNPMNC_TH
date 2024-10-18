using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
                if (string.IsNullOrWhiteSpace(monAn.TenMon) || monAn.TenMon.Length <= 1)
                {
                    ModelState.AddModelError("TenMon", "Tên món ăn không hợp lệ. Hãy nhập lại.");
                }

                if (!monAn.GiaMon.HasValue || monAn.GiaMon.Value <= 0)
                {
                    ModelState.AddModelError("GiaMon", "Giá món không hợp lệ. Hãy nhập một giá trị lớn hơn 0.");
                }

                if (monAn.UploadImage == null || monAn.UploadImage.ContentLength == 0)
                {
                    ModelState.AddModelError("UploadImage", "Vui lòng chọn một hình ảnh cho món ăn.");
                }

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

                    database.MonAn.Add(monAn);
                    database.SaveChanges();
                    return RedirectToAction("ViewMonAn");
                }

                return View(monAn);
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
            try
            {
                var existingMonAn = database.MonAn.Find(id);
                if (existingMonAn == null)
                {
                    return HttpNotFound();
                }

                if (string.IsNullOrWhiteSpace(monAn.TenMon) || monAn.TenMon.Length <= 1)
                {
                    ModelState.AddModelError("TenMon", "Tên món ăn không hợp lệ. Hãy nhập lại.");
                }
                if (!monAn.GiaMon.HasValue || monAn.GiaMon.Value <= 0)
                {
                    ModelState.AddModelError("GiaMon", "Giá món không hợp lệ. Hãy nhập một giá trị lớn hơn 0.");
                }

                if (ModelState.IsValid)
                {
                    existingMonAn.TenMon = monAn.TenMon;
                    existingMonAn.GiaMon = monAn.GiaMon;
                    // Cập nhật các trường khác nếu cần

                    if (monAn.UploadImage != null && monAn.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(monAn.UploadImage.FileName);
                        string extension = Path.GetExtension(monAn.UploadImage.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        existingMonAn.ImageMonAn = "~/Content/images/" + filename;
                        monAn.UploadImage.SaveAs(path);
                    }

                    database.Entry(existingMonAn).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("ViewMonAn");
                }
                return View(existingMonAn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(database.MonAn.Find(id));
            }
        }

        [HttpGet]
        public ActionResult DeleteMonAn(int id)
        {
            var monAn = database.MonAn.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        // POST: Xử lý việc xóa món ăn
        [HttpPost, ActionName("DeleteMonAn")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMonAnConfirmed(int id)
        {
            try
            {
                var monAn = database.MonAn.Find(id);
                if (monAn == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra xem có DichVuSuDung nào liên quan không
                var relatedDichVu = database.DichVuSuDung.Any(d => d.MonAnID == id);
                if (relatedDichVu)
                {
                    ModelState.AddModelError("", "Không thể xóa món ăn này vì nó đang được sử dụng trong các dịch vụ.");
                    return View(monAn);
                }

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

                TempData["SuccessMessage"] = "Món ăn đã được xóa thành công.";
                return RedirectToAction("ViewMonAn");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa món ăn: " + ex.Message);
                return View(database.MonAn.Find(id));
            }
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
                // Kiểm tra các ràng buộc cho HieuXe
                if (string.IsNullOrWhiteSpace(xe.HieuXe))
                {
                    ModelState.AddModelError("HieuXe", "Vui lòng nhập hãng xe.");
                }
                else if (xe.HieuXe.Length < 2 || xe.HieuXe.Length > 50)
                {
                    ModelState.AddModelError("HieuXe", "Hãng xe phải có độ dài từ 2 đến 50 ký tự.");
                }

                // Kiểm tra các ràng buộc cho BienSoXe
                if (string.IsNullOrWhiteSpace(xe.BienSoXe))
                {
                    ModelState.AddModelError("BienSoXe", "Vui lòng nhập biển số xe.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(xe.BienSoXe, @"^\d{2}[A-Z]-\d{3}\.\d{2}$"))
                {
                    ModelState.AddModelError("BienSoXe", "Biển số xe không đúng định dạng (VD: 51F-123.45).");
                }

                // Kiểm tra các ràng buộc cho TaiXe
                if (string.IsNullOrWhiteSpace(xe.TaiXe))
                {
                    ModelState.AddModelError("TaiXe", "Vui lòng nhập tên tài xế.");
                }
                else if (xe.TaiXe.Length < 2 || xe.TaiXe.Length > 100)
                {
                    ModelState.AddModelError("TaiXe", "Tên tài xế phải có độ dài từ 2 đến 100 ký tự.");
                }

                // Kiểm tra các ràng buộc cho SoChoNgoi
                if (!xe.SoChoNgoi.HasValue)
                {
                    ModelState.AddModelError("SoChoNgoi", "Vui lòng nhập số chỗ ngồi.");
                }
                else if (xe.SoChoNgoi.Value < 4 || xe.SoChoNgoi.Value > 50)
                {
                    ModelState.AddModelError("SoChoNgoi", "Số chỗ ngồi phải từ 4 đến 50 chỗ.");
                }

                // Kiểm tra các ràng buộc cho GiaXe (Giá thuê xe theo ngày)
                if (!xe.GiaXe.HasValue)
                {
                    ModelState.AddModelError("GiaXe", "Vui lòng nhập giá thuê xe.");
                }
                else if (xe.GiaXe.Value < 500000 || xe.GiaXe.Value > 5000000)
                {
                    ModelState.AddModelError("GiaXe", "Giá thuê xe phải từ 500,000đ đến 5,000,000đ một ngày.");
                }

                // Kiểm tra biển số xe đã tồn tại
                var existingXe = database.Xe.FirstOrDefault(x => x.BienSoXe == xe.BienSoXe);
                if (existingXe != null)
                {
                    ModelState.AddModelError("BienSoXe", "Biển số xe đã tồn tại trong hệ thống.");
                    return View(xe);
                }

                // Kiểm tra ảnh tải lên
                if (xe.UploadImage == null || xe.UploadImage.ContentLength == 0)
                {
                    ModelState.AddModelError("UploadImage", "Vui lòng chọn hình ảnh cho xe.");
                    return View(xe);
                }
                else
                {
                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(xe.UploadImage.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("UploadImage", "Chỉ chấp nhận file ảnh có định dạng .jpg, .jpeg hoặc .png");
                        return View(xe);
                    }

                    // Kiểm tra kích thước file (giới hạn 5MB)
                    if (xe.UploadImage.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadImage", "Kích thước file không được vượt quá 5MB");
                        return View(xe);
                    }
                }

                if (ModelState.IsValid)
                {
                    string filename = Path.GetFileNameWithoutExtension(xe.UploadImage.FileName);
                    string extension = Path.GetExtension(xe.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    xe.ImageXe = "~/Content/images/" + filename;
                    xe.UploadImage.SaveAs(path);
                    database.Xe.Add(xe);
                    database.SaveChanges();
                    return RedirectToAction("ViewXe");
                }
                return View(xe);
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
            if (string.IsNullOrEmpty(xe.ImageXe))
            {
                xe.ImageXe = "~/Content/images/default.jpg";
            }
            return View(xe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditXe(int id, Xe xe, HttpPostedFileBase UploadImage)
        {
            try
            {
                // Kiểm tra các ràng buộc cho HieuXe
                if (string.IsNullOrWhiteSpace(xe.HieuXe))
                {
                    ModelState.AddModelError("HieuXe", "Vui lòng nhập hãng xe.");
                }
                else if (xe.HieuXe.Length < 2 || xe.HieuXe.Length > 50)
                {
                    ModelState.AddModelError("HieuXe", "Hãng xe phải có độ dài từ 2 đến 50 ký tự.");
                }

                // Kiểm tra các ràng buộc cho BienSoXe
                if (string.IsNullOrWhiteSpace(xe.BienSoXe))
                {
                    ModelState.AddModelError("BienSoXe", "Vui lòng nhập biển số xe.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(xe.BienSoXe, @"^\d{2}[A-Z]-\d{3}\.\d{2}$"))
                {
                    ModelState.AddModelError("BienSoXe", "Biển số xe không đúng định dạng (VD: 51F-123.45).");
                }

                // Kiểm tra các ràng buộc cho TaiXe
                if (string.IsNullOrWhiteSpace(xe.TaiXe))
                {
                    ModelState.AddModelError("TaiXe", "Vui lòng nhập tên tài xế.");
                }
                else if (xe.TaiXe.Length < 2 || xe.TaiXe.Length > 100)
                {
                    ModelState.AddModelError("TaiXe", "Tên tài xế phải có độ dài từ 2 đến 100 ký tự.");
                }

                // Kiểm tra các ràng buộc cho SoChoNgoi
                if (!xe.SoChoNgoi.HasValue)
                {
                    ModelState.AddModelError("SoChoNgoi", "Vui lòng nhập số chỗ ngồi.");
                }
                else if (xe.SoChoNgoi.Value < 4 || xe.SoChoNgoi.Value > 50)
                {
                    ModelState.AddModelError("SoChoNgoi", "Số chỗ ngồi phải từ 4 đến 50 chỗ.");
                }

                // Kiểm tra các ràng buộc cho GiaXe (Giá thuê xe theo ngày)
                if (!xe.GiaXe.HasValue)
                {
                    ModelState.AddModelError("GiaXe", "Vui lòng nhập giá thuê xe.");
                }
                else if (xe.GiaXe.Value < 500000 || xe.GiaXe.Value > 5000000)
                {
                    ModelState.AddModelError("GiaXe", "Giá thuê xe phải từ 500,000đ đến 5,000,000đ một ngày.");
                }

                if (ModelState.IsValid)
                {
                    var existingXe = database.Xe.Find(id);
                    if (existingXe == null)
                    {
                        return HttpNotFound();
                    }

                    // Kiểm tra biển số xe đã tồn tại (trừ xe hiện tại)
                    var duplicateBienSo = database.Xe.FirstOrDefault(x => x.BienSoXe == xe.BienSoXe && x.XeID != id);
                    if (duplicateBienSo != null)
                    {
                        ModelState.AddModelError("BienSoXe", "Biển số xe đã tồn tại trong hệ thống.");
                        return View(xe);
                    }

                    // Nếu có ảnh mới, kiểm tra và cập nhật
                    if (UploadImage != null && UploadImage.ContentLength > 0)
                    {
                        // Kiểm tra định dạng file
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(UploadImage.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("UploadImage", "Chỉ chấp nhận file ảnh có định dạng .jpg, .jpeg hoặc .png");
                            return View(xe);
                        }

                        // Kiểm tra kích thước file (giới hạn 5MB)
                        if (UploadImage.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("UploadImage", "Kích thước file không được vượt quá 5MB");
                            return View(xe);
                        }

                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extension = Path.GetExtension(UploadImage.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        existingXe.ImageXe = "~/Content/images/" + filename;
                        UploadImage.SaveAs(path);
                    }

                    // Cập nhật thông tin xe
                    existingXe.HieuXe = xe.HieuXe;
                    existingXe.BienSoXe = xe.BienSoXe;
                    existingXe.TaiXe = xe.TaiXe;
                    existingXe.SoChoNgoi = xe.SoChoNgoi;
                    existingXe.GiaXe = xe.GiaXe;

                    database.Entry(existingXe).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("ViewXe");
                }
                return View(xe);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(xe);
            }
        }

        public ActionResult ViewSpa()
        {
            var spas = database.Spa.ToList();
            return View(spas);
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
      
        [HttpGet]
        public ActionResult DeleteSpa(int id)
        {
            var spa = database.Spa.Find(id);
            if (spa == null)
            {
                return HttpNotFound();
            }
            return View(spa);
        }

        // POST: Xử lý việc xóa spa
        [HttpPost, ActionName("DeleteSpa")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSpaConfirmed(int id)
        {
            try
            {
                var spa = database.Spa.Find(id);
                if (spa == null)
                {
                    return HttpNotFound();
                }

                // Xóa file ảnh cũ nếu có
                if (!string.IsNullOrEmpty(spa.ImageSpa))
                {
                    string fullPath = Server.MapPath(spa.ImageSpa);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                database.Spa.Remove(spa);
                database.SaveChanges();

                TempData["SuccessMessage"] = "Spa đã được xóa thành công.";
                return RedirectToAction("ViewSpa");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa spa: " + ex.Message);
                return View(database.Spa.Find(id));
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
                if (!ModelState.IsValid)
                {
                    return View(spa);
                }

                // Validate TenDichVu
                if (string.IsNullOrEmpty(spa.TenDichVu))
                {
                    ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống");
                    return View(spa);
                }

                // Validate MoTa
                if (string.IsNullOrEmpty(spa.MoTa))
                {
                    ModelState.AddModelError("MoTa", "Mô tả không được để trống");
                    return View(spa);
                }
                if (spa.MoTa.Length < 10 || spa.MoTa.Length > 2000)
                {
                    ModelState.AddModelError("MoTa", "Mô tả phải từ 10 đến 2000 ký tự");
                    return View(spa);
                }

                // Validate GiaDichVu
                if (spa.GiaDichVu.Value <= 0)
                {
                    ModelState.AddModelError("GiaDichVu", "Giá dịch vụ phải lớn hơn 0");
                    return View(spa);
                }

                // Validate ThoiGianDichVu
                if (spa.ThoiGianDichVu.Value <= 0)
                {
                    ModelState.AddModelError("ThoiGianDichVu", "Thời gian dịch vụ phải lớn hơn 0");
                    return View(spa);
                }

                // Validate Image (bắt buộc khi thêm mới)
                if (spa.UploadImage == null)
                {
                    ModelState.AddModelError("UploadImage", "Vui lòng chọn ảnh cho dịch vụ");
                    return View(spa);
                }

                // Validate Image if exists
                if (spa.UploadImage != null)
                {
                    // Check file size (max 5MB)
                    if (spa.UploadImage.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadImage", "Kích thước file không được vượt quá 5MB");
                        return View(spa);
                    }

                    // Check file extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(spa.UploadImage.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("UploadImage", "Chỉ chấp nhận file ảnh có định dạng: .jpg, .jpeg, .png, .gif");
                        return View(spa);
                    }

                    // Handle image upload
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
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(spa);
            }
        }

        public ActionResult EditSpa(int id)
        {
            var spa = database.Spa.Find(id);
            if (spa == null)
            {
                return HttpNotFound();
            }
            return View(spa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSpa(int id, Spa spa)
        {
            try
            {
                var existingSpa = database.Spa.Find(id);
                if (existingSpa == null)
                {
                    return HttpNotFound();
                }

                if (!ModelState.IsValid)
                {
                    // Giữ lại giá trị cũ nếu validation fail
                    spa.ImageSpa = existingSpa.ImageSpa;
                    return View(spa);
                }

                // Validate TenDichVu
                if (string.IsNullOrEmpty(spa.TenDichVu))
                {
                    ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống");
                    spa.TenDichVu = existingSpa.TenDichVu;
                    return View(spa);
                }

                // Validate MoTa
                if (string.IsNullOrEmpty(spa.MoTa))
                {
                    ModelState.AddModelError("MoTa", "Mô tả không được để trống");
                    spa.MoTa = existingSpa.MoTa;
                    return View(spa);
                }
                if (spa.MoTa.Length < 10 || spa.MoTa.Length > 2000)
                {
                    ModelState.AddModelError("MoTa", "Mô tả phải từ 10 đến 2000 ký tự");
                    spa.MoTa = existingSpa.MoTa;
                    return View(spa);
                }

                // Validate GiaDichVu
                if (spa.GiaDichVu.Value <= 0)
                {
                    ModelState.AddModelError("GiaDichVu", "Giá dịch vụ phải lớn hơn 0");
                    spa.GiaDichVu = existingSpa.GiaDichVu;
                    return View(spa);
                }

                // Validate ThoiGianDichVu
                if (spa.ThoiGianDichVu.Value <= 0)
                {
                    ModelState.AddModelError("ThoiGianDichVu", "Thời gian dịch vụ phải lớn hơn 0");
                    spa.ThoiGianDichVu = existingSpa.ThoiGianDichVu;
                    return View(spa);
                }

                // Handle image update
                if (spa.UploadImage != null)
                {
                    // Check file size (max 5MB)
                    if (spa.UploadImage.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadImage", "Kích thước file không được vượt quá 5MB");
                        spa.ImageSpa = existingSpa.ImageSpa;
                        return View(spa);
                    }

                    // Check file extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(spa.UploadImage.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("UploadImage", "Chỉ chấp nhận file ảnh có định dạng: .jpg, .jpeg, .png, .gif");
                        spa.ImageSpa = existingSpa.ImageSpa;
                        return View(spa);
                    }

                    // Upload new image
                    string filename = Path.GetFileNameWithoutExtension(spa.UploadImage.FileName);
                    string extension = Path.GetExtension(spa.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    spa.ImageSpa = "~/Content/images/" + filename;
                    spa.UploadImage.SaveAs(path);

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(existingSpa.ImageSpa))
                    {
                        string oldImagePath = Server.MapPath(existingSpa.ImageSpa);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                else
                {
                    // Giữ nguyên ảnh cũ nếu không upload ảnh mới
                    spa.ImageSpa = existingSpa.ImageSpa;
                }

                // Update existing spa with new values
                database.Entry(existingSpa).CurrentValues.SetValues(spa);
                database.SaveChanges();
                return RedirectToAction("ViewSpa");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(spa);
            }
        }
     
        public ActionResult ViewSpaKH()
        {
            var spas = database.Spa.ToList();
            return View(spas);
        }
    }
}
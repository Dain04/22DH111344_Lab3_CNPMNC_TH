using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models;

namespace DemoDB2.Controllers
{
    public class LoginNhanVienController : Controller
    {
        QLKSEntities database = new QLKSEntities();
        // GET: LoginNhanVien
        public ActionResult SelectIDChucVu()
        {
            ChucVu se_cate = new ChucVu();
            se_cate.ListChucVu = database.ChucVu.ToList<ChucVu>();
            return PartialView("SelectIDChucVu", se_cate);
        }
        public ActionResult LoginNV()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginNV(NhanVien _user)
        {
            var check = database.NhanVien.Where(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin đăng nhập";
                return View("LoginNV");
            }
            else
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["NameUser"] = _user.Email;
                Session["PasswordUser"] = _user.MatKhau;
                Session["NhanVienID"] = check.NhanVienID;
                Session["ChucVuID"] = check.ChucVuID;
                Session["TenChucVu"] = check.ChucVu.TenChucVu;
                return RedirectToAction("TrangChuNV", "HomeNV");
            }
        }
        public ActionResult RegisterNhanVien()
        {
            ViewBag.ListChucVu = database.ChucVu.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNhanVien(NhanVien _user, string ConfirmPassword)
        {
            System.Diagnostics.Debug.WriteLine("RegisterNhanVien method called");
            ViewBag.ListChucVu = database.ChucVu.ToList();

            if (ModelState.IsValid)
            {
                if (_user == null || string.IsNullOrWhiteSpace(_user.Ten) || _user.Ten.Length > 30)
                {
                    ViewBag.ErrorRegister = "Tên của bạn không được để trống hoặc phải có nhiều hơn 30 ký tự";
                    return View(_user);
                }
                if (string.IsNullOrWhiteSpace(_user.Email))
                {
                    ViewBag.ErrorRegister = "Hãy nhập email của bạn vào";
                    return View(_user);
                }

                bool isValidEmail = Regex.IsMatch(_user.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");
                if (!_user.Email.Contains("@") || !isValidEmail)
                {
                    ViewBag.ErrorRegister = "Email không hợp lệ.";
                    return View(_user);
                }
                if(string.IsNullOrWhiteSpace(_user.SoDienThoai)||_user.SoDienThoai.Length != 10)
                {
                    ViewBag.ErrorRegister = "Số điện thoại phải có 10 số";
                    return View(_user);
                }
                if (!_user.ChucVuID.HasValue || _user.ChucVuID.Value == 0)
                {
                    ViewBag.ErrorRegister = "Hãy chọn chức vụ cho tài khoản";
                    return View(_user);
                }
                if (string.IsNullOrWhiteSpace(_user.MatKhau) || _user.MatKhau.Length < 6)
                {
                    ViewBag.ErrorRegister = "Mật khẩu phải có ít nhât 6 ký tự";
                    return View(_user);
                }
                if (_user.MatKhau != ConfirmPassword)
                {
                    ViewBag.ErrorRegister = "Mật khẩu và xác nhận mật khẩu không khớp.";
                    return View(_user);
                }

                var check_Email = database.NhanVien.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_Email == null)
                {
                    database.Configuration.ValidateOnSaveEnabled = false;
                    database.NhanVien.Add(_user);
                    database.SaveChanges();
                    return RedirectToAction("LoginNV");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã tồn tại";
                    return View(_user);
                }
            }
            return View(_user);
        }
        public ActionResult LogOutNV()
        {
            Session.Abandon();
            return RedirectToAction("LoginNV", "LoginNhanVien");
        }
        public ActionResult ProfileNV()
        {
            if (Session["NameUser"] == null || Session["PasswordUser"] == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }

            string nameUser = (string)Session["NameUser"];
            if (Session["NhanVienID"] == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }
            int id = (int)Session["NhanVienID"];

            var user = database.NhanVien
                .Include(nv => nv.ChucVu)
                .FirstOrDefault(s => s.Email == nameUser && s.NhanVienID == id);

            if (user == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }

            
            user.TenChucVu = user.ChucVu?.TenChucVu;

            var luongMoiNhat = database.Luong
                .Where(l => l.NhanVienID == id)
                .OrderByDescending(l => l.Nam)
                .ThenByDescending(l => l.Thang)
                .FirstOrDefault();

            ViewBag.LuongMoiNhat = luongMoiNhat;

            return View(user);
        }
        public ActionResult EditNV(int id)
        {
            NhanVien user = database.NhanVien.FirstOrDefault(u => u.NhanVienID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult EditNV(NhanVien model, string newPassword, string confirmNewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = database.NhanVien.Find(model.NhanVienID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra tên
                if (string.IsNullOrWhiteSpace(model.Ten) || model.Ten.Length > 30)
                {
                    ModelState.AddModelError("Ten", "Tên của bạn không được để trống hoặc phải có ít hơn hoặc bằng 30 ký tự");
                    return View(model);
                }

                // Kiểm tra email
                bool isValidEmail = Regex.IsMatch(model.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");
                if (!isValidEmail)
                {
                    ModelState.AddModelError("Email", "Email không hợp lệ.");
                    return View(model);
                }

                // Kiểm tra số điện thoại
                if (string.IsNullOrWhiteSpace(model.SoDienThoai) || !Regex.IsMatch(model.SoDienThoai, @"^\d{10}$"))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại phải có 10 số");
                    return View(model);
                }


                // Kiểm tra mật khẩu mới nếu được cung cấp
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword.Length < 6)
                    {
                        ModelState.AddModelError("newPassword", "Mật khẩu phải có ít nhất 6 ký tự");
                        return View(model);
                    }
                    if (newPassword != confirmNewPassword)
                    {
                        ModelState.AddModelError("confirmNewPassword", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View(model);
                    }
                    existingUser.MatKhau = newPassword;
                }

                // Cập nhật thông tin
                existingUser.Ten = model.Ten;
                existingUser.DiaChi = model.DiaChi;
                existingUser.SoDienThoai = model.SoDienThoai;
                existingUser.Email = model.Email;
                existingUser.ChucVuID = model.ChucVuID;

                try
                {
                    database.SaveChanges();
                    return RedirectToAction("ProfileNV");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                }
            }
            return View(model);
        }
        public ActionResult ViewNV()
        {
            if (Session["ChucVuID"] == null || (int)Session["ChucVuID"] != 1)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập vào trang này.";
                return View("AccessDenied");
            }
            var nhanVien = database.NhanVien
                   .Include(nv => nv.ChucVu)
                   .Select(nv => new
                   {
                       NhanVienID = nv.NhanVienID,
                       Ten = nv.Ten,
                       DiaChi = nv.DiaChi,
                       SoDienThoai = nv.SoDienThoai,
                       Email = nv.Email,
                       ChucVuID = nv.ChucVuID,
                       TenChucVu = nv.ChucVu.TenChucVu
                   })
                   .ToList()
                   .Select(x => new NhanVien
                   {
                       NhanVienID = x.NhanVienID,
                       Ten = x.Ten,
                       DiaChi = x.DiaChi,
                       SoDienThoai = x.SoDienThoai,
                       Email = x.Email,
                       ChucVuID = x.ChucVuID,
                       TenChucVu = x.TenChucVu
                   })
                   .ToList();
            return View(nhanVien);
        }
        // GET: LoginNhanVien/DeleteNV/5
        public ActionResult DeleteNV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = database.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        [HttpPost, ActionName("DeleteNV")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNVConfirmed(int id)
        {
            NhanVien nhanVien = database.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            database.NhanVien.Remove(nhanVien);
            database.SaveChanges();
            return RedirectToAction("ViewNV");
        }
        public ActionResult CustomerManagement()
        {
            if (Session["ChucVuID"] == null || (int)Session["ChucVuID"] != 1)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập vào trang này.";
                return View("AccessDenied");
            }

            var customers = database.NguoiDung.ToList();
            return View(customers);
        
        }
    }
}
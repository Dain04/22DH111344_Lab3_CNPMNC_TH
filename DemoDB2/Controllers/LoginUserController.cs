using DemoDB2.Models;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DemoDB2.Controllers
{
    public class LoginUserController : Controller
    {
        QLKSEntities database = new QLKSEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAcount(NguoiDung _user)
        {
            var check = database.NguoiDung.Where(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin đăng nhập";
                return View("Index");
            }
            else
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["NameUser"] = _user.Email;
                Session["ID"] = check.NguoiDungID;
                Session["ProfileImage"] = check.ImageUser ?? "/Content/Images/default-avatar.png";
                return RedirectToAction("TrangChu", "Home");
            }
        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(NguoiDung _user, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                bool isValidEmail = Regex.IsMatch(_user.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");

                if (!_user.Email.Contains("@") || !isValidEmail)
                {
                    ViewBag.ErrorRegister = "Email không hợp lệ.";
                    return View();
                }

                if (_user.MatKhau != ConfirmPassword)
                {
                    ViewBag.ErrorRegister = "Mật khẩu và xác nhận mật khẩu không khớp.";
                    return View();
                }

                var check_Email = database.NguoiDung.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_Email == null)
                {
                    database.Configuration.ValidateOnSaveEnabled = false;
                    database.NguoiDung.Add(_user);
                    database.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã tồn tại";
                    return View();
                }
            }
            return View();
        }

        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index", "LoginUser");
        }

        public new ActionResult Profile()
        {
            if (Session["NameUser"] == null || Session["ID"] == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            string nameUser = (string)Session["NameUser"];
            int id = (int)Session["ID"];

            NguoiDung user = database.NguoiDung.Where(s => s.Email == nameUser && s.NguoiDungID == id).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }
            return View(user);
        }

        public ActionResult Edit(int id)
        {
            NguoiDung user = database.NguoiDung.FirstOrDefault(u => u.NguoiDungID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(NguoiDung model, HttpPostedFileBase UploadImage, string newPassword, string confirmNewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = database.NguoiDung.Find(model.NguoiDungID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                existingUser.TenNguoiDung = model.TenNguoiDung;
                existingUser.DiaChi = model.DiaChi;
                existingUser.SoDienThoai = model.SoDienThoai;
                existingUser.Email = model.Email;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword == confirmNewPassword)
                    {
                        existingUser.MatKhau = newPassword;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View(model);
                    }
                }

                // Check for the uploaded image
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    // Check file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(UploadImage.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("UploadImage", "Chỉ hỗ trợ các định dạng hình ảnh: JPEG, PNG.");
                        return View(model);
                    }

                    // Check file size (e.g., max size: 2 MB)
                    const int maxSize = 2 * 1024 * 1024; // 2 MB
                    if (UploadImage.ContentLength > maxSize)
                    {
                        ModelState.AddModelError("UploadImage", "Kích thước tệp không được vượt quá 2 MB.");
                        return View(model);
                    }

                    // Save the image
                    var fileName = Path.GetFileName(UploadImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    UploadImage.SaveAs(path);
                    existingUser.ImageUser = "~/Content/Images/" + fileName;
                    Session["ProfileImage"] = existingUser.ImageUser;
                }

                try
                {
                    database.SaveChanges();
                    return RedirectToAction("Profile");
                }
                catch (DbEntityValidationException dbEx)
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

        public ActionResult ExternalLogin(string provider)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "LoginUser"));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Index");
            }

            // Kiểm tra xem người dùng đã tồn tại trong cơ sở dữ liệu chưa
            var user = database.NguoiDung.FirstOrDefault(u => u.Email == loginInfo.Email);

            if (user == null)
            {
                // Tạo người dùng mới nếu chưa tồn tại
                user = new NguoiDung
                {
                    Email = loginInfo.Email,
                    TenNguoiDung = loginInfo.DefaultUserName ?? "Unknown",
                    MatKhau = "123", // Đặt một mật khẩu mặc định
                                                    
                };
                database.NguoiDung.Add(user);
                try
                {
                    await database.SaveChangesAsync();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    // Log the error or handle it appropriately
                    return RedirectToAction("Error", "Home"); // Redirect to an error page
                }
            }

            // Đăng nhập người dùng
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, loginInfo.ExternalIdentity);

            // Thiết lập các biến session
            Session["NameUser"] = user.Email;
            Session["ID"] = user.NguoiDungID;
          

            return RedirectToAction("TrangChu", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
          
          
          
        }
    }
}

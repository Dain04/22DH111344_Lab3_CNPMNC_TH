using DemoDB2.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class ChucVuController : Controller
    {
        private QLKSEntities database = new QLKSEntities();
      
        // GET: ChucVu
        public ActionResult ViewChucVu()
        {
            if (Session["ChucVuID"] == null || (int)Session["ChucVuID"] != 1)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập vào trang này.";
                return View("AccessDenied");
            }
            var chucVu = database.ChucVu.ToList();
            return View(chucVu);
        }
      
      
        // GET: ChucVu/CreateChucVu
        public ActionResult CreateChucVu()
        {
            return View();
        }

        // POST: ChucVu/CreateChucVu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChucVu([Bind(Include = "TenChucVu,MoTaChucVu")] ChucVu chucVu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    database.ChucVu.Add(chucVu);
                    database.SaveChanges();
                    return RedirectToAction("ViewChucVu");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi tạo chức vụ: " + ex.Message);
                }
            }

            return View(chucVu);
        }
        public ActionResult EditChucVu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChucVu chucVu = database.ChucVu.Find(id);
            if (chucVu == null)
            {
                return HttpNotFound();
            }
            return View(chucVu);
        }

        // POST: ChucVu/EditChucVu/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChucVu([Bind(Include = "ChucVuID,TenChucVu,MoTaChucVu")] ChucVu chucVu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    database.Entry(chucVu).State = EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("ViewChucVu");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật chức vụ: " + ex.Message);
                }
            }
            return View(chucVu);
        }

        // GET: ChucVu/DeleteChucVu/5
        public ActionResult DeleteChucVu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChucVu chucVu = database.ChucVu.Find(id);
            if (chucVu == null)
            {
                return HttpNotFound();
            }
            return View(chucVu);
        }

        // POST: ChucVu/DeleteChucVu/5
        [HttpPost, ActionName("DeleteChucVu")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ChucVu chucVu = database.ChucVu.Find(id);
                if (chucVu == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra xem có nhân viên nào đang giữ chức vụ này không
                if (chucVu.NhanVien.Any())
                {
                    ModelState.AddModelError("", "Không thể xóa chức vụ này vì có nhân viên đang giữ chức vụ.");
                    return View(chucVu);
                }

                database.ChucVu.Remove(chucVu);
                database.SaveChanges();
                return RedirectToAction("ViewChucVu");
            }
            catch (Exception ex)
            {
                // Log the full exception details
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ModelState.AddModelError("", "Lỗi khi xóa chức vụ: " + ex.Message);
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
using DemoDB2.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class ChucVuController : Controller
    {
        private QLKSEntities database = new QLKSEntities();
      
        // GET: ChucVu
        public ActionResult ViewChucVu()
        {
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
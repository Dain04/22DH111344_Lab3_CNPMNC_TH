

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ChucVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChucVu()
        {
            this.NhanVien = new HashSet<NhanVien>();
        }
        public List<ChucVu> ListChucVu { get; internal set; }

        [Required(ErrorMessage = "Mã chức vụ không được để trống")]
        [Display(Name = "Mã chức vụ")]
        public int ChucVuID { get; set; }

        [Required(ErrorMessage = "Tên chức vụ không được để trống")]
        [StringLength(50, ErrorMessage = "Tên chức vụ không được vượt quá 50 ký tự")]
        [Display(Name = "Tên chức vụ")]
        public string TenChucVu { get; set; }

        [Required(ErrorMessage = "Mô tả chức vụ không được để trống")]
        [Display(Name = "Mô tả chức vụ")]
        public string MoTaChucVu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhanVien> NhanVien { get; set; }
    }
}
